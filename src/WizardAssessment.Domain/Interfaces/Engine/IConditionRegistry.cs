using WizardAssessment.Domain.Interfaces.Navigation;

namespace WizardAssessment.Domain.Interfaces.Engine;

public interface IConditionRegistry
{
    INavigationCondition? GetCondition(string conditionType);
}
