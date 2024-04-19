using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web.Helpers;
using MailKit;
using MailKit.Net.Imap;
using MailKit.Search;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Azure.Management.Monitor.Fluent.Models;
using Microsoft.EntityFrameworkCore;
using MimeKit;
using NuGet.Protocol.Plugins;
using WebApplication1.Areas.Identity.Data;
using WebApplication1.Data;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class EmailsController : Controller
    {
        private readonly WebDbContext _context;
        private readonly EmailReceiver _emailReceiver;
        private readonly UserManager<ApplicationUser> _userManager;
        public EmailsController(WebDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _emailReceiver = new EmailReceiver();
            _userManager = userManager;
        }

        public async Task<IActionResult> Index(string emailAddress)
        {
            ApplicationUser user = await _userManager.GetUserAsync(User);

            // Retrieve all members from the database
            var allMembers = await _context.Members.Where(m => m.UserId == user.Id).ToListAsync(); // Replace _context.Members with your actual DbSet for members
            ViewBag.AllMembers = allMembers;

            // Retrieve all employees from the database
            var allEmployees = await _context.Employees.Where(m => m.UserId == user.Id).ToListAsync(); // Replace _context.Employees with your actual DbSet for employees
            ViewBag.AllEmployees = allEmployees;

            // Retrieve all emails from the database
            var allEmails = await _context.Emails.Where(m => m.UserId == user.Id).ToListAsync(); // Replace _context.Emails with your actual DbSet for emails

            // Filter emails based on the entered email address
            /*var filteredEmails = allEmails.Where(e => e.SenderId == emailAddress || e.ReceiverId == emailAddress).ToList();*/
            var filteredEmails = allEmails.Where(e => e.SenderId == emailAddress).ToList();

            ViewBag.FilteredEmails = filteredEmails;

            // Create a new instance of Email if needed
            Email email = new Email(); // Assuming you're creating a new instance of Email

            return View(email);
        }


        public async Task<IActionResult> SentEmails(string receiverEmail)
        {
            var sentEmails = await _context.Emails
                .Where(e => e.ReceiverId == receiverEmail) // Filter sent emails by the receiver's email address
                .ToListAsync();

            ViewBag.SentEmails = sentEmails;

            return View(sentEmails);
        }

        [HttpPost]
        public async Task<IActionResult> ReceivedEmails(string emailAddress)
        {
            // Check if the email address is provided
            if (string.IsNullOrEmpty(emailAddress))
            {
                ModelState.AddModelError(string.Empty, "Email address is required.");
                return RedirectToAction(nameof(Index));
            }

            // Retrieve emails for the specified email address
            var receivedEmails = await _context.Emails
                .Where(e => e.ReceiverId == emailAddress)
                .ToListAsync();

            // Set ViewBag with received emails
            ViewBag.ReceivedEmails = receivedEmails;

            // Redirect to Index action
            return RedirectToAction(nameof(Index));
        }




        [HttpPost]
        public async Task<IActionResult> SendEmail(string ReceiverId, string Subject, string Content)
        {
            // Sender's email address and app-specific password
            string fromAddress = "gymmanagement0@gmail.com";
            string password = "tsda oboq jsdv mjgq";


            // Get the currently logged-in user
            ApplicationUser user = await _userManager.GetUserAsync(User);

            try
            {
                // Set up SMTP client
                SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);
                smtp.EnableSsl = true;
                smtp.Credentials = new NetworkCredential(fromAddress, password);

                // Create MailMessage object
                MailMessage mailMessage = new MailMessage();
                mailMessage.From = new MailAddress(fromAddress);
                mailMessage.To.Add(new MailAddress(ReceiverId));
                mailMessage.Subject = Subject;
                mailMessage.Body = Content;

                // Send the email
                smtp.Send(mailMessage);



                // Create a new Member with the UserId set to the current user's Id
                var newEmail = new Email
                {
                    SenderId = fromAddress,
                    ReceiverId = ReceiverId,
                    Subject = Subject,
                    Content = Content,
                    Timestamp = DateTime.Now,
                    ApplicationUser = user,
                };


                // Add the new Member and TransactionFee to the database
                _context.Emails.Add(newEmail);

                // Save changes to the database
                await _context.SaveChangesAsync();





                ViewBag.Message = "Email sent successfully!";
            }
            catch (System.Exception ex)
            {
                ViewBag.Message = "Error sending email: " + ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<List<Email>> GetEmailsAsync(string emailAddress)
        {
            // Retrieve emails sent and received by the specified email address, ordered by timestamp in descending order
            var emails = await _context.Emails
                .Where(e => e.SenderId == emailAddress || e.ReceiverId == emailAddress)
                .OrderByDescending(e => e.Timestamp)
                .ToListAsync();

            return emails;
        }

        /*[HttpPost]
        public async Task<ActionResult> ShowEmails(string emailAddress)
        {
            List<Email> relevantEmails = new List<Email>();
            // Get the currently logged-in user
            ApplicationUser user = await _userManager.GetUserAsync(User);

            try
            {
                // Retrieve relevant emails where the inputted email is either the sender or receiver
                relevantEmails = _context.Emails
                    .Where(e => e.SenderId == emailAddress || e.ReceiverId == emailAddress)
                    .ToList();

                // Format the emails for display
                string emailsMessage = "No emails found.";
                if (relevantEmails.Any())
                {
                    emailsMessage = "<ul>";
                    foreach (var email in relevantEmails)
                    {
                        emailsMessage += "<li>";
                        emailsMessage += $"<strong>From:</strong> {email.SenderId}<br>";
                        emailsMessage += $"<strong>To:</strong> {email.ReceiverId}<br>";
                        emailsMessage += $"<strong>Subject:</strong> {email.Subject}<br>";
                        emailsMessage += $"<strong>Content:</strong> {email.Content}<br>";
                        emailsMessage += "</li>";
                    }
                    emailsMessage += "</ul>";
                }

                return Content(emailsMessage); // Return the formatted emails as HTML content
            }
            catch (Exception ex)
            {
                return Content("Error retrieving emails: " + ex.Message); // Return an error message
            }
        }
*/

       /* public async Task<ActionResult> ReceiveEmailsAsync(string emailAddress)
        {
            List<MimeMessage> receivedEmails = new List<MimeMessage>();
            // Get the currently logged-in user
            ApplicationUser user = await _userManager.GetUserAsync(User);
            string fromAddress = "gymmanagement0@gmail.com";

            try
            {
                using (var client = new ImapClient())
                {
                    client.Connect("imap.gmail.com", 993, true);
                    client.Authenticate("gymmanagement0@gmail.com", "tsda oboq jsdv mjgq");

                    var inbox = client.Inbox;
                    inbox.Open(FolderAccess.ReadOnly);

                    // Search for emails with the specified email address in the From header
                    var query = SearchQuery.HeaderContains("From", emailAddress);
                    var uids = inbox.Search(query);

                    foreach (var uid in uids)
                    {
                        var message = inbox.GetMessage(uid);
                        receivedEmails.Add(message);
                    }

                    client.Disconnect(true);
                }



                // Save received emails to the database
                foreach (var message in receivedEmails)
                {
                        var recipientAddress = message.From.Mailboxes.FirstOrDefault()?.Address;


                    // Check if an email with the same content already exists
                    bool emailExists = _context.Emails.Any(e =>
                        e.SenderId == recipientAddress &&
                        e.ReceiverId == fromAddress &&
                        e.Subject == message.Subject &&
                        e.Content == message.TextBody &&
                        e.Timestamp == message.Date.DateTime &&
                        e.ApplicationUser.Id == user.Id);

                    if (!emailExists)
                    {


                        var newEmail = new Email
                        {
                            SenderId = recipientAddress, // Assuming SenderId is a string field
                            ReceiverId = fromAddress,
                            Subject = message.Subject,
                            Content = message.TextBody, // Assuming Content is a string field
                            Timestamp = message.Date.DateTime,
                            ApplicationUser = user
                        };

                        _context.Emails.Add(newEmail);
                    }
                }


                ViewBag.ReceivedEmails = receivedEmails;

                // Save changes to the database
                await _context.SaveChangesAsync();
            }
            catch (System.Exception ex)
            {
                ViewBag.ErrorMessage = "Error receiving emails: " + ex.Message;
            }

            // Ensure ViewBag.ReceivedEmails is initialized even if an exception occurs
            if (ViewBag.ReceivedEmails == null)
            {
                ViewBag.ReceivedEmails = new List<MimeMessage>();
            }

            ViewBag.EmailAddress = emailAddress; // Pass the email address to the view

            return View();
        }*/



        public IActionResult EmailPage()
        {
            // Retrieve all members from the database
            var members = _context.Members.ToList(); // Assuming "Members" is your DbSet in DbContext

            // Pass the list of members to the view
            return View(members);
        }

 
        // GET: Emails/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Emails == null)
            {
                return NotFound();
            }

            var email = await _context.Emails
                .Include(e => e.ApplicationUser)
                .FirstOrDefaultAsync(m => m.EmailId == id);
            if (email == null)
            {
                return NotFound();
            }

            return View(email);
        }

        // GET: Emails/Create
        public IActionResult Create()
        {
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id");
            return View();
        }

        // POST: Emails/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("EmailId,SenderId,ReceiverId,Subject,Content,Timestamp,UserId")] Email email)
        {
            if (ModelState.IsValid)
            {
                _context.Add(email);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", email.UserId);
            return View(email);
        }

        // GET: Emails/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Emails == null)
            {
                return NotFound();
            }

            var email = await _context.Emails.FindAsync(id);
            if (email == null)
            {
                return NotFound();
            }
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", email.UserId);
            return View(email);
        }

        // POST: Emails/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("EmailId,SenderId,ReceiverId,Subject,Content,Timestamp,UserId")] Email email)
        {
            if (id != email.EmailId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(email);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EmailExists(email.EmailId))
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
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", email.UserId);
            return View(email);
        }

        // GET: Emails/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Emails == null)
            {
                return NotFound();
            }

            var email = await _context.Emails
                .Include(e => e.ApplicationUser)
                .FirstOrDefaultAsync(m => m.EmailId == id);
            if (email == null)
            {
                return NotFound();
            }

            return View(email);
        }

        // POST: Emails/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Emails == null)
            {
                return Problem("Entity set 'WebDbContext.Emails'  is null.");
            }
            var email = await _context.Emails.FindAsync(id);
            if (email != null)
            {
                _context.Emails.Remove(email);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EmailExists(int id)
        {
          return (_context.Emails?.Any(e => e.EmailId == id)).GetValueOrDefault();
        }
    }
}
