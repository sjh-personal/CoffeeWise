namespace CoffeeWise.Data.Entities;

public class OrderItem
{
    public Guid Id { get; set; }

    public Guid OrderId { get; set; }
    public Order Order { get; set; } = null!;

    public Guid GroupMemberId { get; set; }
    public GroupMember GroupMember { get; set; } = null!;

    public string Description { get; set; } = null!; 
    public decimal Price { get; set; }
}
