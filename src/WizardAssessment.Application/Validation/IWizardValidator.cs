using WizardAssessment.Domain.Models.DTOs;

namespace WizardAssessment.Application.Validation;

public interface IWizardValidator
{
    Task ValidateOrganizationExistsAsync(int organizationId);
    void ValidateSession(WizardSessionDto? session, Guid sessionId);
    void ValidateQuestionMatch(string? expected, string actual);
    void ValidateQuestion(QuestionDto? question, string questionCode);
    Task ValidateAnswerAsync(string questionCode, string answer, int organizationId);
}

