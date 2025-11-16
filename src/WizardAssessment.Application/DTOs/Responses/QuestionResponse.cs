namespace WizardAssessment.Application.DTOs.Responses;

public class QuestionResponse
{
    public string? QuestionCode { get; set; }
    public string? Text { get; set; }
    public string? Type { get; set; }
    public IEnumerable<OptionResponse>? Options { get; set; }
}

