using WizardAssessment.Domain.Models;

namespace WizardAssessment.Domain.Interfaces.Navigation;

public interface IQuestionNavigator
{
    string QuestionCode { get; }

    Task<NavigationResult> NavigateAsync(WizardContext context);
}
