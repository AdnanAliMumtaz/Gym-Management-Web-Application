using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Areas.Identity.Data;
using WebApplication1.Data;
using WebApplication1.Models;
using WebApplication1.Models.ViewModels;

namespace WebApplication1.Controllers
{
    public class MembersController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;

        private readonly WebDbContext _context;

        public MembersController(WebDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        /*public IActionResult Index()
        {
            return View();
        }*/

        /*// GET: Member
        public async Task<IActionResult> Index()
        {
            var members = await _context.Members.ToListAsync();
            return View(members);
        }*/

        // GET: Member
        public async Task<IActionResult> Index()
        {
            ApplicationUser user = await _userManager.GetUserAsync(User);

            // Check if the user is authenticated
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // Filter members based on the user's ID
            var members = await _context.Members
                .Where(m => m.UserId == user.Id)
                .ToListAsync();

            return View(members);
        }

        public IActionResult Members()
        {
            var members = _context.Members.ToList(); // Retrieve the list of members from your context
            return View(members);
        }


        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddDummyData(string FirstName, string LastName, string Email, string PhoneNumber, decimal Amount, DateTime DatePaid)
        {
            // Get the currently logged-in user
            ApplicationUser user = await _userManager.GetUserAsync(User);

            // Create a new Member with the UserId set to the current user's Id
            var newMember = new Member
            {
                MemberFirstName = FirstName,
                MemberLastName = LastName,
                MemberEmail = Email,
                MemberPhoneNumber = PhoneNumber,
                /*MemberEmail = "adnan3432@gmail.com",*/
                /*MemberPhoneNumber = "093434334",*/
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
    }

}
