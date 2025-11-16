using Microsoft.EntityFrameworkCore;
using WizardAssessment.Domain.Interfaces.Repositories;
using WizardAssessment.Domain.Models.DTOs;
using WizardAssessment.Infrastructure.Data;

namespace WizardAssessment.Infrastructure.Repositories;

public class BucketConfigurationRepository : IBucketConfigurationRepository
{
    private readonly WizardDbContext _context;

    public BucketConfigurationRepository(WizardDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<BucketConfigurationDto>> GetAllAsync()
    {
        return await _context.BucketConfigurations
            .ToListAsync();
    }
}

