namespace CoffeeWise.Data.Entities;

public class Group
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;

    public ICollection<GroupMember> Members { get; set; } = new List<GroupMember>();
}