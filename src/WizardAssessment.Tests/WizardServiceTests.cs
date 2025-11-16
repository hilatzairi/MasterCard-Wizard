using Microsoft.Extensions.Logging;
using Moq;
using WizardAssessment.Application.DTOs.Requests;
using WizardAssessment.Application.Services;
using WizardAssessment.Application.Validation;
using WizardAssessment.Domain.Interfaces.Caching;
using WizardAssessment.Domain.Interfaces.Engine;
using WizardAssessment.Domain.Interfaces.Navigation;
using WizardAssessment.Domain.Interfaces.Options;
using WizardAssessment.Domain.Interfaces.Repositories;
using WizardAssessment.Domain.Models;
using WizardAssessment.Domain.Models.DTOs;
using Xunit;

namespace WizardAssessment.Tests;

public class WizardServiceTests
{
    private readonly Mock<INavigatorRegistry> _navigatorRegistry;
    private readonly Mock<IOptionsProviderRegistry> _optionsRegistry;
    private readonly Mock<IWizardSessionRepository> _sessionRepo;
    private readonly Mock<ISystemDataCache> _cache;
    private readonly Mock<IWizardValidator> _validator;
    private readonly Mock<ILogger<WizardService>> _logger;
    private readonly WizardService _service;

    public WizardServiceTests()
    {
        _navigatorRegistry = new Mock<INavigatorRegistry>();
        _optionsRegistry = new Mock<IOptionsProviderRegistry>();
        _sessionRepo = new Mock<IWizardSessionRepository>();
        _cache = new Mock<ISystemDataCache>();
        _validator = new Mock<IWizardValidator>();
        _logger = new Mock<ILogger<WizardService>>();
        _service = new WizardService(
            _navigatorRegistry.Object,
            _optionsRegistry.Object,
            _sessionRepo.Object,
            _cache.Object,
            _validator.Object,
            _logger.Object);
    }

    [Fact]
    public async Task WizardFlow_CompletesWithBucket()
    {
        var sessionId = Guid.NewGuid();
        var request = new SubmitAnswerRequest
        {
            SessionId = sessionId,
            QuestionCode = "START",
            Answer = "Small"
        };

        var session = new WizardSessionDto
        {
            Id = sessionId,
            CurrentQuestionCode = "START"
        };

        _sessionRepo.Setup(r => r.GetByIdAsync(sessionId)).ReturnsAsync(session);

        var navigator = new Mock<IQuestionNavigator>();

        navigator.Setup(n => n.NavigateAsync(It.Is<WizardContext>(c => c.Answer == "Small")))
            .ReturnsAsync(new NavigationResult { RecommendedBucket = "Lite" });

        _navigatorRegistry.Setup(r => r.GetNavigator("START"))
            .Returns(navigator.Object);

        var response = await _service.SubmitAnswerAsync(request);

        Assert.True(response.IsCompleted);
        Assert.Equal("Lite", response.RecommendedBucket);
    }

    [Fact]
    public async Task WizardFlow_MultipleQuestions_ThenCompletes()
    {
        var sessionId = Guid.NewGuid();

        var session1 = new WizardSessionDto
        {
            Id = sessionId,
            CurrentQuestionCode = "OrgSize"
        };

        _sessionRepo.SetupSequence(r => r.GetByIdAsync(sessionId))
            .ReturnsAsync(session1)
            .ReturnsAsync(new WizardSessionDto
            {
                Id = sessionId,
                CurrentQuestionCode = "Startup"
            });

        var q1Navigator = new Mock<IQuestionNavigator>();
        q1Navigator.Setup(n => n.NavigateAsync(It.Is<WizardContext>(c => c.Answer == "Medium")))
            .ReturnsAsync(new NavigationResult { NextQuestionCode = "Startup" });

        _navigatorRegistry.Setup(r => r.GetNavigator("OrgSize"))
            .Returns(q1Navigator.Object);

        var q2 = new QuestionDto { Code = "Startup", Text = "Question 2", Type = "SingleChoice" };

        _cache.Setup(c => c.GetQuestionAsync("Startup"))
            .ReturnsAsync(q2);

        var q2OptionsProvider = new Mock<IQuestionOptionsProvider>();
        q2OptionsProvider.Setup(p => p.GetOptionsAsync(It.IsAny<int>()))
            .ReturnsAsync(new List<QuestionOptionDto>());

        _optionsRegistry.Setup(r => r.GetProvider("Startup"))
            .Returns(q2OptionsProvider.Object);

        var response1 = await _service.SubmitAnswerAsync(new SubmitAnswerRequest
        {
            SessionId = sessionId,
            QuestionCode = "OrgSize",
            Answer = "Medium"
        });

        Assert.False(response1.IsCompleted);
        Assert.Equal("Startup", response1.Question!.QuestionCode);

        var q2Navigator = new Mock<IQuestionNavigator>();
        q2Navigator.Setup(n => n.NavigateAsync(It.Is<WizardContext>(c => c.Answer == "Yes")))
            .ReturnsAsync(new NavigationResult { RecommendedBucket = "Premium" });

        _navigatorRegistry.Setup(r => r.GetNavigator("Startup"))
            .Returns(q2Navigator.Object);

        var response2 = await _service.SubmitAnswerAsync(new SubmitAnswerRequest
        {
            SessionId = sessionId,
            QuestionCode = "Startup",
            Answer = "Yes"
        });

        Assert.True(response2.IsCompleted);
        Assert.Equal("Premium", response2.RecommendedBucket);
    }
}

