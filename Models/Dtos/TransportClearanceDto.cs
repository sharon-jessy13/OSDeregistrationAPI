namespace OSDeregistrationAPI.Models;

public class TransportClearanceDto
{
    // IMPORTANT: Update these properties to match the actual column names 
    // returned by your stored procedure. These are just examples.
    public int IsApplicable { get; set; }
    public decimal PaymentAmount { get; set; }
    public string? Status { get; set; }
}