using WizardAssessment.Domain.Interfaces.Navigation;
using WizardAssessment.Domain.Interfaces.Repositories;
using WizardAssessment.Domain.Models;

namespace WizardAssessment.Domain.Implementations.Navigation;

public class HasEnvironmentsCondition : INavigationCondition
{
    private readonly IEnvironmentRepository _environmentRepository;

    public HasEnvironmentsCondition(IEnvironmentRepository environmentRepository)
    {
        _environmentRepository = environmentRepository;
    }

    public async Task<bool> IsMetAsync(WizardContext context)
    {
        return await _environmentRepository.HasEnvironmentsAsync(context.OrganizationId);
    }
}
