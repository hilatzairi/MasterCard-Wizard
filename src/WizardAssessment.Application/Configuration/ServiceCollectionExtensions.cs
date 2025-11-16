using Microsoft.Extensions.DependencyInjection;
using WizardAssessment.Application.Services;
using WizardAssessment.Application.Validation;

namespace WizardAssessment.Application.Configuration;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<WizardService>();
        services.AddScoped<IWizardValidator, WizardValidator>();

        return services;
    }
}

