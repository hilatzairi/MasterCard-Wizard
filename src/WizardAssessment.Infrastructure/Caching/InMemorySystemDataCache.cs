using Microsoft.Extensions.Logging;
using WizardAssessment.Domain.Interfaces.Caching;
using WizardAssessment.Domain.Models.DTOs;

namespace WizardAssessment.Infrastructure.Caching;

public class InMemorySystemDataCache : ISystemDataCache
{
    private readonly Dictionary<string, QuestionDto> _questions;
    private readonly Dictionary<string, List<QuestionOptionDto>> _optionsByQuestion;
    private readonly Dictionary<string, Dictionary<string, List<NavigationRuleDto>>> _rulesByQuestionAndAnswer;
    private readonly List<BucketConfigurationDto> _bucketConfigurations;

    public InMemorySystemDataCache(
        IEnumerable<QuestionDto> questions,
        IEnumerable<QuestionOptionDto> options,
        IEnumerable<NavigationRuleDto> rules,
        IEnumerable<BucketConfigurationDto> bucketConfigurations,
        ILogger<InMemorySystemDataCache> logger)
    {
        _questions = BuildQuestionsDictionary(questions);
        _optionsByQuestion = BuildOptionsDictionary(options);
        _rulesByQuestionAndAnswer = BuildRulesDictionary(rules);
        _bucketConfigurations = bucketConfigurations.ToList();

        logger.LogInformation($"System data cache initialized with '{_questions.Count}' questions, '{_optionsByQuestion.Count}' question groups, '{_rulesByQuestionAndAnswer.Count}' rule groups, '{_bucketConfigurations.Count}' bucket configs");
    }

    private static Dictionary<string, QuestionDto> BuildQuestionsDictionary(IEnumerable<QuestionDto> questions)
    {
        return questions.ToDictionary(q => q.Code);
    }

    private static Dictionary<string, List<QuestionOptionDto>> BuildOptionsDictionary(IEnumerable<QuestionOptionDto> options)
    {
        return options.GroupBy(o => o.QuestionCode).ToDictionary(g => g.Key, g => g.ToList());
    }

    private static Dictionary<string, Dictionary<string, List<NavigationRuleDto>>> BuildRulesDictionary(IEnumerable<NavigationRuleDto> rules)
    {
        return rules.GroupBy(r => r.CurrentQuestionCode)
            .ToDictionary(
                qGroup => qGroup.Key,
                qGroup => qGroup.GroupBy(r => r.AnswerValue ?? string.Empty)
                    .ToDictionary(aGroup => aGroup.Key, aGroup => aGroup.ToList())
            );
    }

    public Task<QuestionDto?> GetQuestionAsync(string code)
    {
        return Task.FromResult(_questions.GetValueOrDefault(code));
    }

    public Task<IEnumerable<QuestionDto>> GetAllQuestionsAsync()
    {
        return Task.FromResult<IEnumerable<QuestionDto>>(_questions.Values);
    }

    public Task<IEnumerable<QuestionOptionDto>> GetOptionsForQuestionAsync(string questionCode)
    {
        return Task.FromResult(_optionsByQuestion.GetValueOrDefault(questionCode) ?? Enumerable.Empty<QuestionOptionDto>());
    }

    public Task<IEnumerable<NavigationRuleDto>> GetRulesForQuestionAndAnswerAsync(string questionCode, string answerValue)
    {
        if (_rulesByQuestionAndAnswer.TryGetValue(questionCode, out var answerRules))
            return Task.FromResult(answerRules.GetValueOrDefault(answerValue) ?? Enumerable.Empty<NavigationRuleDto>());

        return Task.FromResult(Enumerable.Empty<NavigationRuleDto>());
    }

    public Task<IEnumerable<BucketConfigurationDto>> GetBucketConfigurationsAsync()
    {
        return Task.FromResult<IEnumerable<BucketConfigurationDto>>(_bucketConfigurations);
    }
}

