namespace CoffeeWise.Data.Entities;

public class Presence
{
    public Guid Id { get; set; }
    public Guid PersonId { get; set; }
    public DateOnly Date { get; set; }
    public bool IsPresent { get; set; }
}