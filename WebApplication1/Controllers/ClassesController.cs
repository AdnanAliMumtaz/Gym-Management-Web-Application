using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR.Hubs;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Areas.Identity.Data;
using WebApplication1.Data;
using WebApplication1.Models;
using WebApplication1.Models.ViewModels;

namespace WebApplication1.Controllers
{
    public class ClassesController : Controller
    {
        private readonly WebDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        public ClassesController(WebDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager; 
        }

        // GET: Classes
        public async Task<IActionResult> Index()
        {
            // Get the currently logged-in user
            var currentUser = await _userManager.GetUserAsync(User);

            if (currentUser == null)
            {
                // Redirect to login or handle the case when the user is not logged in
                return RedirectToAction("Login", "Account");
            }

            // Retrieve only the employees associated with the current user
            var classes = _context.Classes
                .Include(e => e.ApplicationUser)
                .Where(e => e.UserId == currentUser.Id)
                .ToList();

            return View(classes);
        }

        // GET: Classes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Classes == null)
            {
                return NotFound();
            }

            var classes = await _context.Classes
                .Include(c => c.ApplicationUser)
                .FirstOrDefaultAsync(m => m.ClassID == id);
            if (classes == null)
            {
                return NotFound();
            }

            return View(classes);
        }

        // GET: Classes/Create
        public IActionResult Create()
        {
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id");
            return View();
        }

        // POST: Classes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create([Bind("ClassID,ClassName,Description,Date,Duration,UserId")] Classes classes)
        public async Task<IActionResult> Create(String ClassName, String Description, DateTime Date, TimeSpan Duration)
        {

            ApplicationUser user = await _userManager.GetUserAsync(User);

            // Create a new resource instance
            var newResource = new Classes
            {
                ClassName = ClassName,
                Description = Description,
                Date = Date,
                Duration = Duration,
                ApplicationUser = user
            };

            // Add the new resource to the DbSet and save changes
            _context.Classes.Add(newResource);

            await _context.SaveChangesAsync();

            return View(newResource);
            // return RedirectToAction(nameof(Index)); // Assuming you have an Index action to display resources
        }

        // GET: Classes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Classes == null)
            {
                return NotFound();
            }

            var classes = await _context.Classes.FindAsync(id);
            if (classes == null)
            {
                return NotFound();
            }
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", classes.UserId);
            return View(classes);
        }

        // POST: Classes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        /*[HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ClassID,ClassName,Description,Date,Duration,UserId")] Classes classes)
        {
            if (id != classes.ClassID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(classes);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ClassesExists(classes.ClassID))
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
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", classes.UserId);
            return View(classes);
        }*/

        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public IActionResult EditConfirmed(Classes editedClasses)
        {
            /*if (ModelState.IsValid)
            {
            }*/

            // Check if the member exists in the database
            var originalClasses = _context.Classes.Find(editedClasses.ClassID);

            if (originalClasses == null)
            {
                // If the member is not found, return a 404 Not Found result
                return NotFound();
            }

            // Update the existing member's properties with the edited values
/*            ClassName = ClassName,
                Description = Description,
                Date = Date,
                Duration = Duration,
                ApplicationUser = user*/



            originalClasses.ClassName = editedClasses.ClassName;
            originalClasses.Description = editedClasses.Description;
            originalClasses.Date = editedClasses.Date;
            originalClasses.Duration = editedClasses.Duration;

            // Update the existing member's properties with the edited values
            /*_context.Update(updatedMember);*/

            _context.Entry(originalClasses).State = EntityState.Modified;


            // Save changes to the database
            _context.SaveChanges();

            // If the model state is not valid, return to the edit view with validation errors
            return RedirectToAction(nameof(Index));
        }



        // GET: Classes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Classes == null)
            {
                return NotFound();
            }

            var classes = await _context.Classes
                .Include(c => c.ApplicationUser)
                .FirstOrDefaultAsync(m => m.ClassID == id);
            if (classes == null)
            {
                return NotFound();
            }

            return View(classes);
        }

        // POST: Classes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Classes == null)
            {
                return Problem("Entity set 'WebDbContext.Classes'  is null.");
            }
            var classes = await _context.Classes.FindAsync(id);
            if (classes != null)
            {
                _context.Classes.Remove(classes);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ClassesExists(int id)
        {
          return (_context.Classes?.Any(e => e.ClassID == id)).GetValueOrDefault();
        }
    }
}
