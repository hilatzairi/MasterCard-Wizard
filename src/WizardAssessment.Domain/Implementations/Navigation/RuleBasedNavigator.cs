using WizardAssessment.Domain.Interfaces.Engine;
using WizardAssessment.Domain.Interfaces.Navigation;
using WizardAssessment.Domain.Models;

namespace WizardAssessment.Domain.Implementations.Navigation;

public class RuleBasedNavigator : IQuestionNavigator
{
    private readonly IRuleEngineService _ruleEngine;

    public string QuestionCode { get; }

    public RuleBasedNavigator(string questionCode, IRuleEngineService ruleEngine)
    {
        QuestionCode = questionCode;
        _ruleEngine = ruleEngine;
    }

    public Task<NavigationResult> NavigateAsync(WizardContext context)
    {
        return _ruleEngine.GetNextStepAsync(QuestionCode, context);
    }
}
