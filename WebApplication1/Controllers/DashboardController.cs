using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using WebApplication1.Data;
using WebApplication1.Models;
using System.Linq;
using WebApplication1.Models.ViewModels;
using WebApplication1.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace WebApplication1.Controllers
{
    public class DashboardController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly WebDbContext _context;
        public DashboardController(WebDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }


        /*[Authorize]
        public IActionResult Index()
        {
            return View();
        }*/

















        [Authorize]
        public async Task<IActionResult> Index()
        {
            // Get the currently logged-in user
            ApplicationUser user = await _userManager.GetUserAsync(User);

            // Query 1: Total Fees Amount for the specific user's members
            decimal totalFeesAmount = _context.TransactionFees
                .Where(tf => tf.Member.UserId == user.Id)
                .Sum(tf => tf.Amount);

            // Query 2: Count of New Members who joined in the recent one month for the specific user
            DateTime oneMonthAgo = DateTime.Now.AddMonths(-1);
            int newMembersCount = _context.Members
                .Count(m => m.MemberDateJoined >= oneMonthAgo && m.UserId == user.Id);

            // Query 3: Count of Active Admission for the specific user (members with non-null MemberDateJoined)
            int activeAdmissionMembersCount = _context.Members
                .Count(m => m.MemberDateJoined != null && m.UserId == user.Id);

            // Query 4: Retrieve all EntryLogs for the specific user
            var entryLogs = _context.EntryLogs
                .Where(el => el.Member.UserId == user.Id)
                .ToList();

            // Create a ViewModel to hold the data for the view
            var viewModel = new DashboardViewModel
            {
                TotalFeesAmount = totalFeesAmount,
                NewMembersCount = newMembersCount,
                ActiveAdmissionMembersCount = activeAdmissionMembersCount,
                EntryLogs = entryLogs
            };

            // Pass the ViewModel to the view
            return View(viewModel);
        }






















        /*[Authorize]
        public async Task<IActionResult> Index()
        {
            // Get the currently logged-in user
            ApplicationUser user = await _userManager.GetUserAsync(User);

            // Query 1: Total Fees Amount for the specific user's members
            decimal totalFeesAmount = _context.TransactionFees
                .Where(tf => tf.Member.UserId == user.Id)
                .Sum(tf => tf.Amount);

            // Query 2: Count of New Members who joined in the recent one month for the specific user
            DateTime oneMonthAgo = DateTime.Now.AddMonths(-1);
            int newMembersCount = _context.Members
                .Count(m => m.MemberDateJoined >= oneMonthAgo && m.UserId == user.Id);

            // Query 3: Count of Active Admission for the specific user (members with non-null MemberDateJoined)
            int activeAdmissionMembersCount = _context.Members
                .Count(m => m.MemberDateJoined != null && m.UserId == user.Id);

            // Create a ViewModel to hold the data for the view
            var viewModel = new DashboardViewModel
            {
                TotalFeesAmount = totalFeesAmount,
                NewMembersCount = newMembersCount,
                ActiveAdmissionMembersCount = activeAdmissionMembersCount
            };

            // Pass the ViewModel to the view
            return View(viewModel);
        }*/

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddDummyData(string FirstName, string LastName, decimal Amount, DateTime DatePaid)
        {
            // Get the currently logged-in user
            ApplicationUser user = await _userManager.GetUserAsync(User);

            // Create a new Member with the UserId set to the current user's Id
            var newMember = new Member
            {
                MemberFirstName = FirstName,
                MemberLastName = LastName,
                MemberEmail = "adnan3432@gmail.com",
                MemberPhoneNumber = "093434334",
                MemberDateJoined = DatePaid,
                ApplicationUser = user,
            };

            // Create a new TransactionFee associated with the new member
            var newTransactionFee = new TransactionFee
            {
                Amount = Amount,
                DatePaid = DatePaid,
                Member = newMember,
            };

            // Add the new Member and TransactionFee to the database
            _context.Members.Add(newMember);
            _context.TransactionFees.Add(newTransactionFee);

            // Save changes to the database
            await _context.SaveChangesAsync();

            // Redirect to a success page or back to the Members page
            return RedirectToAction(nameof(Index));
        }












        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddEntry()
        {
            // Get the currently logged-in user
            ApplicationUser user = await _userManager.GetUserAsync(User);

            // Get the member associated with the current user
            Member member = _context.Members.FirstOrDefault(m => m.UserId == user.Id);

            if (member != null)
            {
                // Create a new entry log associated with the member
                var newEntryLog = new EntryLog
                {
                    EntryDate = DateTime.Now,
                    RfidTag = "Adnasndjkash kj234 ekja",
                    Member = member
                };

                // Add the new entry log to the database
                _context.EntryLogs.Add(newEntryLog);

                // Save changes to the database
                await _context.SaveChangesAsync();
            }

            // Redirect back to the dashboard
            return RedirectToAction(nameof(Index));
        }

    }
}
