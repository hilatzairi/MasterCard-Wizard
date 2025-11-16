using Microsoft.Extensions.Logging;
using WizardAssessment.Application.DTOs.Requests;
using WizardAssessment.Application.DTOs.Responses;
using WizardAssessment.Application.Validation;
using WizardAssessment.Domain.Interfaces.Caching;
using WizardAssessment.Domain.Interfaces.Engine;
using WizardAssessment.Domain.Interfaces.Repositories;
using WizardAssessment.Domain.Models;
using WizardAssessment.Domain.Models.DTOs;

namespace WizardAssessment.Application.Services;

public class WizardService
{
    private readonly INavigatorRegistry _navigatorRegistry;
    private readonly IOptionsProviderRegistry _optionsProviderRegistry;
    private readonly IWizardSessionRepository _sessionRepository;
    private readonly ISystemDataCache _cache;
    private readonly IWizardValidator _validator;
    private readonly ILogger<WizardService> _logger;

    public WizardService(
        INavigatorRegistry navigatorRegistry,
        IOptionsProviderRegistry optionsProviderRegistry,
        IWizardSessionRepository sessionRepository,
        ISystemDataCache cache,
        IWizardValidator validator,
        ILogger<WizardService> logger)
    {
        _navigatorRegistry = navigatorRegistry;
        _optionsProviderRegistry = optionsProviderRegistry;
        _sessionRepository = sessionRepository;
        _cache = cache;
        _validator = validator;
        _logger = logger;
    }

    public async Task<WizardStepResponse> StartWizardAsync(StartWizardRequest request)
    {
        _logger.LogInformation($"Starting wizard for organization '{request.OrganizationId}'");

        await ValidateStartWizard(request);

        var session = WizardSessionDto.CreateNew(request.OrganizationId);
        await _sessionRepository.CreateAsync(session);

        _logger.LogInformation($"Wizard session '{session.Id}' created for organization '{request.OrganizationId}'");

        var context = new WizardContext
        {
            OrganizationId = session.OrganizationId,
            Answer = ""
        };

        var response = await ExecuteNavigationAsync(session, context);
        await _sessionRepository.SaveAsync(session);

        return response;
    }

    public async Task<WizardStepResponse> SubmitAnswerAsync(SubmitAnswerRequest request)
    {
        _logger.LogInformation($"Processing answer for session '{request.SessionId}', question '{request.QuestionCode}', answer '{request.Answer}'");

        var session = await _sessionRepository.GetByIdAsync(request.SessionId);
        await ValidateSubmitAnswer(session, request);

        var context = new WizardContext
        {
            OrganizationId = session!.OrganizationId,
            Answer = request.Answer
        };

        var response = await ExecuteNavigationAsync(session!, context);
        await _sessionRepository.SaveAsync(session!);

        return response;
    }

    private async Task<WizardStepResponse> ExecuteNavigationAsync(WizardSessionDto session, WizardContext context)
    {
        var navigator = _navigatorRegistry.GetNavigator(session.CurrentQuestionCode!);
        var navResult = await navigator.NavigateAsync(context);

        return await ProcessNavigationResult(navResult, session, context);
    }

    private async Task<WizardStepResponse> ProcessNavigationResult(NavigationResult navResult, WizardSessionDto session, WizardContext context)
    {
        if (navResult.RecommendedBucket != null)
        {
            var response = CompleteWizard(session, navResult);
            _logger.LogInformation($"Wizard completed for session '{session.Id}' with bucket '{navResult.RecommendedBucket}'");
            return response;
        }
        else if (navResult.NextQuestionCode != null)
        {
            var response = await ContinueToNextQuestion(session, navResult, context);
            _logger.LogInformation($"Navigating to next question '{navResult.NextQuestionCode}' for session '{session.Id}'");
            return response;
        }

        throw new InvalidOperationException("Navigator returned neither bucket nor next question");
    }

    private WizardStepResponse CompleteWizard(WizardSessionDto session, NavigationResult navResult)
    {
        session.IsCompleted = true;
        session.RecommendedBucket = navResult.RecommendedBucket;
        session.CompletedAt = DateTime.UtcNow;
        return WizardStepResponse.CreateCompletion(session.Id, navResult.RecommendedBucket!);
    }

    private async Task<WizardStepResponse> ContinueToNextQuestion(WizardSessionDto session, NavigationResult navResult, WizardContext context)
    {
        session.CurrentQuestionCode = navResult.NextQuestionCode;
        QuestionResponse questionData = await ReceiveQuestionData(navResult.NextQuestionCode!, context);
        return WizardStepResponse.CreateNextStep(session.Id, questionData);
    }

    private async Task ValidateStartWizard(StartWizardRequest request)
    {
        await _validator.ValidateOrganizationExistsAsync(request.OrganizationId);
    }

    private async Task ValidateSubmitAnswer(WizardSessionDto? session, SubmitAnswerRequest request)
    {
        _validator.ValidateSession(session, request.SessionId);
        _validator.ValidateQuestionMatch(session!.CurrentQuestionCode, request.QuestionCode);
        await _validator.ValidateAnswerAsync(request.QuestionCode, request.Answer, session.OrganizationId);
    }

    private async Task<QuestionResponse> ReceiveQuestionData(string questionCode, WizardContext context)
    {
        var question = await _cache.GetQuestionAsync(questionCode);
        _validator.ValidateQuestion(question, questionCode);

        var optionsProvider = _optionsProviderRegistry.GetProvider(questionCode);
        var options = await optionsProvider.GetOptionsAsync(context.OrganizationId);

        var optionsResponse = options.Select(o => new OptionResponse
        {
            Value = o.Value,
            DisplayText = o.DisplayText
        });

        return new QuestionResponse
        {
            QuestionCode = question!.Code,
            Text = question.Text,
            Type = question.Type,
            Options = optionsResponse
        };
    }
}

