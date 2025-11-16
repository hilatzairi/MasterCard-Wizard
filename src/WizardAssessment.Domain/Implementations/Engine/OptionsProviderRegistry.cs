using Microsoft.Extensions.Logging;
using WizardAssessment.Domain.Interfaces.Engine;
using WizardAssessment.Domain.Interfaces.Options;

namespace WizardAssessment.Domain.Implementations.Engine;

public class OptionsProviderRegistry : IOptionsProviderRegistry
{
    private readonly Dictionary<string, IQuestionOptionsProvider> _providers;
    private readonly ILogger<OptionsProviderRegistry> _logger;

    public OptionsProviderRegistry(IEnumerable<IQuestionOptionsProvider> providers, ILogger<OptionsProviderRegistry> logger)
    {
        _providers = providers.ToDictionary(p => p.QuestionCode);
        _logger = logger;
    }

    public IQuestionOptionsProvider GetProvider(string questionCode)
    {
        if (_providers.TryGetValue(questionCode, out var provider))
            return provider;

        _logger.LogWarning($"No options provider found for question '{questionCode}'");
        throw new KeyNotFoundException($"No options provider found for question: {questionCode}");
    }
}

