using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class WhatsappMessagesController : Controller
    {
        private readonly WebDbContext _context;

        public WhatsappMessagesController(WebDbContext context)
        {
            _context = context;
        }

        // GET: WhatsappMessages
        public async Task<IActionResult> Index()
        {
            var webDbContext = _context.WhatsappMessages.Include(w => w.ApplicationUser);
            return View(await webDbContext.ToListAsync());
        }

        // GET: WhatsappMessages/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.WhatsappMessages == null)
            {
                return NotFound();
            }

            var whatsappMessage = await _context.WhatsappMessages
                .Include(w => w.ApplicationUser)
                .FirstOrDefaultAsync(m => m.MessageId == id);
            if (whatsappMessage == null)
            {
                return NotFound();
            }

            return View(whatsappMessage);
        }

        // GET: WhatsappMessages/Create
        public IActionResult Create()
        {
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id");
            return View();
        }

        // POST: WhatsappMessages/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MessageId,SenderId,ReceiverId,Content,Timestamp,UserId")] WhatsappMessage whatsappMessage)
        {
            if (ModelState.IsValid)
            {
                _context.Add(whatsappMessage);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", whatsappMessage.UserId);
            return View(whatsappMessage);
        }

        // GET: WhatsappMessages/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.WhatsappMessages == null)
            {
                return NotFound();
            }

            var whatsappMessage = await _context.WhatsappMessages.FindAsync(id);
            if (whatsappMessage == null)
            {
                return NotFound();
            }
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", whatsappMessage.UserId);
            return View(whatsappMessage);
        }

        // POST: WhatsappMessages/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MessageId,SenderId,ReceiverId,Content,Timestamp,UserId")] WhatsappMessage whatsappMessage)
        {
            if (id != whatsappMessage.MessageId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(whatsappMessage);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!WhatsappMessageExists(whatsappMessage.MessageId))
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
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", whatsappMessage.UserId);
            return View(whatsappMessage);
        }

        // GET: WhatsappMessages/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.WhatsappMessages == null)
            {
                return NotFound();
            }

            var whatsappMessage = await _context.WhatsappMessages
                .Include(w => w.ApplicationUser)
                .FirstOrDefaultAsync(m => m.MessageId == id);
            if (whatsappMessage == null)
            {
                return NotFound();
            }

            return View(whatsappMessage);
        }

        // POST: WhatsappMessages/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.WhatsappMessages == null)
            {
                return Problem("Entity set 'WebDbContext.WhatsappMessages'  is null.");
            }
            var whatsappMessage = await _context.WhatsappMessages.FindAsync(id);
            if (whatsappMessage != null)
            {
                _context.WhatsappMessages.Remove(whatsappMessage);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool WhatsappMessageExists(int id)
        {
          return (_context.WhatsappMessages?.Any(e => e.MessageId == id)).GetValueOrDefault();
        }
    }
}
