namespace WizardAssessment.Domain.Models.DTOs;

public class WizardSessionDto
{
    public Guid Id { get; set; }
    public int OrganizationId { get; set; }
    public string? CurrentQuestionCode { get; set; }
    public bool IsCompleted { get; set; }
    public string? RecommendedBucket { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? CompletedAt { get; set; }

    public static WizardSessionDto CreateNew(int organizationId)
    {
        return new WizardSessionDto
        {
            Id = Guid.NewGuid(),
            OrganizationId = organizationId,
            CurrentQuestionCode = CustomQuestionCodes.Start,
            IsCompleted = false,
            CreatedAt = DateTime.UtcNow
        };
    }
}

