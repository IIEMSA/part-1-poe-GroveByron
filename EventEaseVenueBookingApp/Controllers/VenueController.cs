using Azure.Storage.Blobs;
using EventEaseVenueBookingApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EventEaseVenueBookingApp.Controllers
{
    public class VenueController : Controller
    {

        private readonly VenueBookingDBContext _context;

        public VenueController(VenueBookingDBContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var venue = await _context.Venue.ToListAsync();
            return View(venue);
        }

        public IActionResult Create()
        {

            return View();

        }

        [HttpPost]
        public async Task<IActionResult> Create(Venue venue)
        {
            if (venue.ImageFile == null || venue.ImageFile.Length == 0)
            {
                ModelState.AddModelError("ImageFile", "Image is required.");
            }

            if (ModelState.IsValid)
            {
                var blobUrl = await UploadImageToBlobAsync(venue.ImageFile);
                venue.ImageURL = blobUrl;

                _context.Add(venue);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Venue was created successfully.";
                return RedirectToAction(nameof(Index));
            }

            return View(venue);
        }



        public async Task<IActionResult> Details(int? id)
        {
            var venue = await _context.Venue.FirstOrDefaultAsync(m => m.VenueID == id);

            if (venue == null)
            {

                return NotFound();

            }

            return View(venue);
        }

        private async Task<string> UploadImageToBlobAsync(IFormFile imageFile)
        {
            


            var blobServiceClient = new BlobServiceClient(connectionString);
            var containerClient = blobServiceClient.GetBlobContainerClient(containerName);
            var blobClient = containerClient.GetBlobClient(Guid.NewGuid() + Path.GetExtension(imageFile.FileName));

            var blobHttpHeaders = new Azure.Storage.Blobs.Models.BlobHttpHeaders
            {
                ContentType = imageFile.ContentType
            };

            using (var stream = imageFile.OpenReadStream())
            {
                await blobClient.UploadAsync(stream, new Azure.Storage.Blobs.Models.BlobUploadOptions
                {
                    HttpHeaders = blobHttpHeaders
                });
            }

            return blobClient.Uri.ToString();
        }

        public async Task<IActionResult> Delete(int? id)
        {

            var venue = await _context.Venue.FirstOrDefaultAsync(m => m.VenueID == id);

            if (venue == null)
            {

                return NotFound();

            }

            return View(venue);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var venue = await _context.Venue
                .Include(v => v.Event)
                .FirstOrDefaultAsync(v => v.VenueID == id);

            if (venue == null)
            {
                return NotFound();
            }

            // Check if the venue has any events linked to it
            if (venue.Event.Any())
            {
                ModelState.AddModelError(string.Empty, "You can't delete this venue as it contains an event that is scheduled.");
                return View(venue); // Return the same view with the error message
            }

            _context.Venue.Remove(venue);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Venue was deleted successfully.";
            return RedirectToAction(nameof(Index));
        }

        private bool VenueExists(int id)
        {

            return _context.Venue.Any(e => e.VenueID == id);

        }

        public async Task<IActionResult> Edit(int? id)
        {

            if (id == null)
            {

                return NotFound();

            }

            var venue = await _context.Venue.FindAsync(id);

            if (id == null)
            {

                return NotFound();

            }

            return View(venue);

        }

        [HttpPost]

        public async Task<IActionResult> Edit(int id, Venue venue)
        {

            if (id != venue.VenueID)
            {

                return NotFound();

            }

            if (ModelState.IsValid)
            {

                try
                {
                    if (venue.ImageFile != null)
                    {

                        var blobUrl = await UploadImageToBlobAsync(venue.ImageFile);

                        venue.ImageURL = blobUrl;

                    }
                    else
                    {


                    }

                    _context.Update(venue);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Venue was updated successfully.";

                }

                catch (DbUpdateConcurrencyException)
                {

                    if (!VenueExists(venue.VenueID))
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

            return View(venue);

        }

    }

}

