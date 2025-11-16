using Microsoft.EntityFrameworkCore;
using WizardAssessment.Domain.Interfaces.Repositories;
using WizardAssessment.Domain.Models.DTOs;
using WizardAssessment.Infrastructure.Data;

namespace WizardAssessment.Infrastructure.Repositories;

public class WizardSessionRepository : IWizardSessionRepository
{
    private readonly WizardDbContext _context;

    public WizardSessionRepository(WizardDbContext context)
    {
        _context = context;
    }

    public async Task<WizardSessionDto?> GetByIdAsync(Guid sessionId)
    {
        return await _context.WizardSessions
            .FirstOrDefaultAsync(s => s.Id == sessionId);
    }

    public async Task CreateAsync(WizardSessionDto session)
    {
        _context.WizardSessions.Add(session);
        await _context.SaveChangesAsync();
    }

    public async Task SaveAsync(WizardSessionDto session)
    {
        _context.Update(session);
        await _context.SaveChangesAsync();
    }
}
