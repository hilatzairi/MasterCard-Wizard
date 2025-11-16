using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using WizardAssessment.Domain.Interfaces.Caching;
using WizardAssessment.Domain.Interfaces.Repositories;
using WizardAssessment.Infrastructure.Caching;
using WizardAssessment.Infrastructure.Data;
using WizardAssessment.Infrastructure.Repositories;

namespace WizardAssessment.Infrastructure.Configuration;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDatabase(configuration);
        services.AddRepositories();
        services.AddCaching();

        return services;
    }

    private static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        services.AddDbContext<WizardDbContext>(options =>
            options.UseSqlServer(connectionString));

        return services;
    }

    private static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        // Interface-based repositories
        services.AddScoped<IOrganizationRepository, OrganizationRepository>();
        services.AddScoped<IWizardSessionRepository, WizardSessionRepository>();
        services.AddScoped<IEnvironmentRepository, EnvironmentRepository>();
        services.AddScoped<IQuestionRepository, QuestionRepository>();
        services.AddScoped<IQuestionOptionRepository, QuestionOptionRepository>();
        services.AddScoped<INavigationRuleRepository, NavigationRuleRepository>();
        services.AddScoped<IBucketConfigurationRepository, BucketConfigurationRepository>();

        // Concrete repositories (for caching initialization)
        services.AddScoped<QuestionRepository>();
        services.AddScoped<QuestionOptionRepository>();
        services.AddScoped<NavigationRuleRepository>();
        services.AddScoped<BucketConfigurationRepository>();

        return services;
    }

    private static IServiceCollection AddCaching(this IServiceCollection services)
    {
        services.AddSingleton<ISystemDataCache>(serviceProvider =>
        {
            using var scope = serviceProvider.CreateScope();
            var questionRepo = scope.ServiceProvider.GetRequiredService<QuestionRepository>();
            var optionRepo = scope.ServiceProvider.GetRequiredService<QuestionOptionRepository>();
            var ruleRepo = scope.ServiceProvider.GetRequiredService<NavigationRuleRepository>();
            var bucketRepo = scope.ServiceProvider.GetRequiredService<BucketConfigurationRepository>();
            var logger = serviceProvider.GetRequiredService<ILogger<InMemorySystemDataCache>>();

            var questions = questionRepo.GetAllAsync().GetAwaiter().GetResult();
            var options = optionRepo.GetAllAsync().GetAwaiter().GetResult();
            var rules = ruleRepo.GetAllAsync().GetAwaiter().GetResult();
            var buckets = bucketRepo.GetAllAsync().GetAwaiter().GetResult();

            return new InMemorySystemDataCache(questions, options, rules, buckets, logger);
        });

        return services;
    }
}

