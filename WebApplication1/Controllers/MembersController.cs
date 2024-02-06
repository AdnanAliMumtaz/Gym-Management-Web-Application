﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Areas.Identity.Data;
using WebApplication1.Data;
using WebApplication1.Models;
using WebApplication1.Models.ViewModels;

namespace WebApplication1.Controllers
{
    [Authorize]
    public class MembersController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;

        private readonly WebDbContext _context;

        public MembersController(WebDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        /*public async Task<IActionResult> Index()
        {
            ApplicationUser user = await _userManager.GetUserAsync(User);

            // Filter members based on the user's ID
            var members = await _context.Members
                .Where(m => m.UserId == user.Id)
                .ToListAsync();

            // Retrieve entry logs for the specific user
            var entryLogs = _context.EntryLogs
                .Where(el => el.Member.UserId == user.Id)
                .ToList();

            // Create a DashboardViewModel instance and set its Members property
            var viewModel = new WebApplication1.Models.ViewModels.DashboardViewModel
            {
                Members = members,
                EntryLogs =  entryLogs
            };

            return View(viewModel);
        }*/


        [HttpGet]
        public IActionResult Search(string search)
        {
            ApplicationUser user = _userManager.GetUserAsync(User).Result;

            // Filter members based on the user's ID and the search query
            var membersQuery = _context.Members
                .Where(m => m.UserId == user.Id);

            if (!string.IsNullOrEmpty(search))
            {
                search = search.ToLower();
                membersQuery = membersQuery.Where(m =>
                    m.MemberFirstName.ToLower().Contains(search) ||
                    m.MemberLastName.ToLower().Contains(search) ||
                    m.MemberEmail.ToLower().Contains(search) ||
                    m.MemberPhoneNumber.Contains(search));
            }

            var members = membersQuery.ToList();

            return PartialView("_SearchResultsPartial", members);
        }

        public async Task<IActionResult> Index(string search)
        {
            ApplicationUser user = await _userManager.GetUserAsync(User);

            // Filter members based on the user's ID
            var membersQuery = _context.Members
                .Where(m => m.UserId == user.Id);

            // Apply search filter if a search query is provided
            if (!string.IsNullOrEmpty(search))
            {
                search = search.ToLower();
                membersQuery = membersQuery.Where(m =>
                    m.MemberFirstName.ToLower().Contains(search) ||
                    m.MemberLastName.ToLower().Contains(search) ||
                    m.MemberEmail.ToLower().Contains(search) ||
                    m.MemberPhoneNumber.Contains(search));
            }

            var members = await membersQuery.ToListAsync();

            // Retrieve entry logs for the specific user
            var entryLogs = _context.EntryLogs
                .Where(el => el.Member.UserId == user.Id)
                .ToList();

            // Create a DashboardViewModel instance and set its Members property
            var viewModel = new WebApplication1.Models.ViewModels.DashboardViewModel
            {
                Members = members,
                EntryLogs = entryLogs
            };

            return View(viewModel);
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

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var member = await _context.Members
                .FirstOrDefaultAsync(m => m.MemberID == id);

            if (member == null)
            {
                return NotFound();
            }

            return View(member);
        }

        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public IActionResult EditConfirmed(Member updatedMember)
        {
            /*if (ModelState.IsValid)
            {
            }*/

            // Check if the member exists in the database
            var existingMember = _context.Members.Find(updatedMember.MemberID);

            if (existingMember == null)
            {
                // If the member is not found, return a 404 Not Found result
                return NotFound();
            }

            // Update the existing member's properties with the edited values
            existingMember.MemberFirstName = updatedMember.MemberFirstName;
            existingMember.MemberLastName = updatedMember.MemberLastName;
            existingMember.MemberEmail = updatedMember.MemberEmail;
            existingMember.MemberPhoneNumber = updatedMember.MemberPhoneNumber;
            existingMember.MemberDateJoined = updatedMember.MemberDateJoined;
            existingMember.MemberDateLeft = updatedMember.MemberDateLeft;

            // Update the existing member's properties with the edited values
            /*_context.Update(updatedMember);*/

            _context.Entry(existingMember).State = EntityState.Modified;


            // Save changes to the database
            _context.SaveChanges();

            // Redirect to the member details page or another appropriate page
            // return RedirectToAction("Edit", new { id = existingMember.MemberID });

            // If the model state is not valid, return to the edit view with validation errors
            return RedirectToAction(nameof(Index));
        }


        // GET: Member/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var member = await _context.Members
                .FirstOrDefaultAsync(m => m.MemberID == id);

            if (member == null)
            {
                return NotFound();
            }

            return View(member);
        }

        // POST: Member/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var member = await _context.Members.FindAsync(id);
            _context.Members.Remove(member);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

    }

}
