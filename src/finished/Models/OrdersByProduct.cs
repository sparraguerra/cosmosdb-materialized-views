namespace MaterializedViewProcessor.Models;

public class OrdersByProduct
{
    public string? Id { get; set; }
    public string? ProductId { get; set; }
    public string? Product { get; set; }
    public decimal? Quantity { get; set; }
    public decimal? Total { get; set; }

    public override string ToString() => $"{Id} {ProductId} {Product} {Total}";
}
