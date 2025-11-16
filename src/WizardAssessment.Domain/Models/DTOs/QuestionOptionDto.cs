namespace WizardAssessment.Domain.Models.DTOs;

public class QuestionOptionDto
{
    public int Id { get; set; }
    public required string QuestionCode { get; set; }
    public required string Value { get; set; }
    public required string DisplayText { get; set; }
    public int SortOrder { get; set; }
}

