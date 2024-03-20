using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Humanizer.Localisation;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Areas.Identity.Data;
using WebApplication1.Data;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class ResourceController : Controller
    {
        private readonly WebDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ResourceController(WebDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Resources
        public async Task<IActionResult> Index()
        {
            var webDbContext = _context.Resources.Include(r => r.ApplicationUser);
            return View(await webDbContext.ToListAsync());
        }

        // GET: Resources/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Resources == null)
            {
                return NotFound();
            }

            var resource = await _context.Resources
                .Include(r => r.ApplicationUser)
                .FirstOrDefaultAsync(m => m.ResourceID == id);
            if (resource == null)
            {
                return NotFound();
            }

            return View(resource);
        }

        // GET: Resources/Create
        public IActionResult Create()
        {
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(String itemName, String itemType, int itemQuantity, DateTime PurchasedDate, int itemPrice, String itemNotes)
        {

            ApplicationUser user = await _userManager.GetUserAsync(User);

            // Create a new resource instance
            var newResource = new Resource
            {
                ItemName = itemName,
                ItemType = itemType,
                ItemQuantity = itemQuantity,
                PurchasedDate = PurchasedDate,
                ItemPrice = itemPrice,
                ItemNotes = itemNotes,
                ApplicationUser = user
            };

            // Add the new resource to the DbSet and save changes
            _context.Resources.Add(newResource);

            await _context.SaveChangesAsync();

            return View(newResource);
            // return RedirectToAction(nameof(Index)); // Assuming you have an Index action to display resources
        }


        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var item = await _context.Resources
                .FirstOrDefaultAsync(m => m.ResourceID == id);

            if (item == null)
            {
                return NotFound();
            }

            return View(item);
        }

        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public IActionResult EditConfirmed(Resource editedResource)
        {
            /*if (ModelState.IsValid)
            {
            }*/

            // Check if the member exists in the database
            var originalResource = _context.Resources.Find(editedResource.ResourceID);

            if (originalResource == null)
            {
                // If the member is not found, return a 404 Not Found result
                return NotFound();
            }

            // Update the existing member's properties with the edited values
            originalResource.ItemName = editedResource.ItemName;
            originalResource.ItemType = editedResource.ItemType;
            originalResource.ItemQuantity = editedResource.ItemQuantity;
            originalResource.PurchasedDate = editedResource.PurchasedDate;
            originalResource.ItemPrice = editedResource.ItemPrice;
            originalResource.ItemNotes = editedResource.ItemNotes;

            // Update the existing member's properties with the edited values
            /*_context.Update(updatedMember);*/

            _context.Entry(originalResource).State = EntityState.Modified;


            // Save changes to the database
            _context.SaveChanges();

            // If the model state is not valid, return to the edit view with validation errors
            return RedirectToAction(nameof(Index));
        }


        // GET: Resources/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Resources == null)
            {
                return NotFound();
            }

            var resource = await _context.Resources
                .Include(r => r.ApplicationUser)
                .FirstOrDefaultAsync(m => m.ResourceID == id);
            if (resource == null)
            {
                return NotFound();
            }

            return View(resource);
        }

        // POST: Resources/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Resources == null)
            {
                return Problem("Entity set 'WebDbContext.Resources'  is null.");
            }
            var resource = await _context.Resources.FindAsync(id);
            if (resource != null)
            {
                _context.Resources.Remove(resource);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ResourceExists(int id)
        {
          return (_context.Resources?.Any(e => e.ResourceID == id)).GetValueOrDefault();
        }
    }
}
