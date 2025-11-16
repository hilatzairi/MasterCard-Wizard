using WizardAssessment.Domain.Models;

namespace WizardAssessment.Domain.Interfaces.Navigation;

public interface INavigationCondition
{
    Task<bool> IsMetAsync(WizardContext context);
}
