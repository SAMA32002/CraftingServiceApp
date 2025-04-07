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

            // User has Services (Crafter)
            builder.Entity<ApplicationUser>()
                .HasMany(u => u.Services)
                .WithOne(s => s.Crafter)
                .HasForeignKey(s => s.CrafterId)
                .OnDelete(DeleteBehavior.Restrict);

            // User has Posts (Client)
            builder.Entity<ApplicationUser>()
                .HasMany(u => u.Posts)
                .WithOne(p => p.Client)
                .HasForeignKey(p => p.ClientId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<IdentityUserRole<string>>()
                .HasOne<IdentityRole>()
                .WithMany()
                .HasForeignKey(ur => ur.RoleId)
                .OnDelete(DeleteBehavior.Restrict);

            // Service & Category Relationship
            builder.Entity<Service>()
                .HasOne(s => s.Category)
                .WithMany(c => c.Services)
                .HasForeignKey(s => s.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            // Set precision for Service.Price
            builder.Entity<Service>()
                .Property(s => s.Price)
                .HasPrecision(18, 4);

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
                .OnDelete(DeleteBehavior.Restrict);

            // Prevent Cascade Delete for Comment → Crafter
            builder.Entity<Comment>()
                .HasOne(c => c.Crafter)
                .WithMany()
                .HasForeignKey(c => c.CrafterId)
                .OnDelete(DeleteBehavior.Restrict);

            // Client & Address Relationship
            builder.Entity<Address>()
                .HasOne(a => a.Client)
                .WithMany(u => u.Addresses)
                .HasForeignKey(a => a.ClientId)
                .OnDelete(DeleteBehavior.Restrict);

            // Explicitly define foreign key for Review → Service
            builder.Entity<Review>()
                .HasOne(r => r.Service)
                .WithMany(s => s.Reviews)
                .HasForeignKey(r => r.ServiceId)
                .OnDelete(DeleteBehavior.Restrict);

            // Explicitly define foreign key for Review → Client
            builder.Entity<Review>()
                .HasOne(r => r.Client)
                .WithMany()
                .HasForeignKey(r => r.ClientId)
                .OnDelete(DeleteBehavior.Restrict);

            // Set precision for Payment.Amount
            builder.Entity<Payment>()
                .Property(p => p.Amount)
                .HasPrecision(18, 4);

            // Relationship: Payment → Request (Instead of Service)
            builder.Entity<Payment>()
                .HasOne<Request>()
                .WithMany()
                .HasForeignKey(up => up.RequestId)
                .OnDelete(DeleteBehavior.Restrict);

            // Relationship: Request → Client
            builder.Entity<Request>()
                .HasOne(r => r.Client)
                .WithMany(u => u.SentRequests)
                .HasForeignKey(r => r.ClientId)
                .OnDelete(DeleteBehavior.Restrict);

            // Relationship: Request → Service
            builder.Entity<Request>()
                .HasOne(r => r.Service)
                .WithMany(s => s.Requests)
                .HasForeignKey(r => r.ServiceId)
                .OnDelete(DeleteBehavior.Restrict);

            // Relationship: Request → RequestSchedule (One-to-One)
            builder.Entity<Request>()
                .HasOne(r => r.SelectedSchedule)
                .WithOne()
                .HasForeignKey<Request>(r => r.SelectedScheduleId)
                .OnDelete(DeleteBehavior.NoAction); // Prevent cascading issues

            builder.Entity<Request>()
                .HasOne(r => r.Payment)
                .WithOne(p => p.Request) // Explicitly set navigation
                .HasForeignKey<Request>(r => r.PaymentId)
                .OnDelete(DeleteBehavior.SetNull); // Keep request if payment is deleted

            builder.Entity<Request>()
                .HasOne(r => r.SelectedAddress)
                .WithMany()
                .HasForeignKey(r => r.SelectedAddressId)
                .OnDelete(DeleteBehavior.Restrict); // Prevent cascading delete

            builder.Entity<Address>()
                .HasOne(a => a.Client)
                .WithMany(u => u.Addresses)
                .HasForeignKey(a => a.ClientId)
                .OnDelete(DeleteBehavior.Cascade); // Delete addresses if client is removed

            builder.Entity<Request>()
                .Property(r => r.Status)
                .HasConversion<string>(); // Stores enum as string

            builder.Entity<Request>()
                .Property(r => r.PaymentStatus)
                .HasConversion<string>(); // Stores enum as string

            builder.Entity<RequestSchedule>()
                .HasOne(rs => rs.Request)
                .WithMany(r => r.ProposedDates)
                .HasForeignKey(rs => rs.RequestId)
                .OnDelete(DeleteBehavior.Cascade); // Automatically remove schedules when the request is deleted

            builder.Entity<Notification>()
                .HasOne(n => n.User)
                .WithMany(u => u.Notifications)
                .HasForeignKey(n => n.UserId)
                .OnDelete(DeleteBehavior.Cascade);

        }
    }
}
