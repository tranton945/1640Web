using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using _1640WebApp.Data;
using _1640WebApp.Models;
using Microsoft.AspNetCore.Authorization;

namespace _1640WebApp.Controllers
{
    [Authorize(Roles = " Admin, Manager")]
    public class NotificationAdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public NotificationAdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: NotificationAdmin
        public async Task<IActionResult> Index()
        {
            var notifications = await _context.tNotifications.OrderByDescending(n => n.Id).ToListAsync();
            var types = await _context.tNotifications.Select(n => n.Type).Distinct().ToListAsync();
            var allOptions = new List<SelectListItem>();
            allOptions.Add(new SelectListItem { Text = " -- Select --", Value = "" });
            foreach (var type in types)
            {
                allOptions.Add(new SelectListItem { Text = type, Value = type });
            }
            ViewBag.Types = allOptions;
            return View(notifications);
        }
        // sort
        public async Task<IActionResult> FilterAdmin(string typeData)
        {
            var notifications = await _context.tNotifications.OrderByDescending(n => n.Id).ToListAsync();
            var types = await _context.tNotifications.Select(n => n.Type).Distinct().ToListAsync();
            var allOptions = new List<SelectListItem>();
            allOptions.Add(new SelectListItem { Text = " -- Select --", Value = "" });
            foreach (var type in types)
            {
                allOptions.Add(new SelectListItem { Text = type, Value = type });
            }
            ViewBag.Types = allOptions;
            if (!String.IsNullOrEmpty(typeData))
            {
                notifications = await _context.tNotifications
                    .Where(n => n.Type == typeData)
                    .OrderByDescending(n => n.Id)
                    .ToListAsync();
            }
            return View("Index", notifications);
        }
        // handle search
        public async Task<IActionResult> SearchAdmin(string searchString)
        {
            var notifications = await _context.tNotifications.OrderByDescending(n => n.Id).ToListAsync();
            var types = await _context.tNotifications.Select(n => n.Type).Distinct().ToListAsync();
            var allOptions = new List<SelectListItem>();
            allOptions.Add(new SelectListItem { Text = " -- Select --", Value = "" });
            foreach (var type in types)
            {
                allOptions.Add(new SelectListItem { Text = type, Value = type });
            }
            ViewBag.Types = allOptions;

            // data after search
            var results = await _context.tNotifications
                .Where(n => n.Title.Contains(searchString) ||
                            n.Content.Contains(searchString))
                .OrderByDescending(n => n.Id)
                .ToListAsync();

            return View("Index", results);
        }

        // GET: NotificationAdmin/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.tNotifications == null)
            {
                return NotFound();
            }

            var tNotification = await _context.tNotifications
                .FirstOrDefaultAsync(m => m.Id == id);
            if (tNotification == null)
            {
                return NotFound();
            }

            return View(tNotification);
        }

        // GET: NotificationAdmin/Create
        public IActionResult Create()
        {
            List<SelectListItem> type = new()
            {
                new SelectListItem { Value = "Info", Text = "Info" },
                new SelectListItem { Value = "Waring", Text = "Waring" },
                new SelectListItem { Value = "Danger", Text = "Danger" },
            };

            ViewBag.type = type;
            return View();
        }

        // POST: NotificationAdmin/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Content,Type")] tNotification tNotification)
        {
            if (ModelState.IsValid)
            {
                _context.Add(tNotification);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(tNotification);
        }

        // GET: NotificationAdmin/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.tNotifications == null)
            {
                return NotFound();
            }

            var tNotification = await _context.tNotifications.FindAsync(id);
            if (tNotification == null)
            {
                return NotFound();
            }
            List<SelectListItem> type = new()
            {
                new SelectListItem { Value = "Info", Text = "Info" },
                new SelectListItem { Value = "Waring", Text = "Waring" },
                new SelectListItem { Value = "Danger", Text = "Danger" },
            };

            ViewBag.type = type;
            return View(tNotification);
        }

        // POST: NotificationAdmin/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Content,Type")] tNotification tNotification)
        {
            if (id != tNotification.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tNotification);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!tNotificationExists(tNotification.Id))
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
            return View(tNotification);
        }

        // GET: NotificationAdmin/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.tNotifications == null)
            {
                return NotFound();
            }

            var tNotification = await _context.tNotifications
                .FirstOrDefaultAsync(m => m.Id == id);
            if (tNotification == null)
            {
                return NotFound();
            }

            return View(tNotification);
        }

        // POST: NotificationAdmin/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.tNotifications == null)
            {
                return Problem("Entity set 'ApplicationDbContext.tNotifications'  is null.");
            }
            var tNotification = await _context.tNotifications.FindAsync(id);
            if (tNotification != null)
            {
                _context.tNotifications.Remove(tNotification);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool tNotificationExists(int id)
        {
          return (_context.tNotifications?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
