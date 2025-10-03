using Dapper;
using Microsoft.Data.SqlClient;
using OSDeregistrationAPI.Data;
using OSDeregistrationAPI.Models;
using System.Data;
using OSDeregistrationAPI.Models.Dtos;
using System.Xml.Linq;

public class DeregistrationRepository : IDeregistrationRepository
{
    private readonly string _connectionString;

    // Corrected constructor for Dependency Injection
    public DeregistrationRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")!;
    }

    private IDbConnection CreateConnection() => new SqlConnection(_connectionString);
    // 1. OSDeregistration_GetAllmyDirectOSEmployees
    public async Task<IEnumerable<Employee>> GetEmployeesForRM(int rmEID)
    {
        using var connection = CreateConnection();
        const string spName = "OSDeregistration_GetAllmyDirectOSEmployees";
        var parameters = new { EID = rmEID };

        var employees = await connection.QueryAsync<Employee, EmployeeNameAndIdDto, Employee>(
            spName,
            (employee, dto) =>
            {
                string rawName = dto.EmplyeeNameAndID;
                int openParen = rawName.LastIndexOf('(');
                employee.FullName = (openParen > 0) ? rawName.Substring(0, openParen).Trim() : rawName;
                return employee;
            },
            parameters,
            commandType: CommandType.StoredProcedure,
            splitOn: "EmplyeeNameAndID");

        return employees;
    }

    // 2. MO_Master_Employee_GetAllEmployeesByResType_DropDown
    public async Task<IEnumerable<Employee>> GetAllOSEmployees()
    {
        using var connection = CreateConnection();
        return await connection.QueryAsync<Employee>(
            "MO_Master_Employee_GetAllEmployeesByResType_DropDown",
            new { TypeCode = "OS" },
            commandType: CommandType.StoredProcedure);
    }

    // 3. OSDeregistration_Master_GetReasonList
    public async Task<IEnumerable<DeregistrationReason>> GetReasons()
    {
        using var connection = new SqlConnection(_connectionString);
        return await connection.QueryAsync<DeregistrationReason>("OSDeregistration_Master_GetReasonList",
            new { IsActive = 1 },
            commandType: CommandType.StoredProcedure);
    }

    // 4. OSDeregistration_Master_GetRatingCriteria
    public async Task<IEnumerable<RatingCriterion>> GetRatingCriteria()
    {
        using var connection = new SqlConnection(_connectionString);
        return await connection.QueryAsync<RatingCriterion>("OSDeregistration_Master_GetRatingCriteria",
            commandType: CommandType.StoredProcedure);
    }

    // 5 & 6. MO_Master_Employee_GetEmployeeDetailsByMEMPID and OSDeregistration_GetCurrentProjectsOfEmployee
    public async Task<Employee?> GetEmployeeDetails(int mempId)
    {
        using var connection = CreateConnection();
        var parameters = new { MEMPID = mempId };

        var dto = await connection.QuerySingleOrDefaultAsync< EmployeeDetailsDto>(
            "MO_Master_Employee_GetEmployeeDetailsByMEMPID",
            parameters,
            commandType: CommandType.StoredProcedure);

        if (dto == null)
        {
            return null; // This path returns a value
        }

        // Map from the DTO to the clean Employee model
        var employee = new Employee
        {
            MempId = dto.MempId,
            FullName = dto.FullName,
            JoinDate = dto.JoinDate,
            PartnerCompanyName = dto.Master_Partner_Company_Name,
            PrimaryGroupName = dto.PrimaryGroupName
        };

        employee.CurrentProjects = (await connection.QueryAsync<Project>(
            "OSDeregistration_GetCurrentProjectsOfEmployee",
            parameters,
            commandType: CommandType.StoredProcedure)).ToList();

        return employee; // The other path also returns a value
    }
    // 7. OSDeregistration_InsertDetails
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

        await connection.ExecuteAsync("OSDeregistration_InsertDetails",
            parameters,
            commandType: CommandType.StoredProcedure);

        return parameters.Get<int>("@MasterID");
    }

    // 8. OSDeregistration_InsertRateDetails
    public async Task InsertRatings(int masterId, IEnumerable<RatingSubmission> ratings)
    {
        using var connection = new SqlConnection(_connectionString);
        foreach (var rating in ratings)
        {
            await connection.ExecuteAsync("OSDeregistration_InsertRateDetails",
                new { OSDID = masterId, RatingID = rating.RatingID },
                commandType: CommandType.StoredProcedure);
        }
    }

    // 9. OSDeregistration_UpdateConfirmCount
    public async Task UpdateConfirmationCount(int instanceId)
    {
        using var connection = new SqlConnection(_connectionString);
        await connection.ExecuteAsync("OSDeregistration_UpdateConfirmCount",
            new { InstanceId = instanceId },
            commandType: CommandType.StoredProcedure);
    }

    // 10. ResignationAndRetention_UpdateActualRelievingDate
    public async Task UpdateActualRelievingDate(int eid, DateTime? relievingDate)
    {
        using var connection = new SqlConnection(_connectionString);
        await connection.ExecuteAsync("ResignationAndRetention_UpdateActualRelievingDate",
            new { EID = eid, ActualRelievingDate = relievingDate },
            commandType: CommandType.StoredProcedure);
    }

    // 11. MSDeregistration_Clearance_InsertXML
    public async Task SaveClearanceItems(string itemXml, int actedByMempId)
    {
        using var connection = new SqlConnection(_connectionString);
        await connection.ExecuteAsync("MSDeregistration_Clearance_InsertXML",
            new { ItemXML = itemXml, ActedbyMempID = actedByMempId },
            commandType: CommandType.StoredProcedure);
    }

    // 12. OSDeregistration_PopulateTransportClearanceStatusByOSDID
    public async Task<TransportClearanceDto?> GetTransportClearanceStatus(int osdId, int mempId)
    {
        using var connection = new SqlConnection(_connectionString);
        return await connection.QuerySingleOrDefaultAsync<TransportClearanceDto>("OSDeregistration_PopulateTransportClearanceStatusByOSDID",
            new { OSDID = osdId, MEmpID = mempId },
            commandType: CommandType.StoredProcedure);
    }

    // 13. OSDeregistration_UpdateOSResuptreldate
    public async Task UpdateStatusOnHrApproval(int osMempId, DateTime? relievingDate, int osdId)
    {
        using var connection = new SqlConnection(_connectionString);
        await connection.ExecuteAsync("OSDeregistration_UpdateOSResuptreldate",
            new { OSMEmpID = osMempId, RelievingDate = relievingDate, OSDID = osdId },
            commandType: CommandType.StoredProcedure);
    }

    // 14. OSDeregistration_GetDeatilsByMasterID
    public async Task<DeregistrationDetailsDto?> GetDeregistrationDetails(int osdId)
    {
        using var conn = new SqlConnection(_connectionString);
        return await conn.QuerySingleOrDefaultAsync<DeregistrationDetailsDto>(
            "OSDeregistration_GetDeatilsByMasterID",
            new { OSDID = osdId },
            commandType: CommandType.StoredProcedure);
    }
}