using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Security.Policy;
using WebApplication1.Areas.Identity.Data;
using WebApplication1.Data;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    [Authorize(Roles = "Employee")]
    public class EmployeeDashboard : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        /*private readonly WebDbContext _context;*/
        public EmployeeDashboard(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
            /*_context = context;*/
        }

        public IActionResult Index()
        {
            var Owners = _userManager.GetUsersInRoleAsync("Owner").Result;
            return View(Owners);
        }

        /*[HttpPost]
        public IActionResult SendRequest(string ownerId)
        {
            var request = new ConnectionRequest
            {
                SenderId = User.FindFirstValue(ClaimTypes.NameIdentifier),
                ReceiverId = ownerId,
                Status = "Pending"
            };

            _context.ConnectionRequests.Add(request);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }*/
    }



    /* private readonly UserManager<ApplicationUser> _userManager;

     public OwnersController(UserManager<ApplicationUser> userManager)
     {
         _userManager = userManager;
     }

     public IActionResult Index()
     {
         var owners = _userManager.GetUsersInRoleAsync("Owner").Result;
         return View(owners);
     }*/


}
