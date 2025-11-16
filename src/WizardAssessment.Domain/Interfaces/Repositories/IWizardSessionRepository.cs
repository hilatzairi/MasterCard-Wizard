using WizardAssessment.Domain.Models.DTOs;

namespace WizardAssessment.Domain.Interfaces.Repositories;

public interface IWizardSessionRepository
{
    Task<WizardSessionDto?> GetByIdAsync(Guid sessionId);
    Task CreateAsync(WizardSessionDto session);
    Task SaveAsync(WizardSessionDto session);
}
