using OSDeregistrationAPI.Models;

namespace OSDeregistrationAPI.Services;

public interface IDeregistrationService
{
    Task<int> SubmitDeregistrationAsync(DeregistrationRequest request);
    Task SubmitClearanceItemsAsync(ClearanceSubmission submission);
}