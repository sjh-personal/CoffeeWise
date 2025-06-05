using Microsoft.EntityFrameworkCore;
using CoffeeWise.Data.Entities;

namespace CoffeeWise.Data;

public class CoffeeWiseDbContext(DbContextOptions<CoffeeWiseDbContext> options) : DbContext(options)
{
    public DbSet<Person> Persons => Set<Person>();
    public DbSet<Group> Groups => Set<Group>();
    public DbSet<GroupMember> GroupMembers => Set<GroupMember>();
    public DbSet<Presence> Presences => Set<Presence>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<GroupMember>()
            .HasIndex(gm => new { gm.PersonId, gm.GroupId })
            .IsUnique();
        
        modelBuilder.Entity<Presence>()
            .HasIndex(p => new { p.PersonId, p.Date })
            .IsUnique();
        
        modelBuilder.Entity<Order>()
            .HasMany(o => o.Items)
            .WithOne(oi => oi.Order)
            .HasForeignKey(oi => oi.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<OrderItem>()
            .HasOne(oi => oi.GroupMember)
            .WithMany()
            .HasForeignKey(oi => oi.GroupMemberId)
            .OnDelete(DeleteBehavior.Restrict);
        
        modelBuilder.Entity<Presence>()
            .Property(p => p.Date)
            .HasColumnType("date");
    }
}