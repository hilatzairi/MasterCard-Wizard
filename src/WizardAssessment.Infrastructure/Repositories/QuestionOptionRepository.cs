using Microsoft.EntityFrameworkCore;
using WizardAssessment.Domain.Interfaces.Repositories;
using WizardAssessment.Domain.Models.DTOs;
using WizardAssessment.Infrastructure.Data;

namespace WizardAssessment.Infrastructure.Repositories;

public class QuestionOptionRepository : IQuestionOptionRepository
{
    private readonly WizardDbContext _context;

    public QuestionOptionRepository(WizardDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<QuestionOptionDto>> GetAllAsync()
    {
        return await _context.QuestionOptions
            .ToListAsync();
    }
}

