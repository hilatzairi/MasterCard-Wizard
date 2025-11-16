namespace WizardAssessment.Domain.Models.DTOs;

public class OrganizationDto
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public DateTime CreatedAt { get; set; }
}

