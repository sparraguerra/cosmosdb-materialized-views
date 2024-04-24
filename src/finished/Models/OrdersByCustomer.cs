namespace MaterializedViewProcessor.Models;

public class OrdersByCustomer
{
    public string? Id { get; set; }
    public string? CustomerId { get; set; }
    public string? Customer { get; set; }
    public decimal? Total { get; set; }

    public override string ToString() => $"{Id} {CustomerId} {Customer} {Total}";
}
