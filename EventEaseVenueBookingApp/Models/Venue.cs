namespace EventEaseVenueBookingApp.Models
{
    public class Venue
    {
        public int VenueID { get; set; }
        public string VenueName { get; set; }
        public string VenueLocation { get; set; }
        public int VenueCapacity { get; set; }
        public string ImageURL { get; set; }
        public List<Event> Event { get; set; } = new();  
        public List<Booking> Booking { get; set; } = new();
    }
}
