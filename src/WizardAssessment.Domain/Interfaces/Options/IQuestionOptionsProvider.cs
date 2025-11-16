using WizardAssessment.Domain.Models.DTOs;

namespace WizardAssessment.Domain.Interfaces.Options;

public interface IQuestionOptionsProvider
{
    string QuestionCode { get; }
    Task<IEnumerable<QuestionOptionDto>> GetOptionsAsync(int organizationId);
}
