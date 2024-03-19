using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebApplication1.Data;
using WebApplication1.Models;
using WebApplication1.Models.ViewModels;

namespace WebApplication1.Controllers
{
    [Authorize]
    public class MessagesController : Controller
    {
        private readonly WebDbContext _context;

        public MessagesController(WebDbContext context)
        {
            _context = context;
        }

        /*public IActionResult Index()
        {
            return View();
        }*/


        public IActionResult Index(string receiverId)
        {
            /*var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var receivedMessages = _context.Messages.Where(m => m.ReceiverId == currentUserId).ToList();
            return View(receivedMessages);*/



            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var receivedMessages = _context.Messages.Where(m => m.ReceiverId == currentUserId).ToList();
            var sentMessages = _context.Messages.Where(m => m.SenderId == currentUserId).ToList();
            var allMessages = receivedMessages.Concat(sentMessages).OrderByDescending(m => m.Timestamp).ToList();
            return View(allMessages);

        }


        /*public IActionResult SendMessage(string receiverId, string content)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Create a new message
            var message = new Message
            {
                SenderId = currentUserId,
                ReceiverId = receiverId,
                Content = content,
                Timestamp = DateTime.Now
            };

            // Save the message to the database
            _context.Messages.Add(message);
            _context.SaveChanges();

            // Redirect back to the messages page
            return RedirectToAction("ViewMessages");
        }*/




        [HttpPost]
        public IActionResult SendMessage(string receiverId, string content)
        {
            // Get the current user's ID (the sender)
            var senderId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Create a new message
            var message = new Message
            {
                SenderId = senderId,
                ReceiverId = receiverId,
                Content = content,
                Timestamp = DateTime.Now
            };

            // Add the message to the database
            _context.Messages.Add(message);
            _context.SaveChanges();

            // Redirect to the messaging page or any other appropriate action
            return RedirectToAction("Index", "Messages");
        }




        public IActionResult ViewMessages()
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Fetch accepted connections for the current user
            var acceptedConnections = _context.ConnectionRequests
                .Where(r => r.ReceiverId == currentUserId && r.Status == "Accepted")
                .ToList();

            // Create a view model to pass both messages and accepted connections to the view
            var viewModel = new MessageViewModel
            {
                Messages = _context.Messages.ToList(),
                AcceptedConnections = acceptedConnections
            };

            return View(viewModel);
        }






        // Action for viewing a conversation between two users
        public IActionResult ViewConversation(string ownerId, string employeeId)
        {
            // Get the current user's ID
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Retrieve messages between the owner and the employee
            var conversation = _context.Messages
                .Where(m => (m.SenderId == ownerId && m.ReceiverId == employeeId) ||
                            (m.SenderId == employeeId && m.ReceiverId == ownerId))
                .OrderBy(m => m.Timestamp)
                .ToList();

            // Pass the conversation to the view
            return View(conversation);
        }
    }
}
