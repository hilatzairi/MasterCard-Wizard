using WizardAssessment.Domain.Models.DTOs;

namespace WizardAssessment.Domain.Interfaces.Repositories;

public interface IBucketConfigurationRepository
{
    Task<IEnumerable<BucketConfigurationDto>> GetAllAsync();
}

