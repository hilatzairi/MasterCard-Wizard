using WizardAssessment.Domain.Interfaces.Buckets;
using WizardAssessment.Domain.Interfaces.Caching;

namespace WizardAssessment.Domain.Implementations.Buckets;

public class CountBasedBucketCalculator : IBucketCalculator
{
    private readonly ISystemDataCache _cache;

    public CountBasedBucketCalculator(ISystemDataCache cache)
    {
        _cache = cache;
    }

    public async Task<string> Calculate(int count)
    {
        var buckets = await _cache.GetBucketConfigurationsAsync();

        foreach (var bucket in buckets)
        {
            if (count >= bucket.MinEnvironments &&
                (bucket.MaxEnvironments == null || count <= bucket.MaxEnvironments))
            {
                return bucket.BucketName;
            }
        }

        throw new InvalidOperationException("No bucket configurations found");
    }
}
