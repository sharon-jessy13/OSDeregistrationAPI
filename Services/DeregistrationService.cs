using OSDeregistrationAPI.Data;
using OSDeregistrationAPI.Models;
using System.Text;

namespace OSDeregistrationAPI.Services;

public class DeregistrationService : IDeregistrationService
{
    private readonly IDeregistrationRepository _repository;

    public DeregistrationService(IDeregistrationRepository repository)
    {
        _repository = repository;
    }

    public async Task<int> SubmitDeregistrationAsync(DeregistrationRequest request)
    {
        int masterId = await _repository.CreateDeregistrationRequest(request);

        if (request.Ratings.Any())
        {
            await _repository.InsertRatings(masterId, request.Ratings);
        }

        return masterId;
    }

    public async Task SubmitClearanceItemsAsync(ClearanceSubmission submission)
    {
        string xml = GenerateClearanceXml(submission);
        await _repository.SaveClearanceItems(xml, submission.ActedByMempId);
    }

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
                sb.Append($"<LastworkingDate>{submission.LastWorkingDate}</LastworkingDate>");
            }
            sb.Append("</Item>");
        }
        
        sb.Append("</root>");
        return sb.ToString();
    }
}