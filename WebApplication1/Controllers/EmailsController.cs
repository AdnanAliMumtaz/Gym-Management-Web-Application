using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using MailKit;
using MailKit.Net.Imap;
using MailKit.Search;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Azure.Management.Monitor.Fluent.Models;
using Microsoft.EntityFrameworkCore;
using MimeKit;
using WebApplication1.Data;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class EmailsController : Controller
    {
        private readonly WebDbContext _context;
        private readonly EmailReceiver _emailReceiver;

        public EmailsController(WebDbContext context)
        {
            _context = context;
            _emailReceiver = new EmailReceiver();
        }

        // GET: Emails
        /*public async Task<IActionResult> Index()
        {
            Email email = new Email(); 
            return View(email);
        }*/

        // GET: Emails
        public async Task<IActionResult> Index()
        {
            // Retrieve all members from the database
            var allMembers = await _context.Members.ToListAsync(); // Replace _context.Members with your actual DbSet for members

            // Pass the list of members to the view
            ViewBag.AllMembers = allMembers;

            // Retrieve all employees from the database
            var allEmployees = await _context.Employees.ToListAsync(); // Replace _context.Members with your actual DbSet for members

            // Pass the list of members to the view
            ViewBag.AllEmployees = allEmployees;


            // Create a new instance of Email if needed
            Email email = new Email(); // Assuming you're creating a new instance of Email

            return View(email);
        }


        [HttpPost]
        public async Task<IActionResult> SendEmail(string ReceiverId, string Subject, string Content)
        {
            // Sender's email address and app-specific password
            string fromAddress = "gymmanagement0@gmail.com";
            /*string password = "Gym12345@Gym";*/
            string password = "tsda oboq jsdv mjgq";


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

                ViewBag.Message = "Email sent successfully!";
            }
            catch (System.Exception ex)
            {
                ViewBag.Message = "Error sending email: " + ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }








        public ActionResult ReceiveEmails(string emailAddress)
        {
            List<MimeMessage> receivedEmails = new List<MimeMessage>();

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

                ViewBag.ReceivedEmails = receivedEmails;
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
        }

        public IActionResult EmailPage()
        {
            // Retrieve all members from the database
            var members = _context.Members.ToList(); // Assuming "Members" is your DbSet in DbContext

            // Pass the list of members to the view
            return View(members);
        }






        /*[HttpPost]
        public async Task<IActionResult> SendEmail(string ReceiverId, string Subject, string Content)
        {


            // Sender's email address and password
            string fromAddress = "gymmanagement0@gmail.com";
            string password = "Gym12345@Gym";

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

            try
            {

                *//*// Sender's email address and password
                string fromAddress = "gymmanagement0@gmail.com";
                string password = "Gym12345@Gym";

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
                smtp.Send(mailMessage);*//*

                ViewBag.Message = "Email sent successfully!";
            }
            catch (System.Exception ex)
            {
                ViewBag.Message = "Error sending email: " + ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }*/


        /*public ViewResult Index(WebApplication1.Models.Email _objModelMail)
        {
            if (ModelState.IsValid)
            {
                MailMessage mail = new MailMessage();
                mail.To.Add(_objModelMail.SenderId);
                mail.From = new MailAddress(_objModelMail.ReceiverId);
                mail.Subject = _objModelMail.Subject;
                string Body = _objModelMail.Content;
                mail.Body = Body;
                mail.IsBodyHtml = true;
                SmtpClient smtp = new SmtpClient();
                smtp.Host = "smtp.gmail.com";
                smtp.Port = 587;
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new System.Net.NetworkCredential("khanaddi193@gmail.com", "audhfH1482$"); 
                smtp.EnableSsl = true;
                smtp.Send(mail);
                return View("Index", _objModelMail);
            }
            else
            {
                return View();
            }
        }



*/


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
