namespace MaterializedViewProcessor.Models;

public class CreateOrderRequest
{
    public string Id { get; set; } = System.Guid.NewGuid().ToString();
    public string? CustomerId { get; set; }
    public string? Customer { get; set; }
    public DateTime? OrderDate { get; set; }
    public Address? Address { get; set; }
    public List<OrderItem> OrderItems { get; set; } = [];
}

public class Address
{
    public string? Street { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? Zip { get; set; }
    public string? Country { get; set; }

    public override string ToString() => $"{Street} {City} {State} {Zip} {Country}";
}
 
public class OrderItem
{
    public string? ProductId { get; set; }
    public string? Product { get; set; }
    public decimal? Quantity { get; set; }
    public decimal? Price { get; set; }
    public decimal? Total => Quantity * Price;
    public override string ToString() => $"{ProductId} {Product} {Quantity} {Price} {Total}";
}

