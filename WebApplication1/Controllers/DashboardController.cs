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




        [Authorize]
        public async Task<IActionResult> Index()
        {
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

            // Retrieve the list of members for the specific user
            var members = _context.Members
                .Where(m => m.UserId == user.Id)
                .ToList();

            // Calculate the start date for the last seven days
            DateTime startDate = DateTime.Now.Date.AddDays(-6);

            // Create a list with default values (0) for the last seven days
            var entryLogsPerDay = Enumerable.Range(0, 7)
                .Select(offset => startDate.AddDays(offset).Date)
                .ToDictionary(date => date, date => 0);

            // Query 5: Retrieve EntryLogs for the last seven days for the specific user
            var entryLogsLastSevenDays = _context.EntryLogs
                .Where(el => el.Member.UserId == user.Id && el.EntryDate >= startDate)
                .ToList();

            // Update the counts in the entryLogsPerDay list
            foreach (var entryLog in entryLogsLastSevenDays)
            {
                var entryDate = entryLog.EntryDate.Date;
                entryLogsPerDay[entryDate]++;
            }

            // Query 6: Retrieve Members joined and left per month for the last 12 months
            DateTime startMonthDate = DateTime.Now.Date.AddMonths(-11); // Start from 12 months ago


            /*// Query 6: Retrieve Members joined and left per month for the last 12 months
            var membersJoinedPerMonth = _context.Members
                .Where(m => m.UserId == user.Id && m.MemberDateJoined >= startMonthDate)
                .GroupBy(m => new { Year = m.MemberDateJoined.Year, Month = m.MemberDateJoined.Month })
                .Select(group => new EntryCount
                {
                    Date = new DateTime(group.Key.Year, group.Key.Month, 1), // First day of the month
                    Count = group.Count()
                })
                .ToDictionary(entry => entry.Date, entry => entry);

            var membersLeftPerMonth = _context.Members
                .Where(m => m.UserId == user.Id && m.MemberDateLeft.HasValue && m.MemberDateLeft.Value >= startMonthDate)
                .GroupBy(m => new { Year = m.MemberDateLeft.Value.Year, Month = m.MemberDateLeft.Value.Month })
                .Select(group => new EntryCount
                {
                    Date = new DateTime(group.Key.Year, group.Key.Month, 1), // First day of the month
                    Count = group.Count()
                })
                .ToDictionary(entry => entry.Date, entry => entry);*/



            var currentDate = DateTime.Now.Date;

            // Create a sequence of the last 12 months
            var last12Months = Enumerable.Range(0, 12)
                .Select(offset => currentDate.AddMonths(-offset))
                .Select(date => new DateTime(date.Year, date.Month, 1))
                .ToList();

            // Query 6: Retrieve Members joined per month for the last 12 months
            var membersJoinedPerMonth = last12Months
                .GroupJoin(
                    _context.Members
                        .Where(m => m.UserId == user.Id && m.MemberDateJoined >= startMonthDate)
                        .GroupBy(m => new { Year = m.MemberDateJoined.Year, Month = m.MemberDateJoined.Month })
                        .Select(group => new EntryCount
                        {
                            Date = new DateTime(group.Key.Year, group.Key.Month, 1),
                            Count = group.Count()
                        }),
                    month => month,
                    joinedData => joinedData.Date,
                    (month, joinedData) => joinedData.SingleOrDefault() ?? new EntryCount { Date = month, Count = 0 }
                )
                .ToDictionary(entry => entry.Date, entry => entry);

            // Query 6: Retrieve Members left per month for the last 12 months
            var membersLeftPerMonth = last12Months
                .GroupJoin(
                    _context.Members
                        .Where(m => m.UserId == user.Id && m.MemberDateLeft.HasValue && m.MemberDateLeft.Value >= startMonthDate)
                        .GroupBy(m => new { Year = m.MemberDateLeft.Value.Year, Month = m.MemberDateLeft.Value.Month })
                        .Select(group => new EntryCount
                        {
                            Date = new DateTime(group.Key.Year, group.Key.Month, 1),
                            Count = group.Count()
                        }),
                    month => month,
                    leftData => leftData.Date,
                    (month, leftData) => leftData.SingleOrDefault() ?? new EntryCount { Date = month, Count = 0 }
                )
                .ToDictionary(entry => entry.Date, entry => entry);


            // Create a ViewModel to hold the data for the view
            var viewModel = new DashboardViewModel
            {
                TotalFeesAmount = totalFeesAmount,
                NewMembersCount = newMembersCount,
                ActiveAdmissionMembersCount = activeAdmissionMembersCount,
                Members = members,
                EntryLogsPerDay = entryLogsPerDay,
                MembersJoinedPerMonth = membersJoinedPerMonth,
                MembersLeftPerMonth = membersLeftPerMonth
            };

            // Pass the ViewModel to the view
            return View(viewModel);
        }

































































        [Authorize]
        public async Task<IActionResult> EntriesGraph()
        {
            ApplicationUser user = await _userManager.GetUserAsync(User);

            Member member = await _context.Members
                .Include(m => m.EntryLog)
                .FirstOrDefaultAsync(m => m.UserId == user.Id);

            if (member != null)
            {
                var entriesPerDay = member.EntryLog
                    .Where(el => el.EntryDate >= DateTime.Today.AddMonths(-1))
                    .GroupBy(el => el.EntryDate.Date)
                    .Select(group => new EntryCount { Date = group.Key, Count = group.Count() })
                    .OrderBy(entry => entry.Date)
                    .ToList();

                var viewModel = new EntriesGraphViewModel
                {
                    EntriesPerDay = entriesPerDay
                };

                return View(viewModel);
            }

            return RedirectToAction(nameof(Index));
        }


        /*[Authorize]
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
                *//*MemberEmail = "adnan3432@gmail.com",*/
                /*MemberPhoneNumber = "093434334",*//*
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
        }*/





        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddEntry(int memberId)
        {
            ApplicationUser user = await _userManager.GetUserAsync(User);

            // Get the selected member
            Member member = _context.Members
                .Include(m => m.EntryLog)
                .FirstOrDefault(m => m.UserId == user.Id && m.MemberID == memberId);

            if (member != null)
            {
                // Check if there is an existing entry within the past 2 hours
                DateTime twoHoursAgo = DateTime.Now.AddHours(-2);

                bool hasExistingEntry = member.EntryLog.Any(el => el.EntryDate >= twoHoursAgo);

                if (!hasExistingEntry)
                {
                    // Create a new EntryLog
                    EntryLog newEntryLog = new EntryLog
                    {
                        EntryDate = DateTime.Now,
                        RfidTag = "Adnasndjkash kj234 ekja",
                        Member = member
                    };

                    // Add the new EntryLog to the database
                    _context.EntryLogs.Add(newEntryLog);
                    _context.SaveChanges();

                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    // Member already has an entry within the past 2 hours
                    // Handle this scenario (e.g., display a message or redirect to an error page)
                    return RedirectToAction(nameof(Index));
                }
            }

            // Handle the case where the member is not found
            return RedirectToAction(nameof(Index));
        }




        /*[Authorize]
        [HttpPost]
        public async Task<IActionResult> AddEntry()
        {
            ApplicationUser user = await _userManager.GetUserAsync(User);

            // Get the member for the current user
            Member member = _context.Members
                .Include(m => m.EntryLog)
                .FirstOrDefault(m => m.UserId == user.Id);

            if (member != null)
            {
                // Check if there is an existing entry within the past 2 hours
                DateTime twoHoursAgo = DateTime.Now.AddHours(-2);

                bool hasExistingEntry = member.EntryLog.Any(el => el.EntryDate >= twoHoursAgo);

                if (!hasExistingEntry)
                {
                    // Create a new EntryLog
                    EntryLog newEntryLog = new EntryLog
                    {
                        EntryDate = DateTime.Now,
                        RfidTag = "Adnasndjkash kj234 ekja",
                        Member = member
                    };

                    // Add the new EntryLog to the database
                    _context.EntryLogs.Add(newEntryLog);
                    _context.SaveChanges();

                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    // Member already has an entry within the past 2 hours
                    // Handle this scenario (e.g., display a message or redirect to an error page)
                    return RedirectToAction(nameof(Index));
                }
            }

            // Handle the case where the member is not found
            return RedirectToAction(nameof(Index));
        }*/

    }
}
