using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using WizardAssessment.Domain.Implementations.Buckets;
using WizardAssessment.Domain.Implementations.Engine;
using WizardAssessment.Domain.Implementations.Navigation;
using WizardAssessment.Domain.Implementations.Options;
using WizardAssessment.Domain.Interfaces.Buckets;
using WizardAssessment.Domain.Interfaces.Caching;
using WizardAssessment.Domain.Interfaces.Engine;
using WizardAssessment.Domain.Interfaces.Navigation;
using WizardAssessment.Domain.Interfaces.Options;

namespace WizardAssessment.Domain.Configuration;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDomain(this IServiceCollection services)
    {
        services.AddDomainServices();
        services.AddNavigators();
        services.AddOptionsProviders();
        services.AddBucketCalculators();

        return services;
    }

    private static IServiceCollection AddDomainServices(this IServiceCollection services)
    {
        services.AddScoped<IRuleEngineService, RuleEngineService>();
        services.AddScoped<IConditionRegistry, ConditionRegistry>();
        services.AddScoped<INavigationCondition, HasEnvironmentsCondition>();

        return services;
    }

    private static IServiceCollection AddNavigators(this IServiceCollection services)
    {
        // Register specialist navigators
        services.AddScoped<IQuestionNavigator, EnvSelectionQuestionNavigator>();

        // Register the navigator registry with dynamic rule-based navigators
        services.AddScoped<INavigatorRegistry>(provider =>
        {
            var cache = provider.GetRequiredService<ISystemDataCache>();
            var ruleEngine = provider.GetRequiredService<IRuleEngineService>();
            var logger = provider.GetRequiredService<ILogger<NavigatorRegistry>>();

            var specialistNavigators = provider.GetServices<IQuestionNavigator>().ToList();
            var specialistQuestionCodes = specialistNavigators.Select(n => n.QuestionCode).ToHashSet();

            var allQuestions = cache.GetAllQuestionsAsync().Result;

            var ruleBasedNavigators = allQuestions
                .Where(q => !specialistQuestionCodes.Contains(q.Code))
                .Select(q => new RuleBasedNavigator(q.Code, ruleEngine));

            var allNavigators = specialistNavigators.Concat(ruleBasedNavigators);

            return new NavigatorRegistry(allNavigators, logger);
        });

        return services;
    }

    private static IServiceCollection AddOptionsProviders(this IServiceCollection services)
    {
        // Register specialist options providers
        services.AddScoped<IQuestionOptionsProvider, EnvSelectionOptionsProvider>();

        // Register the options provider registry with dynamic static providers
        services.AddScoped<IOptionsProviderRegistry>(provider =>
        {
            var cache = provider.GetRequiredService<ISystemDataCache>();
            var logger = provider.GetRequiredService<ILogger<OptionsProviderRegistry>>();

            var specialistProviders = provider.GetServices<IQuestionOptionsProvider>().ToList();
            var specialistQuestionCodes = specialistProviders.Select(p => p.QuestionCode).ToHashSet();

            var allQuestions = cache.GetAllQuestionsAsync().Result;

            var staticProviders = allQuestions
                .Where(q => !specialistQuestionCodes.Contains(q.Code))
                .Select(q => new StaticQuestionOptionsProvider(q.Code, cache));

            var allProviders = specialistProviders.Concat(staticProviders);

            return new OptionsProviderRegistry(allProviders, logger);
        });

        return services;
    }

    private static IServiceCollection AddBucketCalculators(this IServiceCollection services)
    {
        services.AddScoped<IBucketCalculator, CountBasedBucketCalculator>();

        return services;
    }
}

