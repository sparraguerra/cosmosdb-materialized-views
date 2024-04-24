namespace MaterializedViewProcessor.Models;

public class OrdersByQuarter
{
    public string? Id { get; set; }
    public string? CustomerId { get; set; }
    public string? Customer { get; set; }
    public string? Qtr { get; set; }
    public decimal? NumberOfOrders { get; set; }
    public decimal? Total { get; set; }

    public override string ToString() => $"{Id} {CustomerId} {Customer} {Qtr} {Total}";
}
