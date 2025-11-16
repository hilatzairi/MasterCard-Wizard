namespace WizardAssessment.Application.DTOs.Responses;

public class WizardStepResponse
{
    public Guid SessionId { get; set; }
    public bool IsCompleted { get; set; }
    public QuestionResponse? Question { get; set; }
    public string? RecommendedBucket { get; set; }

    public static WizardStepResponse CreateNextStep(Guid sessionId, QuestionResponse question)
    {
        return new WizardStepResponse
        {
            SessionId = sessionId,
            IsCompleted = false,
            Question = question,
            RecommendedBucket = null
        };
    }

    public static WizardStepResponse CreateCompletion(Guid sessionId, string bucket)
    {
        return new WizardStepResponse
        {
            SessionId = sessionId,
            IsCompleted = true,
            Question = null,
            RecommendedBucket = bucket
        };
    }
}

