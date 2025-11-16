using Microsoft.EntityFrameworkCore;
using WizardAssessment.Domain.Interfaces.Repositories;
using WizardAssessment.Infrastructure.Data;

namespace WizardAssessment.Infrastructure.Repositories;

public class OrganizationRepository : IOrganizationRepository
{
    private readonly WizardDbContext _context;

    public OrganizationRepository(WizardDbContext context)
    {
        _context = context;
    }

    public async Task<bool> ExistsAsync(int organizationId)
    {
        return await _context.Organizations.AnyAsync(o => o.Id == organizationId);
    }
}
