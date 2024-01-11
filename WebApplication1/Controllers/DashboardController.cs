using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using WebApplication1.Data;
using WebApplication1.Models;
using System.Linq; // Import this namespace for LINQ queries
/*using AspNetCore;*/
using WebApplication1.Models.ViewModels;



namespace WebApplication1.Controllers
{
    public class DashboardController : Controller
    {
        private readonly ManagingDbContext _context;
        public DashboardController(ManagingDbContext context)
        {
            _context = context;
        }


        /*[Authorize]
        public IActionResult Index()
        {
            return View();
        }*/


        [Authorize]
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
        }





        /*[Authorize]
        public IActionResult Index()
        {
            // Retrieve data from the database (example: get all members and related transaction fees)
            var membersWithTransactionFees = _context.Members.Include(m => m.TransactionFee).ToList();

            // Pass the data to the view
            return View(membersWithTransactionFees);
        }



        [HttpPost]
        public IActionResult AddDummyData(string FirstName, string LastName, decimal Amount, DateTime DatePaid)
        {
            // Create a new Member
            var newMember = new Member
            {
                MemberFirstName = FirstName,
                MemberLastName = LastName,
                MemberEmail = "adnan3432@gmail.com",
                MemberPhoneNumber = "093434334",
                MemberDateJoined = DatePaid
            };

            // Create a new TransactionFee
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
            _context.SaveChanges();

            // Redirect to a success page or back to the Members page
            *//*return RedirectToAction("/Dashboard/Index");*//*
            return RedirectToAction(nameof(Index));
        }*/




    }
}
