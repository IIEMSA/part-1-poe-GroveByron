using EventEaseVenueBookingApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace EventEaseVenueBookingApp.Controllers
{
    public class BookingController : Controller
    {
        private readonly VenueBookingDBContext _context;

        public BookingController(VenueBookingDBContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var booking = await _context.Booking.Include(b => b.Venue).Include(b => b.Event).ToListAsync();
            return View(booking); 
        }

        public IActionResult Create()
        {
            ViewData["Venues"] = new SelectList(_context.Venue, "VenueID", "VenueName");
            ViewData["Events"] = new SelectList(_context.Event, "EventID", "EventName");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Booking booking)
        {
            if (ModelState.IsValid)
            {
                var duplicateBooking = await _context.Booking.AnyAsync(b => b.VenueID == booking.VenueID && b.EventID == booking.EventID &&
                b.BookingDate.Date == booking.BookingDate.Date && b.BookingDate.Hour == booking.BookingDate.Hour && b.BookingDate.Minute == booking.BookingDate.Minute

                );

                if (duplicateBooking)
                {
                    var venueName = await _context.Venue
                        .Where(v => v.VenueID == booking.VenueID)
                        .Select(v => v.VenueName)
                        .FirstOrDefaultAsync();

                    var eventName = await _context.Event
                        .Where(e => e.EventID == booking.EventID)
                        .Select(e => e.EventName)
                        .FirstOrDefaultAsync();

                    ModelState.AddModelError("", $"Sorry! The event '{eventName}' is already booked at venue '{venueName}' for {booking.BookingDate:MMMM dd, yyyy hh:mm tt}.");
                }

                if (!ModelState.IsValid)
                {
                    ViewData["Venues"] = new SelectList(_context.Venue, "VenueID", "VenueName", booking.VenueID);
                    ViewData["Events"] = new SelectList(_context.Event, "EventID", "EventName", booking.EventID);
                    return View(booking);
                }

                _context.Add(booking);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["Venues"] = new SelectList(_context.Venue, "VenueID", "VenueName", booking.VenueID);
            ViewData["Events"] = new SelectList(_context.Event, "EventID", "EventName", booking.EventID);
            return View(booking);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var booking = await _context.Booking
                .Include(b => b.Venue) 
                .Include(b => b.Event)  
                .FirstOrDefaultAsync(m => m.BookingID == id);

            if (booking == null)
            {
                return NotFound();
            }

            return View(booking);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var booking = await _context.Booking
                .Include(b => b.Venue)  
                .Include(b => b.Event)  
                .FirstOrDefaultAsync(m => m.BookingID == id);

            if (booking == null)
            {
                return NotFound();
            }

            return View(booking);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {

            var booking = await _context.Booking.FindAsync(id);
            _context.Booking.Remove(booking);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));

        }

        private bool EventExists(int id)
        {

            return _context.Booking.Any(e => e.BookingID == id);

        }

        public async Task<IActionResult> Edit(int? id)
        {

            if (id == null)
            {

                return NotFound();

            }

            var booking = await _context.Booking.FindAsync(id);

            if (id == null)
            {

                return NotFound();

            }

            ViewBag.VenueID = new SelectList(_context.Venue, "VenueID", "VenueName", booking.VenueID);
            ViewBag.EventID = new SelectList(_context.Event, "EventID", "EventName", booking.EventID);

            return View(booking);

        }

        [HttpPost]

        public async Task<IActionResult> Edit(int id, Booking booking)
        {

            if (id != booking.BookingID)
            {

                return NotFound();

            }

            if (ModelState.IsValid)
            {

                try
                {

                    _context.Update(booking);
                    await _context.SaveChangesAsync();

                }

                catch (DbUpdateConcurrencyException)
                {

                    if (!EventExists(booking.BookingID))
                    {

                        return NotFound();

                    }
                    else
                    {

                        throw;

                    }

                }

                return RedirectToAction(nameof(Index));

            }

            return View(booking);

        }
    }
}
