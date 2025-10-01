namespace OSDeregistrationAPI.Models;

public class DeregistrationRequest
{
    public int OSMEmpID { get; set; }
    public string? SkillsWorked { get; set; }
    public DateTime? RelievingDate { get; set; }
    public bool IsRecommendedFurther { get; set; }
    public int ReasonID { get; set; }
    public List<RatingSubmission> Ratings { get; set; } = new();
}

public class RatingSubmission
{
    public int CriteriaId { get; set; }
    public int RatingID { get; set; }
}