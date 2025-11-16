using Microsoft.AspNetCore.Mvc;
using WizardAssessment.Application.DTOs.Requests;
using WizardAssessment.Application.DTOs.Responses;
using WizardAssessment.Application.Services;

namespace WizardAssessment.API.Controllers;

[ApiController]
[Route("api/wizard")]
public class WizardController : ControllerBase
{
    private readonly WizardService _wizardService;

    public WizardController(WizardService wizardService)
    {
        _wizardService = wizardService;
    }

    [HttpPost("start")]
    public async Task<ActionResult<WizardStepResponse>> StartWizard([FromBody] StartWizardRequest request)
    {
        var response = await _wizardService.StartWizardAsync(request);
        return Ok(response);
    }

    [HttpPost("sessions/{sessionId}/answer")]
    public async Task<ActionResult<WizardStepResponse>> SubmitAnswer([FromRoute] Guid sessionId, [FromBody] SubmitAnswerRequest request)
    {
        request.SessionId = sessionId;
        var response = await _wizardService.SubmitAnswerAsync(request);
        return Ok(response);
    }
}
