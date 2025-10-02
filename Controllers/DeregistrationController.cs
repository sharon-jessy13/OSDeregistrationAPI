using Microsoft.AspNetCore.Mvc;
using OSDeregistrationAPI.Data;
using OSDeregistrationAPI.Models;
using System.Text;

[ApiController]
[Route("api/[controller]")]
public class DeregistrationController : ControllerBase
{
    private readonly IDeregistrationRepository _repository;

    public DeregistrationController(IDeregistrationRepository repository)
    {
        _repository = repository;
    }

    // --- GET Endpoints ---
    [HttpGet("employees/rm/{rmId}")]
    public async Task<IActionResult> GetEmployeesForManager(int rmId)
    {
        var data = await _repository.GetEmployeesForRM(rmId);
        return Ok(data);
    }
    
    [HttpGet("employees/hr")]
    public async Task<IActionResult> GetEmployeesForHr()
    {
        var data = await _repository.GetAllOSEmployees();
        return Ok(data);
    }

    [HttpGet("lookup/reasons")]
    public async Task<IActionResult> GetReasons()
    {
        var data = await _repository.GetReasons();
        return Ok(data);
    }
    
    [HttpGet("lookup/rating-criteria")]
    public async Task<IActionResult> GetRatingCriteria()
    {
        var data = await _repository.GetRatingCriteria();
        return Ok(data);
    }

    [HttpGet("employees/{mempId}/details")]
    public async Task<IActionResult> GetEmployeeDetails(int mempId)
    {
        var data = await _repository.GetEmployeeDetails(mempId);
        if (data == null)
        {
            return NotFound($"No employee found with MEMPID: {mempId}");
        }

        return Ok(data);
    }

    [HttpGet("clearance/transport-status")]
    public async Task<IActionResult> GetTransportStatus([FromQuery] int osdId, [FromQuery] int mempId)
    {
        var data = await _repository.GetTransportClearanceStatus(osdId, mempId);
        return Ok(data);
    }
[HttpGet("details/{osdId}")]
    public async Task<IActionResult> GetDeregistrationDetails(int osdId)
    {
        var data = await _repository.GetDeregistrationDetails(osdId);
        if (data == null)
        {
            return NotFound();
        }
        return Ok(data);
    }
    // --- POST/PUT Endpoints ---
    [HttpPost("submit")]
    public async Task<IActionResult> SubmitDeregistration([FromBody] DeregistrationRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        
        int masterId = await _repository.CreateDeregistrationRequest(request);

        if (request.Ratings != null && request.Ratings.Any())
        {
            await _repository.InsertRatings(masterId, request.Ratings);
        }

        return Ok(new { MasterID = masterId });
    }
    
    [HttpPost("clearance/items")]
    public async Task<IActionResult> SubmitClearanceItems([FromBody] ClearanceSubmission submission)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        
        string xml = GenerateClearanceXml(submission);
        await _repository.SaveClearanceItems(xml, submission.ActedByMempId);
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
        await _repository.UpdateStatusOnHrApproval(model.OsMempId, model.RelievingDate, model.OsdId);
        return Ok();
    }
    public record OsHrApprovalModel(int OsMempId, DateTime? RelievingDate, int OsdId);

    private string GenerateClearanceXml(ClearanceSubmission submission)
    {
        var sb = new StringBuilder();
        sb.Append("<root>");

        foreach (var item in submission.Items)
        {
            sb.Append("<Item>");
            sb.Append($"<ItemKeyName>{item.KeyName}</ItemKeyName>");
            sb.Append($"<ItemDispName>{item.DisplayName}</ItemDispName>");
            sb.Append($"<OSDID>{submission.MasterId}</OSDID>");
            sb.Append($"<ControlType>{item.ControlType}</ControlType>");
            sb.Append($"<DeptCatID>{item.DepartmentCategoryId}</DeptCatID>");
            sb.Append($"<ItemTextVal>{item.TextValue ?? ""}</ItemTextVal>");
            sb.Append($"<ItemCheckBoxRbtVal>{item.SelectedValue}</ItemCheckBoxRbtVal>");
            if (submission.LastWorkingDate == DateTime.MinValue)
            {
                sb.Append($"<LastworkingDate>{System.DBNull.Value}</LastworkingDate>");
            }
            else
            {
                sb.Append($"<LastworkingDate>{submission.LastWorkingDate:yyyy-MM-dd}</LastworkingDate>");
            }
            sb.Append("</Item>");
        }
        
        sb.Append("</root>");
        return sb.ToString();
    }
}