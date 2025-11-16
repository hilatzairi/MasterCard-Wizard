namespace WizardAssessment.Domain.Models.DTOs;

public class NavigationRuleDto
{
    public int RuleId { get; set; }

    public required string CurrentQuestionCode { get; set; }

    public string? AnswerValue { get; set; }

    public string? NextQuestionCode { get; set; }

    public string? RecommendedBucket { get; set; }

    public string? ConditionType { get; set; }

    public int Priority { get; set; }
}

