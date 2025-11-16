using Microsoft.EntityFrameworkCore;
using WizardAssessment.Domain.Interfaces.Repositories;
using WizardAssessment.Domain.Models.DTOs;
using WizardAssessment.Infrastructure.Data;

namespace WizardAssessment.Infrastructure.Repositories;

public class QuestionRepository : IQuestionRepository
{
    private readonly WizardDbContext _context;

    public QuestionRepository(WizardDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<QuestionDto>> GetAllAsync()
    {
        return await _context.Questions
            .ToListAsync();
    }
}

