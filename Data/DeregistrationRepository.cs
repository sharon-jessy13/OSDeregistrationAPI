using Dapper;
using Microsoft.Data.SqlClient;
using OSDeregistrationAPI.Models;
using System.Data;

namespace OSDeregistrationAPI.Data;

public class DeregistrationRepository : IDeregistrationRepository
{
    private readonly string _connectionString;

    public DeregistrationRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")!;
    }

    public async Task<IEnumerable<Employee>> GetEmployeesForRM(int rmEID)
    {
        using var connection = new SqlConnection(_connectionString);
        return await connection.QueryAsync<Employee>("OSDeregistration_GetAllmyDirectOSEmployees",
        new { EID = rmEID },
        commandType: CommandType.StoredProcedure);
    }

    public async Task<IEnumerable<Employee>> GetAllOSEmployees()
    {
        using var connection = new SqlConnection(_connectionString);
        var data = await connection.QueryAsync(
        "MD_Master_Employee_GetAllEmployeesByResType_DropDown",
        new { TypeCode = "OS" },
        commandType: CommandType.StoredProcedure);

        return data.Select(row => new Employee
    {
        MempId = (int)row.EID,
        Name = (string)row.FullName_1
    }).ToList();
    }

    public async Task<IEnumerable<DeregistrationReason>> GetReasons()
    {
        using var connection = new SqlConnection(_connectionString);
        return await connection.QueryAsync<DeregistrationReason>("OSDeregistration_Master_GetReasonList",
        new { IsActive = 1 },
        commandType: CommandType.StoredProcedure);
    }

    public async Task<IEnumerable<RatingCriterion>> GetRatingCriteria()
    {
        using var connection = new SqlConnection(_connectionString);
        return await connection.QueryAsync<RatingCriterion>("OSDeregistration_Master_GetRatingCriteria",
        commandType: CommandType.StoredProcedure);
    }

    public async Task<Employee?> GetEmployeeDetails(int mempId)
    {
        using var connection = new SqlConnection(_connectionString);
        var parameters = new { MEMPID = mempId };
        var employee = await connection.QuerySingleOrDefaultAsync<Employee>("UQ_Master_Employee_GetEmployeeDetailsByMEMPID", parameters, commandType: CommandType.StoredProcedure);
        if (employee != null)
        {
            employee.CurrentProjects = (await connection.QueryAsync<Project>("OSDeregistration_GetCurrentProjectsOfEmployee", parameters, commandType: CommandType.StoredProcedure)).ToList();
        }
        return employee;
    }

    public async Task<TransportClearanceDto?> GetTransportClearanceStatus(int osdId, int mempId)
    {
        using var conn = new SqlConnection(_connectionString);
        return await conn.QuerySingleOrDefaultAsync<TransportClearanceDto>(
        "OSDeregistration_PopulateTransportClearanceStatusByOSDID",
        new { OSDID = osdId, MEmpID = mempId },
        commandType: CommandType.StoredProcedure);
    }

    public async Task<int> GetApprovalCount(int masterId)
    {
        using var connection = new SqlConnection(_connectionString);
        return await connection.QuerySingleOrDefaultAsync<int>("OSDeregistration_GetDeatilsByMasterID", new { OSDID = masterId }, commandType: CommandType.StoredProcedure);
    }

    public async Task<int> CreateDeregistrationRequest(DeregistrationRequest request)
    {
        using var connection = new SqlConnection(_connectionString);
        var parameters = new DynamicParameters();
        parameters.Add("@OSMEmpID", request.OSMEmpID);
        parameters.Add("@SkillsWorked", request.SkillsWorked);
        parameters.Add("@RelievingDate", request.RelievingDate);
        parameters.Add("@IsRecommendedFurther", request.IsRecommendedFurther);
        parameters.Add("@ReasonID", request.ReasonID);
        parameters.Add("@MasterID", dbType: DbType.Int32, direction: ParameterDirection.Output);
        await connection.ExecuteAsync("OSDeregistration_InsertDetails", parameters, commandType: CommandType.StoredProcedure);
        return parameters.Get<int>("@MasterID");
    }

    public async Task InsertRatings(int masterId, IEnumerable<RatingSubmission> ratings)
    {
        using var connection = new SqlConnection(_connectionString);
        foreach (var rating in ratings)
        {
            await connection.ExecuteAsync("OSDeregistration_InsertRateDetails", new { OSDID = masterId, RatingID = rating.RatingID }, commandType: CommandType.StoredProcedure);
        }
    }

    public async Task UpdateConfirmationCount(int instanceId)
    {
        using var connection = new SqlConnection(_connectionString);
        await connection.ExecuteAsync("OSDeregistration_UpdateConfirmCount", new { InstanceId = instanceId }, commandType: CommandType.StoredProcedure);
    }

    public async Task UpdateActualRelievingDate(int eid, DateTime? relievingDate)
    {
        using var connection = new SqlConnection(_connectionString);
        await connection.ExecuteAsync("ResignationAndRetention_UpdateActualRelievingDate", new { EID = eid, ActualRelievingDate = relievingDate }, commandType: CommandType.StoredProcedure);
    }

    public async Task SaveClearanceItems(string itemXml, int actedByMempId)
    {
        using var connection = new SqlConnection(_connectionString);
        await connection.ExecuteAsync("OSDeregistration_ClearanaceItemsInfo", new { ItemXML = itemXml, ActedbyMempID = actedByMempId }, commandType: CommandType.StoredProcedure);
    }

    public async Task UpdateOsHrRelievingDate(int osMempId, DateTime? relievingDate, int osdId)
    {
        using var connection = new SqlConnection(_connectionString);
        await connection.ExecuteAsync("OSDeregistration_UpdateOSResuptreldate", new { OSMEmpID = osMempId, RelievingDate = relievingDate, OSDID = osdId }, commandType: CommandType.StoredProcedure);
    }
}