using EnvironmentDto = WizardAssessment.Domain.Models.DTOs.EnvironmentDto;

namespace WizardAssessment.Domain.Interfaces.Repositories;

public interface IEnvironmentRepository
{
    Task<IEnumerable<EnvironmentDto>> GetByOrganizationIdAsync(int organizationId);
    Task<bool> HasEnvironmentsAsync(int organizationId);
}
