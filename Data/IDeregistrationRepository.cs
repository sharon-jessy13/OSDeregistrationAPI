using OSDeregistrationAPI.Models;
using System.Data;

namespace OSDeregistrationAPI.Data;

public interface IDeregistrationRepository
{
    Task<IEnumerable<Employee>> GetEmployeesForRM(int rmEID);
    Task<IEnumerable<Employee>> GetAllOSEmployees();
    Task<IEnumerable<DeregistrationReason>> GetReasons();
    Task<IEnumerable<RatingCriterion>> GetRatingCriteria();
    Task<Employee?> GetEmployeeDetails(int mempId);
    Task<TransportClearanceDto?> GetTransportClearanceStatus(int osdId, int mempId);

    Task<int> GetApprovalCount(int masterId);
    Task<int> CreateDeregistrationRequest(DeregistrationRequest request);
    Task InsertRatings(int masterId, IEnumerable<RatingSubmission> ratings);
    Task UpdateConfirmationCount(int instanceId);
    Task UpdateActualRelievingDate(int eid, DateTime? relievingDate);
    Task SaveClearanceItems(string itemXml, int actedByMempId);
    Task UpdateOsHrRelievingDate(int osMempId, DateTime? relievingDate, int osdId);
}