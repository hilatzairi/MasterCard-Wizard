using WizardAssessment.Domain.Interfaces.Engine;
using WizardAssessment.Domain.Interfaces.Navigation;

namespace WizardAssessment.Domain.Implementations.Engine;

public class ConditionRegistry : IConditionRegistry
{
    private readonly IReadOnlyDictionary<string, INavigationCondition> _conditions;

    public ConditionRegistry(IEnumerable<INavigationCondition> conditions)
    {
        _conditions = conditions.ToDictionary(c => c.GetType().Name.Replace("Condition", ""), c => c);
    }

    public INavigationCondition? GetCondition(string conditionType)
    {
        _conditions.TryGetValue(conditionType, out var condition);
        return condition;
    }
}
