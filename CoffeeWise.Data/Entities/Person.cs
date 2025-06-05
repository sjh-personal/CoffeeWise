namespace CoffeeWise.Data.Entities;

public class Person
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string? SlackUserId { get; set; }

    public ICollection<GroupMember> GroupMemberships { get; set; } = new List<GroupMember>();
}
