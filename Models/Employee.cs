namespace OSDeregistrationAPI.Models;

public class Employee
{
    public int MEMPID { get; set; }
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