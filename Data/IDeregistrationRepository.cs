using OSDeregistrationAPI.Models;
using OSDeregistrationAPI.Models.Dtos;
namespace OSDeregistrationAPI.Data;

public interface IDeregistrationRepository
{
    // Methods to retrieve information for the form
    Task<IEnumerable<EmployeeRmDto>> GetEmployeesForRM(int rmEID);
    Task<IEnumerable<Employee>> GetAllOSEmployees();
    Task<IEnumerable<DeregistrationReason>> GetReasons();
    Task<IEnumerable<RatingCriterion>> GetRatingCriteria();
    Task<Employee?> GetEmployeeDetails(int mempId);
    Task<TransportClearanceDto?> GetTransportClearanceStatus(int osdId, int mempId);
    Task<DeregistrationDetailsDto?> GetDeregistrationDetails(int osdId);
    
    // Methods to create and update the deregistration request
    Task<int> CreateDeregistrationRequest(DeregistrationRequest request);
    Task InsertRatings(int masterId, IEnumerable<RatingSubmission> ratings);
    Task SaveClearanceItems(string itemXml, int actedByMempId);

    // Methods for the approval workflow and finalization
    Task UpdateConfirmationCount(int instanceId);
    Task UpdateStatusOnHrApproval(int osMempId, DateTime? relievingDate, int osdId);
    Task UpdateActualRelievingDate(int eid, DateTime? relievingDate);
}