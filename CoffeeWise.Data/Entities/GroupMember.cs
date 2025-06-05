namespace CoffeeWise.Data.Entities;

public class GroupMember
{
    public Guid Id { get; set; }

    public Guid GroupId { get; set; }
    public Group Group { get; set; } = null!;

    public Guid PersonId { get; set; }
    public Person Person { get; set; } = null!;
}

