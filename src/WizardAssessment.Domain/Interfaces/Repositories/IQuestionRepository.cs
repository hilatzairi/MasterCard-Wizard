using WizardAssessment.Domain.Models.DTOs;

namespace WizardAssessment.Domain.Interfaces.Repositories;

public interface IQuestionRepository
{
    Task<IEnumerable<QuestionDto>> GetAllAsync();
}

