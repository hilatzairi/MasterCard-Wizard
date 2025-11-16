namespace WizardAssessment.Domain.Models.DTOs;

public class BucketConfigurationDto
{
    public required string BucketName { get; set; }
    public int MinEnvironments { get; set; }
    public int? MaxEnvironments { get; set; }
}

