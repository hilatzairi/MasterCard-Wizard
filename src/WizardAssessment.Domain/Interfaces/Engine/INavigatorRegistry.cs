using WizardAssessment.Domain.Interfaces.Navigation;

namespace WizardAssessment.Domain.Interfaces.Engine;

public interface INavigatorRegistry
{
    IQuestionNavigator GetNavigator(string questionCode);
}
