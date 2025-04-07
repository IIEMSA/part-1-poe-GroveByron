namespace EventEaseVenueBookingApp.Models
{
    public class Event
    {
        public int EventID { get; set; }
        public string EventName { get; set; }
        public DateOnly EventDate { get; set; }
        public DateOnly EventEndDate { get; set; }
        public string EventDescription { get; set; } = "";
        public int VenueID { get; set; }  
        public Venue? Venue { get; set; } 
        public List<Booking> Booking { get; set; } = new();
    }
}
