namespace OSDeregistrationAPI.Models;

public class Employee
{
    public int MempId { get; set; }
    public string? Name { get; set; }
    public DateTime JoinDate { get; set; }
    public string? PartnerCompany { get; set; }
    public string? Group { get; set; }
    public List<Project> CurrentProjects { get; set; } = new();
}

public class Project
{
    public int ProjectId { get; set; }
    public string? ProjectName { get; set; }
}

public class DeregistrationDetailsDto
{
    public string? EmployeeID { get; set; }
    public string? FullName { get; set; }
    public int CountConfirmed { get; set; }
    public DateTime? RelievingDate { get; set; }
    public string? SkillsWorked { get; set; }
    public string? GroupName { get; set; }
    public string? PartnerCompanyName { get; set; }
    public int OSMEmpID { get; set; }
    public DateTime JoinDate { get; set; }
}