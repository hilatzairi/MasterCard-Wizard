using WizardAssessment.Domain.Interfaces.Options;
using WizardAssessment.Domain.Interfaces.Repositories;
using WizardAssessment.Domain.Models;
using WizardAssessment.Domain.Models.DTOs;

namespace WizardAssessment.Domain.Implementations.Options;

public class EnvSelectionOptionsProvider : IQuestionOptionsProvider
{
    private readonly IEnvironmentRepository _environmentRepository;

    public string QuestionCode => CustomQuestionCodes.EnvSelection;

    public EnvSelectionOptionsProvider(IEnvironmentRepository environmentRepository)
    {
        _environmentRepository = environmentRepository;
    }

    public async Task<IEnumerable<QuestionOptionDto>> GetOptionsAsync(int organizationId)
    {
        var environments = await _environmentRepository.GetByOrganizationIdAsync(organizationId);

        return environments.Select(e => new QuestionOptionDto
        {
            Value = e.Name,
            DisplayText = e.Name,
            QuestionCode = CustomQuestionCodes.EnvSelection,
            Id = 0,
            SortOrder = 0
        });
    }
}
