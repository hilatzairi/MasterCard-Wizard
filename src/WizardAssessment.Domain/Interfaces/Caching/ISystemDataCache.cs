using WizardAssessment.Domain.Models.DTOs;

namespace WizardAssessment.Domain.Interfaces.Caching;

public interface ISystemDataCache
{
    Task<QuestionDto?> GetQuestionAsync(string code);
    Task<IEnumerable<QuestionDto>> GetAllQuestionsAsync();
    Task<IEnumerable<QuestionOptionDto>> GetOptionsForQuestionAsync(string questionCode);
    Task<IEnumerable<NavigationRuleDto>> GetRulesForQuestionAndAnswerAsync(string questionCode, string answerValue);
    Task<IEnumerable<BucketConfigurationDto>> GetBucketConfigurationsAsync();
}

