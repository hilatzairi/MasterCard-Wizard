using Microsoft.Extensions.Logging;
using WizardAssessment.Domain.Interfaces.Engine;
using WizardAssessment.Domain.Interfaces.Navigation;

namespace WizardAssessment.Domain.Implementations.Engine;

public class NavigatorRegistry : INavigatorRegistry
{
    private readonly IReadOnlyDictionary<string, IQuestionNavigator> _navigators;
    private readonly ILogger<NavigatorRegistry> _logger;

    public NavigatorRegistry(IEnumerable<IQuestionNavigator> navigators, ILogger<NavigatorRegistry> logger)
    {
        _navigators = navigators.ToDictionary(n => n.QuestionCode, n => n);
        _logger = logger;
    }

    public IQuestionNavigator GetNavigator(string questionCode)
    {
        if (_navigators.TryGetValue(questionCode, out var navigator))
            return navigator;

        _logger.LogWarning($"No navigator registered for question '{questionCode}'");
        throw new InvalidOperationException($"No navigator registered for question code '{questionCode}'.");
    }
}

