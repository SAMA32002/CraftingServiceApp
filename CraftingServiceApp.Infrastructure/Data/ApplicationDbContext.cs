using CraftingServiceApp.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

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
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Request> Requests { get; set; }
        public DbSet<RequestSchedule> requestSchedules { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<SliderItem> SliderItems { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // User Relationships
            builder.Entity<ApplicationUser>()
                .HasMany(u => u.Services)
                .WithOne(s => s.Crafter)
                .HasForeignKey(s => s.CrafterId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<ApplicationUser>()
                .HasMany(u => u.Posts)
                .WithOne(p => p.Client)
                .HasForeignKey(p => p.ClientId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<ApplicationUser>()
                .HasMany(u => u.PaymentsAsClient)
                .WithOne(p => p.Client)
                .HasForeignKey(p => p.ClientId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<ApplicationUser>()
                .HasMany(u => u.PaymentsAsCrafter)
                .WithOne(p => p.Crafter)
                .HasForeignKey(p => p.CrafterId)
                .OnDelete(DeleteBehavior.Restrict);

            // Identity Relationship
            builder.Entity<IdentityUserRole<string>>()
                .HasOne<IdentityRole>()
                .WithMany()
                .HasForeignKey(ur => ur.RoleId)
                .OnDelete(DeleteBehavior.Restrict);

            // Service Relationships
            builder.Entity<Service>()
                .HasOne(s => s.Category)
                .WithMany(c => c.Services)
                .HasForeignKey(s => s.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Service>()
                .Property(s => s.Price)
                .HasPrecision(18, 4);

            // Post Relationship
            builder.Entity<Post>()
                .HasOne(p => p.Category)
                .WithMany()
                .HasForeignKey(p => p.CategoryId);

            // Comment Relationships
            builder.Entity<Comment>()
                .HasOne(c => c.Post)
                .WithMany(p => p.Comments)
                .HasForeignKey(c => c.PostId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Comment>()
                .HasOne(c => c.Crafter)
                .WithMany()
                .HasForeignKey(c => c.CrafterId)
                .OnDelete(DeleteBehavior.Restrict);

            // Address Relationships
            builder.Entity<Address>()
                .HasOne(a => a.Client)
                .WithMany(u => u.Addresses)
                .HasForeignKey(a => a.ClientId)
                .OnDelete(DeleteBehavior.Cascade);

            // Review Relationships
            builder.Entity<Review>()
                .HasOne(r => r.Service)
                .WithMany(s => s.Reviews)
                .HasForeignKey(r => r.ServiceId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Review>()
                .HasOne(r => r.Client)
                .WithMany()
                .HasForeignKey(r => r.ClientId)
                .OnDelete(DeleteBehavior.Restrict);

            // Payment Configuration
            builder.Entity<Payment>()
                .Property(p => p.Amount)
                .HasPrecision(18, 2);

            builder.Entity<Payment>()
                .Property(p => p.Status)
                .HasConversion<string>();

            builder.Entity<Payment>()
                .HasOne(p => p.Request)
                .WithOne(r => r.Payment)
                .HasForeignKey<Payment>(p => p.RequestId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Payment>()
                .HasIndex(p => p.StripePaymentIntentId)
                .IsUnique();

            builder.Entity<Payment>()
                .HasIndex(p => p.Status);

            // Request Relationships
            builder.Entity<Request>()
                .HasOne(r => r.Client)
                .WithMany(u => u.SentRequests)
                .HasForeignKey(r => r.ClientId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Request>()
                .HasOne(r => r.Service)
                .WithMany(s => s.Requests)
                .HasForeignKey(r => r.ServiceId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Request>()
                .HasOne(r => r.SelectedSchedule)
                .WithOne()
                .HasForeignKey<Request>(r => r.SelectedScheduleId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<Request>()
                .HasOne(r => r.SelectedAddress)
                .WithMany()
                .HasForeignKey(r => r.SelectedAddressId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Request>()
                .Property(r => r.Status)
                .HasConversion<string>();

            builder.Entity<Request>()
                .HasMany(r => r.ProposedDates)
                .WithOne(rs => rs.Request)
                .HasForeignKey(rs => rs.RequestId)
                .OnDelete(DeleteBehavior.Cascade);

            // Notification Relationship
            builder.Entity<Notification>()
                .HasOne(n => n.User)
                .WithMany(u => u.Notifications)
                .HasForeignKey(n => n.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
