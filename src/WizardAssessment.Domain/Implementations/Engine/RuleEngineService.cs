using Microsoft.Extensions.Logging;
using WizardAssessment.Domain.Interfaces.Caching;
using WizardAssessment.Domain.Interfaces.Engine;
using WizardAssessment.Domain.Models;
using WizardAssessment.Domain.Models.DTOs;

namespace WizardAssessment.Domain.Implementations.Engine;

public class RuleEngineService : IRuleEngineService
{
    private readonly ISystemDataCache _cache;
    private readonly IConditionRegistry _conditionRegistry;
    private readonly ILogger<RuleEngineService> _logger;

    public RuleEngineService(ISystemDataCache cache, IConditionRegistry conditionRegistry,
        ILogger<RuleEngineService> logger)
    {
        _cache = cache;
        _conditionRegistry = conditionRegistry;
        _logger = logger;
    }

    public async Task<NavigationResult> GetNextStepAsync(string currentQuestionCode, WizardContext context)
    {
        _logger.LogInformation($"Evaluating rules for question '{currentQuestionCode}', answer '{context.Answer}'");

        var rules = await _cache.GetRulesForQuestionAndAnswerAsync(currentQuestionCode, context.Answer);
        var sortedRules = rules.OrderBy(rule => rule.Priority);

        foreach (var rule in sortedRules)
        {
            if (await IsRuleAsync(rule, context))
            {
                var result = CreateResult(rule);
                var destination = result.NextQuestionCode ?? result.RecommendedBucket;
                return result;
            }
        }

        _logger.LogWarning($"No matching rule found for question '{currentQuestionCode}', answer '{context.Answer}'");
        throw new InvalidOperationException(
            $"No matching navigation rule for question '{currentQuestionCode}' with answer '{context.Answer}'");
    }

    private async Task<bool> IsRuleAsync(NavigationRuleDto rule, WizardContext context)
    {
        if (string.IsNullOrEmpty(rule.ConditionType))
            return true;

        var condition = _conditionRegistry.GetCondition(rule.ConditionType)
            ?? throw new InvalidOperationException($"Condition '{rule.ConditionType}' not registered");

        return await condition.IsMetAsync(context);
    }

    private static NavigationResult CreateResult(NavigationRuleDto rule)
    {
        if (!string.IsNullOrEmpty(rule.NextQuestionCode))
            return new NavigationResult { NextQuestionCode = rule.NextQuestionCode };

        if (!string.IsNullOrEmpty(rule.RecommendedBucket))
            return new NavigationResult { RecommendedBucket = rule.RecommendedBucket };

        throw new InvalidOperationException($"Rule {rule.RuleId} has no outcome");
    }
}
