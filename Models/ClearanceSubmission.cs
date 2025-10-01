namespace OSDeregistrationAPI.Models;

public class ClearanceSubmission
{
    public int ActedByMempId { get; set; }
    public DateTime LastWorkingDate { get; set; }
    public int MasterId { get; set; }
    public List<ClearanceItem> Items { get; set; } = new();
}

public class ClearanceItem
{
    public required string KeyName { get; set; }
    public required string DisplayName { get; set; }
    public int ControlType { get; set; }
    public int DepartmentCategoryId { get; set; }
    public string? TextValue { get; set; }
    public int SelectedValue { get; set; }
}