using WizardAssessment.Domain.Models.DTOs;

namespace WizardAssessment.Domain.Interfaces.Repositories;

public interface IQuestionOptionRepository
{
    Task<IEnumerable<QuestionOptionDto>> GetAllAsync();
}

