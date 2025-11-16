using WizardAssessment.Domain.Interfaces.Caching;
using WizardAssessment.Domain.Interfaces.Options;
using WizardAssessment.Domain.Models.DTOs;

namespace WizardAssessment.Domain.Implementations.Options;

public class StaticQuestionOptionsProvider : IQuestionOptionsProvider
{
    private readonly ISystemDataCache _cache;
    public string QuestionCode { get; }

    public StaticQuestionOptionsProvider(string questionCode, ISystemDataCache cache)
    {
        QuestionCode = questionCode;
        _cache = cache;
    }

    public Task<IEnumerable<QuestionOptionDto>> GetOptionsAsync(int organizationId)
    {
        return _cache.GetOptionsForQuestionAsync(QuestionCode);
    }
}
