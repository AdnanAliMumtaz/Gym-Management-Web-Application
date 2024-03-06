using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebApplication1.Data;
using WebApplication1.Models;
/*using System.Linq;*/

namespace WebApplication1.Controllers
{
    public class ConnectionRequestsController : Controller
    {
        private readonly WebDbContext _context;

        public ConnectionRequestsController(WebDbContext context)
        {
            _context = context;
        }

        public IActionResult SendRequest(string ownerId)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var existingRequest = _context.ConnectionRequests
                .FirstOrDefault(r => r.SenderId == currentUserId && r.ReceiverId == ownerId);

            if (existingRequest != null)
            {
                // Request already exists, handle accordingly (maybe show an error message)
                return RedirectToAction("Index", "EmployeeDashboard");
            }

            var request = new ConnectionRequest
            {
                SenderId = currentUserId,
                ReceiverId = ownerId,
                Status = "Pending"
            };

            _context.ConnectionRequests.Add(request);
            _context.SaveChanges();

            return RedirectToAction("Index", "EmployeeDashboard");
        }

        /*public IActionResult WithdrawRequest(int requestId)
        {
            var request = _context.ConnectionRequests.FirstOrDefault(r => r.Id == requestId);

            if (request == null)
            {
                // Request not found, handle accordingly (maybe show an error message)
                return RedirectToAction("Index", "EmployeeDashboard");
            }

            _context.ConnectionRequests.Remove(request);
            _context.SaveChanges();

            return RedirectToAction("Index", "EmployeeDashboard");
        }*/

        public IActionResult WithdrawRequest(string ownerId)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var request = _context.ConnectionRequests.FirstOrDefault(r => r.SenderId == currentUserId && r.ReceiverId == ownerId);

            if (request == null)
            {
                // Request not found, handle accordingly (maybe show an error message)
                return RedirectToAction("Index", "EmployeeDashboard");
            }

            _context.ConnectionRequests.Remove(request);
            _context.SaveChanges();

            return RedirectToAction("Index", "EmployeeDashboard");
        }


        public IActionResult AcceptRequest(int requestId)
        {
            var request = _context.ConnectionRequests.FirstOrDefault(r => r.Id == requestId);

            if (request == null)
            {
                // Request not found, handle accordingly (maybe show an error message)
                return RedirectToAction("Index", "OwnerDashboard");
            }

            request.Status = "Accepted";
            _context.SaveChanges();

            return RedirectToAction("Index", "OwnerDashboard");
        }

        public IActionResult RejectRequest(int requestId)
        {
            var request = _context.ConnectionRequests.FirstOrDefault(r => r.Id == requestId);

            if (request == null)
            {
                // Request not found, handle accordingly (maybe show an error message)
                return RedirectToAction("Index", "OwnerDashboard");
            }

            request.Status = "Rejected";
            _context.SaveChanges();

            return RedirectToAction("Index", "OwnerDashboard");
        }

        /*public IActionResult Index()
        {
            return View();
        }*/
    }
}
