using WizardAssessment.Domain.Models;

namespace WizardAssessment.Domain.Interfaces.Engine;

public interface IRuleEngineService
{
    Task<NavigationResult> GetNextStepAsync(string currentQuestionCode, WizardContext context);
}
