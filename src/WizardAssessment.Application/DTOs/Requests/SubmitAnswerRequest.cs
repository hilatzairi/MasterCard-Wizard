namespace WizardAssessment.Application.DTOs.Requests;

public class SubmitAnswerRequest
{
    public Guid SessionId { get; set; }
    public string QuestionCode { get; set; } = string.Empty;
    public string Answer { get; set; } = string.Empty;
}

