using Microsoft.Extensions.Logging;
using WizardAssessment.Domain.Interfaces.Engine;
using WizardAssessment.Domain.Interfaces.Repositories;
using WizardAssessment.Domain.Models;
using WizardAssessment.Domain.Models.DTOs;

namespace WizardAssessment.Application.Validation;

public class WizardValidator : IWizardValidator
{
    private readonly IOrganizationRepository _organizationRepository;
    private readonly IOptionsProviderRegistry _optionsProviderRegistry;
    private readonly ILogger<WizardValidator> _logger;

    public WizardValidator(
        IOrganizationRepository organizationRepository,
        IOptionsProviderRegistry optionsProviderRegistry,
        ILogger<WizardValidator> logger)
    {
        _organizationRepository = organizationRepository;
        _optionsProviderRegistry = optionsProviderRegistry;
        _logger = logger;
    }

    public async Task ValidateOrganizationExistsAsync(int organizationId)
    {
        var exists = await _organizationRepository.ExistsAsync(organizationId);
        if (!exists)
        {
            _logger.LogWarning($"Organization '{organizationId}' not found");
            throw new InvalidOperationException($"Organization {organizationId} not found");
        }
    }

    public void ValidateSession(WizardSessionDto? session, Guid sessionId)
    {
        if (session == null)
        {
            _logger.LogWarning($"Session '{sessionId}' not found");
            throw new InvalidOperationException($"Session {sessionId} not found");
        }

        if (session.IsCompleted)
        {
            _logger.LogWarning($"Session '{sessionId}' already completed");
            throw new InvalidOperationException("Wizard already completed");
        }
    }

    public void ValidateQuestionMatch(string? expected, string actual)
    {
        if (expected != actual)
            throw new InvalidOperationException($"Expected {expected} but got {actual}");
    }

    public void ValidateQuestion(QuestionDto? question, string questionCode)
    {
        if (question == null)
        {
            _logger.LogWarning($"Question '{questionCode}' not found");
            throw new InvalidOperationException($"Question '{questionCode}' not found");
        }
    }

    public async Task ValidateAnswerAsync(string questionCode, string answer, int organizationId)
    {
        if (string.IsNullOrWhiteSpace(answer))
        {
            _logger.LogWarning($"Empty answer provided for question '{questionCode}'");
            throw new InvalidOperationException("Answer cannot be empty");
        }

        var optionsProvider = _optionsProviderRegistry.GetProvider(questionCode);
        var validOptions = await optionsProvider.GetOptionsAsync(organizationId);
        var validValues = validOptions.Select(o => o.Value).ToHashSet();

        if (questionCode == CustomQuestionCodes.EnvSelection)
        {
            var selectedAnswers = answer.Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(a => a.Trim())
                .ToList();

            foreach (var selectedAnswer in selectedAnswers)
            {
                if (!validValues.Contains(selectedAnswer))
                {
                    _logger.LogWarning($"Invalid answer '{selectedAnswer}' in multi-select for question '{questionCode}'");
                    throw new InvalidOperationException($"Invalid answer '{selectedAnswer}' for question '{questionCode}'");
                }
            }
        }
        else
        {
            if (!validValues.Contains(answer))
            {
                _logger.LogWarning($"Invalid answer '{answer}' for question '{questionCode}'");
                throw new InvalidOperationException($"Invalid answer for question '{questionCode}'");
            }
        }
    }
}

