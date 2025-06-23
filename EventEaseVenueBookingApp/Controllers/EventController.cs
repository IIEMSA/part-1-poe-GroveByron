using EventEaseVenueBookingApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace EventEaseVenueBookingApp.Controllers
{
    public class EventController : Controller
    {
        private readonly VenueBookingDBContext _context;

        public EventController(VenueBookingDBContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string? searchType, int? venueID, DateTime? startDate, DateTime? endDate)
        {
            var query = _context.Event
                .Include(e => e.Venue)
                .Include(e => e.EventType)
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchType))
            {
                query = query.Where(e => e.EventType != null && e.EventType.Name == searchType);
            }

            if (venueID.HasValue)
            {
                query = query.Where(e => e.VenueID == venueID.Value);
            }

            if (startDate.HasValue)
            {
                var startDateOnly = DateOnly.FromDateTime(startDate.Value);
                query = query.Where(e => e.EventDate >= startDateOnly);
            }

            if (endDate.HasValue)
            {
                var endDateOnly = DateOnly.FromDateTime(endDate.Value);
                query = query.Where(e => e.EventEndDate <= endDateOnly);
            }

            var events = await query.ToListAsync();

            // Pass dropdown data and selected filter values to ViewBag for the view
            ViewBag.EventTypes = _context.EventType.Select(et => et.Name).ToList();
            ViewBag.Venues = new SelectList(_context.Venue, "VenueID", "VenueName");

            ViewBag.SelectedSearchType = searchType ?? "";
            ViewBag.SelectedVenueID = venueID?.ToString() ?? "";
            ViewBag.SelectedStartDate = startDate?.ToString("yyyy-MM-dd") ?? "";
            ViewBag.SelectedEndDate = endDate?.ToString("yyyy-MM-dd") ?? "";

            return View(events);
        }

        // GET: Event/Create
        public async Task<IActionResult> Create()
        {
            ViewBag.Venues = new SelectList(await _context.Venue.ToListAsync(), "VenueID", "VenueName");
            ViewBag.EventTypes = new SelectList(await _context.EventType.ToListAsync(), "EventTypeID", "Name");
            return View();
        }

        // POST: Event/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Event events)
        {
            if (ModelState.IsValid)
            {
                _context.Add(events);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Event was created successfully.";
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Venues = new SelectList(await _context.Venue.ToListAsync(), "VenueID", "VenueName", events.VenueID);
            ViewBag.EventTypes = new SelectList(await _context.EventType.ToListAsync(), "EventTypeID", "Name", events.EventTypeID);
            return View(events);
        }

        // GET: Event/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var events = await _context.Event
                .Include(e => e.Venue)
                .Include(e => e.EventType)
                .FirstOrDefaultAsync(m => m.EventID == id);

            if (events == null) return NotFound();

            return View(events);
        }

        // GET: Event/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var events = await _context.Event
                .Include(e => e.Venue)
                .Include(e => e.EventType)
                .Include(e => e.Booking)
                .FirstOrDefaultAsync(m => m.EventID == id);

            if (events == null) return NotFound();

            return View(events);
        }

        // POST: Event/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var events = await _context.Event
                .Include(e => e.Booking)
                .FirstOrDefaultAsync(e => e.EventID == id);

            if (events == null) return NotFound();

            if (events.Booking.Any())
            {
                ModelState.AddModelError(string.Empty, "You can't delete this event as it already has bookings.");
                return View(events);
            }

            _context.Event.Remove(events);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Event was deleted successfully.";
            return RedirectToAction(nameof(Index));
        }

        // GET: Event/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var events = await _context.Event.FindAsync(id);
            if (events == null) return NotFound();

            ViewBag.Venues = new SelectList(await _context.Venue.ToListAsync(), "VenueID", "VenueName", events.VenueID);
            ViewBag.EventTypes = new SelectList(await _context.EventType.ToListAsync(), "EventTypeID", "Name", events.EventTypeID);

            return View(events);
        }

        // POST: Event/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
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

            ViewBag.Venues = new SelectList(await _context.Venue.ToListAsync(), "VenueID", "VenueName", events.VenueID);
            ViewBag.EventTypes = new SelectList(await _context.EventType.ToListAsync(), "EventTypeID", "Name", events.EventTypeID);

            return View(events);
        }

        private bool EventExists(int id)
        {
            return _context.Event.Any(e => e.EventID == id);
        }
    }
}