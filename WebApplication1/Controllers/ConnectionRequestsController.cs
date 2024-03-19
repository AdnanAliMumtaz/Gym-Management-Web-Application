using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebApplication1.Data;
using WebApplication1.Models;
/*using System.Linq;*/

namespace WebApplication1.Controllers
{
    /*[Authorize(Roles = "Employee,Owner")]*/
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

        /*public IActionResult AcceptRequest(string receiverId)
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

            if (request.Status == "Accepted")
            {
                // Request is already accepted, do not update
                return RedirectToAction("Index", "Employee");
            }

            request.Status = "Accepted";
            _context.SaveChanges();

            // Send a message to the employee informing them about the connection
            var message = new Message
            {
                SenderId = currentUserId,
                ReceiverId = receiverId,
                Content = "You are now connected with the owner.",
                Timestamp = DateTime.Now,
            };
            _context.Messages.Add(message);
            _context.SaveChanges();

            return RedirectToAction("Index", "Employee");
        }




        public IActionResult RejectRequest(string receiverId)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var request = _context.ConnectionRequests.FirstOrDefault(r => r.SenderId == receiverId && r.ReceiverId == currentUserId);

            if (request == null)
            {
                // Request not found, handle accordingly (maybe show an error message)
                return RedirectToAction("Index", "Employee");
            }

            _context.ConnectionRequests.Remove(request); // Remove the request from the database
            _context.SaveChanges();

            return RedirectToAction("Index", "Employee");
        }
    }
}
