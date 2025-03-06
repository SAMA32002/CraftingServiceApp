using CraftingServiceApp.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace CraftingServiceApp.Infrastructure.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Service> Services { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<Address> Address { get; set; }
        public DbSet<UserPayment> userPayments { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Service & Category Relationship
            builder.Entity<Service>()
                .HasOne(s => s.Category)
                .WithMany(c => c.Services)
                .HasForeignKey(s => s.CategoryId);

            // Post & Category Relationship
            builder.Entity<Post>()
                .HasOne(p => p.Category)
                .WithMany()
                .HasForeignKey(p => p.CategoryId);

            // Prevent Cascade Delete for Comment → Post
            builder.Entity<Comment>()
                .HasOne(c => c.Post)
                .WithMany(p => p.Comments)
                .HasForeignKey(c => c.PostId)
                .OnDelete(DeleteBehavior.Restrict); // Use Restrict or NoAction

            // Prevent Cascade Delete for Comment → Crafter (ApplicationUser)
            builder.Entity<Comment>()
                .HasOne(c => c.Crafter)
                .WithMany()
                .HasForeignKey(c => c.CrafterId)
                .OnDelete(DeleteBehavior.Restrict);

            // Client & Address Relationship
            builder.Entity<Address>()
                .HasOne(a => a.Client)
                .WithMany(u => u.Addresses)
                .HasForeignKey(a => a.ClientId);

            // Prevent Cascade Delete for Payment → Client (ApplicationUser)
            builder.Entity<Payment>()
                .HasOne(p => p.Client)
                .WithMany()
                .HasForeignKey(p => p.ClientId)
                .OnDelete(DeleteBehavior.Restrict); // Or DeleteBehavior.NoAction

            // Prevent Cascade Delete for Payment → Crafter (ApplicationUser)
            builder.Entity<Payment>()
                .HasOne(p => p.Crafter)
                .WithMany()
                .HasForeignKey(p => p.CrafterId)
                .OnDelete(DeleteBehavior.Restrict);

            // Prevent Cascade Delete for Payment → Service
            builder.Entity<Payment>()
                .HasOne(p => p.Service)
                .WithMany()
                .HasForeignKey(p => p.ServiceId)
                .OnDelete(DeleteBehavior.Restrict);

            // Explicitly define foreign key for Review → Service
            builder.Entity<Review>()
                .HasOne(r => r.Service)
                .WithMany(s => s.Reviews)  // Ensure Service has a Reviews collection
                .HasForeignKey(r => r.ServiceId) // Explicit FK mapping
                .OnDelete(DeleteBehavior.Restrict); // Prevent cascading issues

            // Explicitly define foreign key for Review → Client
            builder.Entity<Review>()
                .HasOne(r => r.Client)
                .WithMany()
                .HasForeignKey(r => r.ClientId)
                .OnDelete(DeleteBehavior.Restrict);

            // Set precision for Payment.Amount
            builder.Entity<Payment>()
                .Property(p => p.Amount)
                .HasPrecision(18, 4); // Adjust precision as needed

            // Set precision for Service.Price
            builder.Entity<Service>()
                .Property(s => s.Price)
                .HasPrecision(18, 4); // Adjust precision as needed

            // Relationship: UserPayment → User (ApplicationUser)
            builder.Entity<UserPayment>()
                .HasOne<ApplicationUser>()
                .WithMany()  // If you want to add navigation property, use .WithMany(u => u.Payments)
                .HasForeignKey(up => up.UserId)
                .OnDelete(DeleteBehavior.Restrict); // Prevent cascading delete

            // Relationship: UserPayment → Service
            builder.Entity<UserPayment>()
                .HasOne<Service>()
                .WithMany()  // If Service has a List<UserPayment> navigation property, use .WithMany(s => s.UserPayments)
                .HasForeignKey(up => up.ServiceId)
                .OnDelete(DeleteBehavior.Restrict); // Prevent cascading delete

            // Set precision for Payment.Amount
            builder.Entity<UserPayment>()
                .Property(p => p.Amount)
                .HasPrecision(18, 4); // Adjust precision as needed
        }

    }

}
