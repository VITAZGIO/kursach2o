using FurnitureStore.Models;
using Microsoft.EntityFrameworkCore;

namespace FurnitureStore.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<FurnitureItem> FurnitureItems => Set<FurnitureItem>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Manufacturer> Manufacturers => Set<Manufacturer>();
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();
    public DbSet<AppUser> AppUsers => Set<AppUser>();
    public DbSet<CartItem> CartItems => Set<CartItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<FurnitureItem>()
            .HasIndex(item => item.Article)
            .IsUnique();

        modelBuilder.Entity<AppUser>()
            .HasIndex(user => user.Login)
            .IsUnique();

        modelBuilder.Entity<AppUser>()
            .HasOne(user => user.Customer)
            .WithMany()
            .HasForeignKey(user => user.CustomerId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<FurnitureItem>()
            .Property(item => item.Price)
            .HasConversion<double>();

        modelBuilder.Entity<Order>()
            .Property(order => order.TotalAmount)
            .HasConversion<double>();

        modelBuilder.Entity<OrderItem>()
            .Property(item => item.UnitPrice)
            .HasConversion<double>();

        modelBuilder.Entity<OrderItem>()
            .HasOne(item => item.Order)
            .WithMany(order => order.OrderItems)
            .HasForeignKey(item => item.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<OrderItem>()
            .HasOne(item => item.FurnitureItem)
            .WithMany(furniture => furniture.OrderItems)
            .HasForeignKey(item => item.FurnitureItemId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<CartItem>()
            .HasOne(item => item.AppUser)
            .WithMany(user => user.CartItems)
            .HasForeignKey(item => item.AppUserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<CartItem>()
            .HasOne(item => item.FurnitureItem)
            .WithMany()
            .HasForeignKey(item => item.FurnitureItemId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
