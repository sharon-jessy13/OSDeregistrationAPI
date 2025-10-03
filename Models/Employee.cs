// --- In OSDeregistrationAPI/Models/Employee.cs ---
public class Employee
{
    public int MempId { get; set; }
    public string? FullName { get; set; }
    public DateTime JoinDate { get; set; }
    public string? PartnerCompanyName { get; set; }
    public string? PrimaryGroupName { get; set; }
    public List<Project> CurrentProjects { get; set; } = new();
}

// --- In OSDeregistrationAPI/Models/Project.cs ---
public class Project
{
    public int ProjectId { get; set; }
    public string? ProjectName { get; set; }
}