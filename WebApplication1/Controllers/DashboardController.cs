using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using WebApplication1.Data;
using WebApplication1.Models;
using System.Linq; // Import this namespace for LINQ queries
/*using AspNetCore;*/
using WebApplication1.Models.ViewModels;
using WebApplication1.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace WebApplication1.Controllers
{
    public class DashboardController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ManagingDbContext _context;
        public DashboardController(ManagingDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }


        /*[Authorize]
        public IActionResult Index()
        {
            return View();
        }*/


        /*        [Authorize]
                public IActionResult Index()
                {
                    // Query 1: Total Fees Amount for all members
                    decimal totalFeesAmount = _context.TransactionFees.Sum(tf => tf.Amount);

                    // Query 2: Count of New Members who joined in the recent one month
                    DateTime oneMonthAgo = DateTime.Now.AddMonths(-1);
                    int newMembersCount = _context.Members
                        .Count(m => m.MemberDateJoined >= oneMonthAgo);

                    // Query 3: Count of Active Admission (all members with non-null MemberDateJoined)
                    int activeAdmissionMembersCount = _context.Members
                        .Count(m => m.MemberDateJoined != null);

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
        public IActionResult Index()
        {
            // Get the currently logged-in user
            ApplicationUser user = _userManager.GetUserAsync(User).Result;

            // Query 1: Total Fees Amount for the current user's members
            decimal totalFeesAmount = _context.TransactionFees
                .Where(tf => tf.Member.ApplicationUser == user)
                .Sum(tf => tf.Amount);

            // Query 2: Count of New Members who joined in the recent one month for the current user
            DateTime oneMonthAgo = DateTime.Now.AddMonths(-1);
            int newMembersCount = _context.Members
                .Where(m => m.MemberDateJoined >= oneMonthAgo && m.ApplicationUser == user)
                .Count();

            // Query 3: Count of Active Admission Members for the current user
            int activeAdmissionMembersCount = _context.Members
                .Where(m => m.MemberDateJoined != null && m.ApplicationUser == user)
                .Count();

            // Create a ViewModel to hold the data for the view
            var viewModel = new DashboardViewModel
            {
                TotalFeesAmount = totalFeesAmount,
                NewMembersCount = newMembersCount,
                ActiveAdmissionMembersCount = activeAdmissionMembersCount
            };

            // Pass the ViewModel to the view
            return View(viewModel);
        }



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
                UserId = user.Id,
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
    }
}
