namespace CoffeeWise.Data.Entities;

public class Order
{
    public Guid Id { get; set; }

    public Guid GroupId { get; set; }
    public Group Group { get; set; } = null!;

    public Guid PayerGroupMemberId { get; set; }
    public GroupMember PayerGroupMember { get; set; } = null!;

    public DateTime Date { get; set; }

    public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
}
