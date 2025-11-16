namespace WizardAssessment.Domain.Models.DTOs;

public class EnvironmentDto
{
    public int Id { get; set; }
    public int OrganizationId { get; set; }
    public required string Name { get; set; }
    public DateTime CreatedAt { get; set; }
}

