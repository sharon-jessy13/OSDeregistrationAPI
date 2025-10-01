namespace OSDeregistrationAPI.Models;

public class DeregistrationReason 
{
    public int ReasonID { get; set; }
    public string? Reason { get; set; } 
}

public class RatingCriterion
{
    public int CriteriaId { get; set; }
    public string? CriteriaName { get; set; }
}