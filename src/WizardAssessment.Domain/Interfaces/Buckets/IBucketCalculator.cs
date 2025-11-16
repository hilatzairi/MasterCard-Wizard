namespace WizardAssessment.Domain.Interfaces.Buckets;

public interface IBucketCalculator
{
    Task<string> Calculate(int count);
}
