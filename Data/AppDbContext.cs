using Gift_of_the_givers.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace Gift_of_the_givers.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Disaster> Disasters { get; set; }
        public DbSet<Volunteer> Volunteers { get; set; }
        public DbSet<Donation> Donations { get; set; }
        public DbSet<VolunteerAssignment> VolunteerAssignments { get; set; }
        public DbSet<ResourceAllocation> ResourceAllocations { get; set; }
        public DbSet<ResourceRequest> ResourceRequests { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // User - Volunteer (One-to-One)
            modelBuilder.Entity<Volunteer>()
                .HasOne(v => v.User)
                .WithMany(u => u.Volunteers)
                .HasForeignKey(v => v.UserID);

            // Volunteer - Assignment (One-to-Many)
            modelBuilder.Entity<VolunteerAssignment>()
                .HasOne(va => va.Volunteer)
                .WithMany(v => v.Assignments)
                .HasForeignKey(va => va.VolunteerID);

            // Disaster - Assignment (One-to-Many)
            modelBuilder.Entity<VolunteerAssignment>()
                .HasOne(va => va.Disaster)
                .WithMany(d => d.VolunteerAssignments)
                .HasForeignKey(va => va.DisasterID);

            // Disaster - Donation (One-to-Many)
            modelBuilder.Entity<Donation>()
                .HasOne(d => d.Disaster)
                .WithMany(d => d.Donations)
                .HasForeignKey(d => d.DisasterID);

            // Disaster - ResourceAllocation (One-to-Many)
            modelBuilder.Entity<ResourceAllocation>()
                .HasOne(ra => ra.Disaster)
                .WithMany(d => d.ResourceAllocations)
                .HasForeignKey(ra => ra.DisasterID);

            // Disaster - ResourceRequest (One-to-Many)
            modelBuilder.Entity<ResourceRequest>()
                .HasOne(rr => rr.Disaster)
                .WithMany(d => d.ResourceRequests)
                .HasForeignKey(rr => rr.DisasterID);

            base.OnModelCreating(modelBuilder);
        }
    }
}