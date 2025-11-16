namespace WizardAssessment.Domain.Models.DTOs;

public class SessionAnswerDto
{
    public int Id { get; set; }
    public Guid SessionId { get; set; }
    public required string QuestionCode { get; set; }
    public required string Answer { get; set; }
    public DateTime AnsweredAt { get; set; }
}

