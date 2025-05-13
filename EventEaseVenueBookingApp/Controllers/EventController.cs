using EventEaseVenueBookingApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace EventEaseVenueBookingApp.Controllers
{
    public class EventController : Controller
    {

        private readonly VenueBookingDBContext _context;

        public EventController(VenueBookingDBContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var events = await _context.Event.Include(e => e.Venue).ToListAsync();
            return View(events);
        }

        public IActionResult Create()
        {
            
            ViewBag.Venues = new SelectList(_context.Venue, "VenueID", "VenueName");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Event events)
        {
            if (ModelState.IsValid)
            {
                _context.Add(events);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Event was created sucessfully.";
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Venues = new SelectList(_context.Venue, "VenueID", "VenueName", events.VenueID);
            return View(events);
        }

        public async Task<IActionResult> Details(int? id)
        {

            var events = await _context.Event.FirstOrDefaultAsync(m => m.EventID == id);

            if (events == null)
            {

                return NotFound();

            }

            return View(events);
        }

        public async Task<IActionResult> Delete(int? id)
        {

            var events = await _context.Event.FirstOrDefaultAsync(m => m.EventID == id);

            if (events == null)
            {

                return NotFound();

            }

            return View(events);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var events = await _context.Event
                .Include(e => e.Booking)
                .FirstOrDefaultAsync(e => e.EventID == id);

            if (events == null)
            {
                return NotFound();
            }

            // Check if the event has any bookings
            if (events.Booking.Any())
            {
                // Return the same view with an error message
                ModelState.AddModelError(string.Empty, "You can't delete this event as it already has a booking.");
                return View(events);
            }

            _context.Event.Remove(events);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Event was deleted successfully.";
            return RedirectToAction(nameof(Index));
        }

        private bool EventExists(int id)
        {

            return _context.Event.Any(e => e.EventID == id);

        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var events = await _context.Event.FindAsync(id);
            if (events == null) return NotFound();

            ViewBag.Venues = new SelectList(_context.Venue, "VenueID", "VenueName", events.VenueID);

            return View(events);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, Event events)
        {
            if (id != events.EventID) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(events);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Event was updated successfully.";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EventExists(events.EventID)) return NotFound();
                    else throw;
                }
            }

            ViewBag.Venues = new SelectList(_context.Venue, "VenueID", "VenueName", events.VenueID);

            return View(events);
        }
    }
}
