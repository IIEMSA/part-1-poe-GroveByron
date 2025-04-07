using Microsoft.EntityFrameworkCore;

namespace EventEaseVenueBookingApp.Models
{
    public class VenueBookingDBContext : DbContext
    {

        public VenueBookingDBContext(DbContextOptions<VenueBookingDBContext> options) : base(options) 
        { 
        
        }
        
        public DbSet<Venue> Venue { get; set; }
        public DbSet<Event> Event { get; set; }
        public DbSet<Booking> Booking { get; set; }

    }
}
