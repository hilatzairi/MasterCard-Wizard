namespace WizardAssessment.Domain.Interfaces.Repositories;

public interface IOrganizationRepository
{
    Task<bool> ExistsAsync(int organizationId);
}
