using Microsoft.EntityFrameworkCore;
using WizardAssessment.Domain.Interfaces.Repositories;
using WizardAssessment.Infrastructure.Data;
using EnvironmentDto = WizardAssessment.Domain.Models.DTOs.EnvironmentDto;

namespace WizardAssessment.Infrastructure.Repositories;

public class EnvironmentRepository : IEnvironmentRepository
{
    private readonly WizardDbContext _context;

    public EnvironmentRepository(WizardDbContext context)
    {
        _context = context;
    }

    public async Task<bool> HasEnvironmentsAsync(int organizationId)
    {
        return await _context.Environments.AnyAsync(e => e.OrganizationId == organizationId);
    }

    public async Task<IEnumerable<EnvironmentDto>> GetByOrganizationIdAsync(int organizationId)
    {
        return await _context.Environments
            .Where(e => e.OrganizationId == organizationId)
            .ToListAsync();
    }
}
