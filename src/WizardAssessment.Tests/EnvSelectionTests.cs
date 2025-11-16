using Moq;
using WizardAssessment.Domain.Implementations.Buckets;
using WizardAssessment.Domain.Implementations.Navigation;
using WizardAssessment.Domain.Implementations.Options;
using WizardAssessment.Domain.Interfaces.Caching;
using WizardAssessment.Domain.Interfaces.Repositories;
using WizardAssessment.Domain.Models;
using WizardAssessment.Domain.Models.DTOs;
using Xunit;

namespace WizardAssessment.Tests;

public class EnvSelectionTests
{
    [Theory]
    [InlineData(1, "Lite")]
    [InlineData(3, "Medium")]
    [InlineData(5, "Premium")]
    public async Task BucketCalculator_ReturnsCorrectBucket(int count, string expectedBucket)
    {
        var bucketConfigs = new List<BucketConfigurationDto>
        {
            new() { BucketName = "Lite", MinEnvironments = 1, MaxEnvironments = 1 },
            new() { BucketName = "Medium", MinEnvironments = 2, MaxEnvironments = 3 },
            new() { BucketName = "Premium", MinEnvironments = 4, MaxEnvironments = null }
        };

        var mockCache = new Mock<ISystemDataCache>();
        mockCache.Setup(c => c.GetBucketConfigurationsAsync()).ReturnsAsync(bucketConfigs);

        var calculator = new CountBasedBucketCalculator(mockCache.Object);
        
        var result = await calculator.Calculate(count);

        Assert.Equal(expectedBucket, result);
    }

    [Fact]
    public async Task OptionsProvider_LoadsOrgEnvironments()
    {
        var environments = new List<EnvironmentDto>
        {
            new() { Id = 1, Name = "Production", OrganizationId = 1 },
            new() { Id = 2, Name = "Staging", OrganizationId = 1 }
        };

        var mockRepo = new Mock<IEnvironmentRepository>();
        mockRepo.Setup(r => r.GetByOrganizationIdAsync(1)).ReturnsAsync(environments);

        var provider = new EnvSelectionOptionsProvider(mockRepo.Object);

        var result = await provider.GetOptionsAsync(1);

        Assert.Equal(2, result.Count());
    }

    [Theory]
    [InlineData("Production,Staging,Development", "Medium")]
    [InlineData("Production", "Lite")]
    [InlineData("Prod,Stage,Dev,QA,UAT", "Premium")]
    public async Task EnvSelectionNavigator_ParsesCommaSeparatedList(string commaSeparatedAnswer, string expectedBucket)
    {
        var bucketConfigs = new List<BucketConfigurationDto>
        {
            new() { BucketName = "Lite", MinEnvironments = 1, MaxEnvironments = 1 },
            new() { BucketName = "Medium", MinEnvironments = 2, MaxEnvironments = 3 },
            new() { BucketName = "Premium", MinEnvironments = 4, MaxEnvironments = null }
        };

        var mockCache = new Mock<ISystemDataCache>();
        mockCache.Setup(c => c.GetBucketConfigurationsAsync()).ReturnsAsync(bucketConfigs);

        var calculator = new CountBasedBucketCalculator(mockCache.Object);
        var navigator = new EnvSelectionQuestionNavigator(calculator);

        var context = new WizardContext
        {
            OrganizationId = 1,
            Answer = commaSeparatedAnswer
        };

        var result = await navigator.NavigateAsync(context);

        Assert.Equal(expectedBucket, result.RecommendedBucket);
    }
}

