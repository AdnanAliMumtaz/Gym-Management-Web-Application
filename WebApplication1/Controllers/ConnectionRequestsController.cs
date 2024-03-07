using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebApplication1.Data;
using WebApplication1.Models;
/*using System.Linq;*/

namespace WebApplication1.Controllers
{
 /*   [Authorize(Roles = "Employee")]*/
    [Authorize(Roles = "Employee,Owner")]
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


        /*public IActionResult AcceptRequest(int requestId)
        {
            var request = _context.ConnectionRequests.FirstOrDefault(r => r.Id == requestId);

            if (request == null)
            {
                // Request not found, handle accordingly (maybe show an error message)
                return RedirectToAction("Index", "Employee");
            }

            request.Status = "Accepted";
            _context.SaveChanges();

            return RedirectToAction("Index", "Employee");
        }*/


        public IActionResult AcceptRequest(string receiverId)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var request = _context.ConnectionRequests.FirstOrDefault(r => r.SenderId == receiverId && r.ReceiverId == currentUserId);

            if (request == null)
            {
                // Request not found, handle accordingly (maybe show an error message)
                return RedirectToAction("Index", "Employee");
            }

            request.Status = "Accepted";
            _context.SaveChanges();

            return RedirectToAction("Index", "Employee");
        }


        /*public IActionResult AcceptRequest(int requestId)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var request = _context.ConnectionRequests.FirstOrDefault(r => r.Id == requestId && r.ReceiverId == currentUserId);

            if (request == null)
            {
                // Request not found or current user is not the receiver, handle accordingly
                return RedirectToAction("Index", "Employee");
            }

            request.Status = "Accepted";
            _context.SaveChanges();

            return RedirectToAction("Index", "EmployeeDashboa");
        }*/

        public IActionResult RejectRequest(int requestId)
        {
            var request = _context.ConnectionRequests.FirstOrDefault(r => r.Id == requestId);

            if (request == null)
            {
                // Request not found, handle accordingly (maybe show an error message)
                return RedirectToAction("Index", "Employee");
            }

            request.Status = "Rejected";
            _context.SaveChanges();

            return RedirectToAction("Index", "Employee");
        }
    }
}
