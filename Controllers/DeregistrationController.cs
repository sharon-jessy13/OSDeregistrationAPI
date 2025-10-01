using Microsoft.AspNetCore.Mvc;
using OSDeregistrationAPI.Data;
using OSDeregistrationAPI.Models;
using OSDeregistrationAPI.Services;

[ApiController]
[Route("api/[controller]")]
public class DeregistrationController : ControllerBase
{
    private readonly IDeregistrationRepository _repository;
    private readonly IDeregistrationService _service;

    public DeregistrationController(IDeregistrationRepository repository, IDeregistrationService service)
    {
        _repository = repository; 
        _service = service;       
    }

    // --- GET Endpoints ---
    [HttpGet("employees/rm/{rmId}")]
    public async Task<IActionResult> GetEmployeesForManager(int rmId) =>
     Ok(await _repository.GetEmployeesForRM(rmId));
    
    [HttpGet("employees/hr")]
    public async Task<IActionResult> GetEmployeesForHr() =>
    Ok(await _repository.GetAllOSEmployees());

    [HttpGet("lookup/reasons")]
    public async Task<IActionResult> GetReasons() =>
    Ok(await _repository.GetReasons());
    
    [HttpGet("lookup/rating-criteria")]
    public async Task<IActionResult> GetRatingCriteria() =>
    Ok(await _repository.GetRatingCriteria());

    [HttpGet("employees/{mempId}/details")]
    public async Task<IActionResult> GetEmployeeDetails(int mempId) =>
    Ok(await _repository.GetEmployeeDetails(mempId));

    [HttpGet("clearance/transport-status")]
    public async Task<IActionResult> GetTransportStatus([FromQuery] int osdId, [FromQuery] int mempId) => Ok(await _repository.GetTransportClearanceStatus(osdId, mempId));

    [HttpGet("{masterId}/approval-count")]
    public async Task<IActionResult> GetApprovalCount(int masterId) => Ok(await _repository.GetApprovalCount(masterId));

    // --- POST/PUT Endpoints ---
    [HttpPost("submit")]
    public async Task<IActionResult> SubmitDeregistration([FromBody] DeregistrationRequest request)
    {
        int masterId = await _service.SubmitDeregistrationAsync(request);
        return Ok(new { MasterID = masterId });
    }
    
    [HttpPost("clearance/items")]
    public async Task<IActionResult> SubmitClearanceItems([FromBody] ClearanceSubmission submission)
    {
        await _service.SubmitClearanceItemsAsync(submission);
        return Ok();
    }
    
    [HttpPost("clearance/{instanceId}/update-count")]
    public async Task<IActionResult> UpdateClearanceCount(int instanceId)
    {
        await _repository.UpdateConfirmationCount(instanceId);
        return Ok();
    }
    
    [HttpPut("hr-confirmation")]
    public async Task<IActionResult> HrConfirmation([FromBody] HrConfirmationModel model)
    {
        await _repository.UpdateActualRelievingDate(model.EID, model.ActualRelievingDate);
        return Ok();
    }
    public record HrConfirmationModel(int EID, DateTime? ActualRelievingDate);
    
    [HttpPut("os-hr-approval")]
    public async Task<IActionResult> OsHrApproval([FromBody] OsHrApprovalModel model)
    {
        await _repository.UpdateOsHrRelievingDate(model.OsMempId, model.RelievingDate, model.OsdId);
        return Ok();
    }
    public record OsHrApprovalModel(int OsMempId, DateTime? RelievingDate, int OsdId);
}