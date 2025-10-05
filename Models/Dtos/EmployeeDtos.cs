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

// DTO for the final, clean API response for the RM endpoint
public class EmployeeRmDto
{
    public int MempId { get; set; }
    public string? FullName { get; set; }
}

// DTO to perfectly match the columns from the GetEmployeesForRM stored procedure
public class RawEmployeeFromSpDto
{
    public int MempId { get; set; }
    public string? EmplyeeNameAndID { get; set; } // Note the typo to match your SQL
}

public class EmployeeDropdownItemDto
{
    public int MEmpID { get; set; }
    public int EID { get; set; }
    public int EmployeeID { get; set; }
    public string? FullName { get; set; }
    public string? EmployeeNameAndID { get; set; }
    public string? InternalEmpID { get; set; }
}