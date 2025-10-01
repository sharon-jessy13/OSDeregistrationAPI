namespace OSDeregistrationAPI.Models;

public class Reason
{
    public int ReasonID { get; set; }
    public string? ReasonText { get; set; }
}

public class RatingCriterion
{
    public int CriteriaId { get; set; }
    public string? CriteriaName { get; set; }
}