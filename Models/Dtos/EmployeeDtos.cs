

namespace OSDeregistrationAPI.Models.Dtos;

// DTO for the GetEmployeeDetails method
public class EmployeeDetailsDto
{
    public int MempId { get; set; }
    public string? FullName { get; set; }
    public DateTime JoinDate { get; set; }
    public string? Master_Partner_Company_Name { get; set; }
    public string? PrimaryGroupName { get; set; }
}

// DTO for the GetEmployeesForRM method
public class EmployeeNameAndIdDto
{
    public string? EmplyeeNameAndID { get; set; }
}