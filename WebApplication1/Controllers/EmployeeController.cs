using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Owin.BuilderProperties;
using WebApplication1.Areas.Identity.Data;
using WebApplication1.Data;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly WebDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;


        public EmployeeController(WebDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Employees
        /*public async Task<IActionResult> Index()
        {
            var webDbContext = _context.Employees.Include(e => e.ApplicationUser);
            return View(await webDbContext.ToListAsync());
        }
*/
        // GET: Employees
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
            var employees = _context.Employees
                .Include(e => e.ApplicationUser)
                .Where(e => e.UserId == currentUser.Id)
                .ToList();

            return View(employees);
        }

        // GET: Employees/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Employees == null)
            {
                return NotFound();
            }

            var employee = await _context.Employees
                .Include(e => e.ApplicationUser)
                .FirstOrDefaultAsync(m => m.EmployeeID == id);
            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }

        // GET: Employees/Create
        public IActionResult Create()
        {
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        //        public async Task<IActionResult> Edit(int id, [Bind("EmployeeID,FirstName,LastName,DateOfBirth,Gender,Address,PhoneNumber,Email,UserId")] Employee employee)

        public async Task<IActionResult> Create(String FirstName, String LastName, DateTime DateOfBirth, Gender Gender, String Address, String PhoneNumber, String Email)
        {

            ApplicationUser user = await _userManager.GetUserAsync(User);

            // Create a new resource instance
            var newEmployees = new Employee
            {
                FirstName = FirstName,
                LastName = LastName,
                DateOfBirth = DateOfBirth,
                Gender = Gender,
                Address = Address,
                PhoneNumber = PhoneNumber,
                Email = Email,
                ApplicationUser = user
            };

            // Add the new resource to the DbSet and save changes
            _context.Employees.Add(newEmployees);

            await _context.SaveChangesAsync();

            return View(newEmployees);
            // return RedirectToAction(nameof(Index)); // Assuming you have an Index action to display resources
        }

        // GET: Employees/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var item = await _context.Employees
                .FirstOrDefaultAsync(m => m.EmployeeID == id);

            if (item == null)
            {
                return NotFound();
            }

            return View(item);
        }

        // POST: Employees/Edit/5
        /*        public async Task<IActionResult> Edit(int id, [Bind("EmployeeID,FirstName,LastName,DateOfBirth,Gender,Address,PhoneNumber,Email,UserId")] Employee employee)
        */
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public IActionResult EditConfirmed(Employee editedEmployee)
        {
            /*if (ModelState.IsValid)
            {
            }*/

            // Check if the member exists in the database
            var originalEmployee = _context.Employees.Find(editedEmployee.EmployeeID);

            if (originalEmployee == null)
            {
                // If the member is not found, return a 404 Not Found result
                return NotFound();
            }

            // Update the existing member's properties with the edited values
            originalEmployee.FirstName = editedEmployee.FirstName;
            originalEmployee.LastName = editedEmployee.LastName;
            originalEmployee.DateOfBirth = editedEmployee.DateOfBirth;
            originalEmployee.Gender = editedEmployee.Gender;
            originalEmployee.Address = editedEmployee.Address;
            originalEmployee.PhoneNumber = editedEmployee.PhoneNumber;
            originalEmployee.Email = editedEmployee.Email;

            // Update the existing member's properties with the edited values
            /*_context.Update(updatedMember);*/

            _context.Entry(originalEmployee).State = EntityState.Modified;


            // Save changes to the database
            _context.SaveChanges();

            // If the model state is not valid, return to the edit view with validation errors
            return RedirectToAction(nameof(Index));
        }

        // GET: Employees/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Employees == null)
            {
                return NotFound();
            }

            var employee = await _context.Employees
                .Include(e => e.ApplicationUser)
                .FirstOrDefaultAsync(m => m.EmployeeID == id);
            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }

        // POST: Employees/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Employees == null)
            {
                return Problem("Entity set 'WebDbContext.Employees'  is null.");
            }
            var employee = await _context.Employees.FindAsync(id);
            if (employee != null)
            {
                _context.Employees.Remove(employee);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EmployeeExists(int id)
        {
            return (_context.Employees?.Any(e => e.EmployeeID == id)).GetValueOrDefault();
        }
    }
}