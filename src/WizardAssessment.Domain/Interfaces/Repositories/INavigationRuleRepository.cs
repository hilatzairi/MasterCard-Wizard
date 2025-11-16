using WizardAssessment.Domain.Models.DTOs;

namespace WizardAssessment.Domain.Interfaces.Repositories;

public interface INavigationRuleRepository
{
    Task<IEnumerable<NavigationRuleDto>> GetAllAsync();
}

