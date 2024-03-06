using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebApplication1.Areas.Identity.Data;
using WebApplication1.Data;

namespace WebApplication1.Controllers
{
    [Authorize(Roles = "Owner")]
    public class EmployeeController : Controller
    {
        private readonly WebDbContext _context;

        public EmployeeController(WebDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            // Get the ID of the currently logged-in owner
            var ownerId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Retrieve ConnectionRequests related to the current owner
            var connectionRequests = _context.ConnectionRequests
                .Where(r => r.ReceiverId == ownerId)
                .ToList();

            // Pass the filtered collection to the view
            return View(connectionRequests);
        }
        /*public async Task<IActionResult> EmployeeAsync()
        {
            ApplicationUser user = await _userManager.GetUserAsync(User);

            // Retrieve the list of owners from your data source
            var owners = _userManager.GetRolesAsync(user); // Example method to get all owners from a repository

            return View(owners);
        }*/
    }
}
