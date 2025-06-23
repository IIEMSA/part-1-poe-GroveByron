using Microsoft.EntityFrameworkCore;
using System;

namespace EventEaseVenueBookingApp.Models
{
    public class VenueBookingDBContext : DbContext
    {
        public VenueBookingDBContext(DbContextOptions<VenueBookingDBContext> options) : base(options) { }

        public DbSet<Venue> Venue { get; set; }
        public DbSet<Event> Event { get; set; }
        public DbSet<Booking> Booking { get; set; }
        public DbSet<EventType> EventType { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure conversion for DateOnly properties
            modelBuilder.Entity<Event>()
                .Property(e => e.EventDate)
                .HasConversion(
                    v => v.ToDateTime(TimeOnly.MinValue),
                    v => DateOnly.FromDateTime(v));

            modelBuilder.Entity<Event>()
                .Property(e => e.EventEndDate)
                .HasConversion(
                    v => v.ToDateTime(TimeOnly.MinValue),
                    v => DateOnly.FromDateTime(v));

            base.OnModelCreating(modelBuilder);
        }
    }
}
