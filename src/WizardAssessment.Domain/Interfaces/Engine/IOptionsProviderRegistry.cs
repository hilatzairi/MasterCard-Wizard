using WizardAssessment.Domain.Interfaces.Options;

namespace WizardAssessment.Domain.Interfaces.Engine;

public interface IOptionsProviderRegistry
{
    IQuestionOptionsProvider GetProvider(string questionCode);
}
