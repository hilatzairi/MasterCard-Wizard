using Microsoft.Extensions.Logging;
using Moq;
using WizardAssessment.Domain.Implementations.Engine;
using WizardAssessment.Domain.Interfaces.Caching;
using WizardAssessment.Domain.Interfaces.Engine;
using WizardAssessment.Domain.Interfaces.Navigation;
using WizardAssessment.Domain.Models;
using WizardAssessment.Domain.Models.DTOs;
using Xunit;

namespace WizardAssessment.Tests;

public class RuleEngineServiceTests
{
    private readonly Mock<ISystemDataCache> _cache;
    private readonly Mock<IConditionRegistry> _conditionRegistry;
    private readonly Mock<ILogger<RuleEngineService>> _logger;
    private readonly RuleEngineService _service;

    public RuleEngineServiceTests()
    {
        _cache = new Mock<ISystemDataCache>();
        _conditionRegistry = new Mock<IConditionRegistry>();
        _logger = new Mock<ILogger<RuleEngineService>>();
        _service = new RuleEngineService(_cache.Object, _conditionRegistry.Object, _logger.Object);
    }

    [Fact]
    public async Task RuleEngine_ReturnsNextQuestionOrBucket()
    {
        var rules = new List<NavigationRuleDto>
        {
            new() { RuleId = 1, CurrentQuestionCode = "Q1", AnswerValue = "Yes", Priority = 1, NextQuestionCode = "Q2" ,ConditionType = null }
        };
        _cache.Setup(c => c.GetRulesForQuestionAndAnswerAsync("Q1", "Yes"))
            .ReturnsAsync(rules);

        var context = new WizardContext { OrganizationId = 1, Answer = "Yes" };
        var result = await _service.GetNextStepAsync("Q1", context);

        Assert.Equal("Q2", result.NextQuestionCode);
    }

    [Fact]
    public async Task RuleEngine_RespectsPriority()
    {
        var rules = new List<NavigationRuleDto>
        {
            new() { RuleId = 2, CurrentQuestionCode = "Q1", AnswerValue = "Yes", Priority = 2, NextQuestionCode = "Q3", ConditionType = null },
            new() { RuleId = 1, CurrentQuestionCode = "Q1", AnswerValue = "Yes", Priority = 1, NextQuestionCode = "Q2", ConditionType = null }
        };
        _cache.Setup(c => c.GetRulesForQuestionAndAnswerAsync("Q1", "Yes"))
            .ReturnsAsync(rules);

        var context = new WizardContext { OrganizationId = 1, Answer = "Yes" };
        var result = await _service.GetNextStepAsync("Q1", context);

        Assert.Equal("Q2", result.NextQuestionCode);
    }

    [Fact]
    public async Task RuleEngine_EvaluatesConditions()
    {
        var mockCondition = new Mock<INavigationCondition>();
        mockCondition.Setup(c => c.IsMetAsync(It.IsAny<WizardContext>())).ReturnsAsync(true);

        var rules = new List<NavigationRuleDto>
        {
            new() { RuleId = 1, CurrentQuestionCode = "Q1", AnswerValue = "Yes", Priority = 1, NextQuestionCode = "Q3", ConditionType = "HasEnvironments" }
        };

        _cache.Setup(c => c.GetRulesForQuestionAndAnswerAsync("Q1", "Yes"))
            .ReturnsAsync(rules);

        _conditionRegistry.Setup(c => c.GetCondition("HasEnvironments"))
            .Returns(mockCondition.Object);

        var context = new WizardContext { OrganizationId = 1, Answer = "Yes" };
        var result = await _service.GetNextStepAsync("Q1", context);

        Assert.Equal("Q3", result.NextQuestionCode);
        mockCondition.Verify(c => c.IsMetAsync(context), Times.Once);
    }

    [Fact]
    public async Task RuleEngine_ReturnsBucket()
    {
        var rules = new List<NavigationRuleDto>
        {
            new() { RuleId = 1, CurrentQuestionCode = "Q1", AnswerValue = "Large", Priority = 1, RecommendedBucket = "Premium", ConditionType = null }
        };

        _cache.Setup(c => c.GetRulesForQuestionAndAnswerAsync("Q1", "Large"))
            .ReturnsAsync(rules);

        var context = new WizardContext { OrganizationId = 1, Answer = "Large" };
        var result = await _service.GetNextStepAsync("Q1", context);

        Assert.Equal("Premium", result.RecommendedBucket);
        Assert.Null(result.NextQuestionCode);
    }
}

