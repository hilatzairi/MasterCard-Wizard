using WizardAssessment.Domain.Interfaces.Buckets;
using WizardAssessment.Domain.Interfaces.Navigation;
using WizardAssessment.Domain.Models;

namespace WizardAssessment.Domain.Implementations.Navigation;

public class EnvSelectionQuestionNavigator : IQuestionNavigator
{
    private readonly IBucketCalculator _bucketCalculator;

    public string QuestionCode => CustomQuestionCodes.EnvSelection;

    public EnvSelectionQuestionNavigator(IBucketCalculator bucketCalculator)
    {
        _bucketCalculator = bucketCalculator;
    }

    public async Task<NavigationResult> NavigateAsync(WizardContext context)
    {
        var selectedEnvironments = context.Answer
            .Split(',', StringSplitOptions.RemoveEmptyEntries);

        var count = selectedEnvironments.Length;
        var bucket = await _bucketCalculator.Calculate(count);

        return new NavigationResult
        {
            RecommendedBucket = bucket
        };
    }
}
