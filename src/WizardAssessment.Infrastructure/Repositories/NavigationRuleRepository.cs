using Microsoft.EntityFrameworkCore;
using WizardAssessment.Domain.Interfaces.Repositories;
using WizardAssessment.Domain.Models.DTOs;
using WizardAssessment.Infrastructure.Data;

namespace WizardAssessment.Infrastructure.Repositories;

public class NavigationRuleRepository : INavigationRuleRepository
{
    private readonly WizardDbContext _context;

    public NavigationRuleRepository(WizardDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<NavigationRuleDto>> GetAllAsync()
    {
        return await _context.NavigationRules
            .ToListAsync();
    }
}

