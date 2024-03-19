/*using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Areas.Identity.Data;
using WebApplication1.Data;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{

    public class ResourceController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;

        private readonly WebDbContext _context;

        public ResourceController(WebDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }


        public IActionResult Index()
        {
            // Retrieve all resources from the database
            var resources = _context.Resources.Include(r => r.ApplicationUser).ToList();

            // Pass the resources to the view for display
            return View(resources);
        }


        public async Task<IActionResult> AddResourceAsync(String itemName, String itemType, int itemQuantity, DateTime DatePurchased, int itemPrice, String itemNotes)
        {

            ApplicationUser user = await _userManager.GetUserAsync(User);

            // Create a new resource instance
            var newResource = new Resource
            {
                ItemName = itemName,
                ItemType = itemType,
                ItemQuantity = itemQuantity,
                PurchasedDate = DatePurchased,
                ItemPrice = itemPrice,
                ItemNotes = itemNotes,
                ApplicationUser = user
            };

            // Add the new resource to the DbSet and save changes
            _context.Resources.Add(newResource);
            *//*_context.SaveChanges();*//*

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index)); // Assuming you have an Index action to display resources
        }


        [HttpPost]
        *//*public IActionResult EditResource(int resourceId, string itemName, string itemType, int itemQuality, DateTime purchasedDate, int itemPrice, string itemNotes)
        {
            // Retrieve the resource from the database by its ID
            var resource = _context.Resources.Find(resourceId);
            if (resource == null)
            {
                return NotFound(); // Resource not found
            }

            // Update resource details
            resource.ItemName = itemName;
            resource.ItemType = itemType;
            resource.ItemQuantity = itemQuality;
            resource.PurchasedDate = purchasedDate;
            resource.ItemPrice = itemPrice;
            resource.ItemNotes = itemNotes;

            // Save changes to the database
            _context.SaveChanges();

            return RedirectToAction(nameof(Index)); // Assuming you have an Index action to display resources
        }*/

        /*public IActionResult Edit(int resourceId)
        {
            // Find the resource by ID
            var resourceToEdit = _context.Resources.FirstOrDefault(r => r.ResourceID == resourceId);

            if (resourceToEdit == null)
            {
                // Resource not found, handle accordingly (e.g., show error message)
                return RedirectToAction(nameof(Index));
            }

            // Pass the resource to the view for editing
            return View(resourceToEdit);
        }*/


        /*public async Task<IActionResult> Edit(int? id)
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
        }*//*

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var item = await _context.Resources
                .SingleOrDefaultAsync(m => m.ResourceID == id);

            if (item == null)
            {
                return NotFound();
            }

            *//*return View(item);*//*

            return RedirectToAction(nameof(Index));
        }




        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public IActionResult EditConfirmed(Resource editedResource)
        {
            *//*if (ModelState.IsValid)
            {
            }*//*

            // Check if the member exists in the database
            var originalResource = _context.Resources.Find(editedResource.ResourceID);

            if (originalResource == null)
            {
                // If the member is not found, return a 404 Not Found result
                return NotFound();
            }

            // Update the original resource with edited values
            originalResource.ItemName = editedResource.ItemName;
            originalResource.ItemType = editedResource.ItemType;
            originalResource.ItemQuantity = editedResource.ItemQuantity;
            originalResource.PurchasedDate = editedResource.PurchasedDate;
            originalResource.ItemPrice = editedResource.ItemPrice;
            originalResource.ItemNotes = editedResource.ItemNotes;

            // Update the existing member's properties with the edited values
            *//*_context.Update(updatedMember);*//*

            _context.Entry(originalResource).State = EntityState.Modified;


            // Save changes to the database
            _context.SaveChanges();

            // Redirect to the member details page or another appropriate page
            // return RedirectToAction("Edit", new { id = existingMember.MemberID });

            // If the model state is not valid, return to the edit view with validation errors
            return RedirectToAction(nameof(Index));
        }









        *//*[HttpPost]
        public IActionResult EditResource(Resource editedResource)
        {
            // Find the original resource by ID
            var originalResource = _context.Resources.FirstOrDefault(r => r.ResourceID == editedResource.ResourceID);

            if (originalResource == null)
            {
                // Resource not found, handle accordingly (e.g., show error message)
                return RedirectToAction(nameof(Index));
            }

            // Update the original resource with edited values
            originalResource.ItemName = editedResource.ItemName;
            originalResource.ItemType = editedResource.ItemType;
            originalResource.ItemQuantity = editedResource.ItemQuantity;
            originalResource.PurchasedDate = editedResource.PurchasedDate;
            originalResource.ItemPrice = editedResource.ItemPrice;
            originalResource.ItemNotes = editedResource.ItemNotes;

            // Save changes to the database
            _context.SaveChanges();

            // Redirect back to the resource index page
            return RedirectToAction(nameof(Index));
        }

*/
        /* [HttpPost]
         public IActionResult Delete(int id)
         {
             // Retrieve the resource from the database by its ID
             var resource = _context.Resources.Find(id);
             if (resource == null)
             {
                 return NotFound(); // Resource not found
             }

             // Remove the resource from the DbSet and save changes
             _context.Resources.Remove(resource);
             _context.SaveChanges();

             return RedirectToAction(nameof(Index)); // Assuming you have an Index action to display resources
         }*//*

        [HttpPost]
        public IActionResult DeleteResource(int resourceId)
        {
            var resourceToDelete = _context.Resources.Find(resourceId);
            if (resourceToDelete != null)
            {
                _context.Resources.Remove(resourceToDelete);
                _context.SaveChanges();
            }

            return RedirectToAction(nameof(Index)); // Assuming you have an Index action to display resources
        }


    }
}
*/