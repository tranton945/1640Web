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
    [Authorize(Roles = "Staff, Admin, Manager, Coordinator")]
    public class NotificationUserController : Controller
    {
        private readonly ApplicationDbContext _context;

        public NotificationUserController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: NotificationUser
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
        public async Task<IActionResult> Filter(string typeData)
        {
            var notifications = await _context.tNotifications.OrderByDescending(n => n.Id).ToListAsync();

            // select data (Type) in database 
            // sau này lấy thêm datetime (đã note)
            var types = await _context.tNotifications.Select(n => n.Type).Distinct().ToListAsync();
            //var latestDay = _context.Notifications.Max(n => n.Day);
            //var earliestDay = _context.Notifications.Min(n => n.Day);
            var allOptions = new List<SelectListItem>();
            allOptions.Add(new SelectListItem { Text = " -- Select --", Value = "" });
            foreach (var type in types)
            {
                allOptions.Add(new SelectListItem { Text = type, Value = type });
            }
            //allOptions.Add(new SelectListItem { Text = $"Latest day: {latestDay.ToShortDateString()}", Value = latestDay.ToString("yyyy-MM-dd") });
            //allOptions.Add(new SelectListItem { Text = $"Earliest day: {earliestDay.ToShortDateString()}", Value = earliestDay.ToString("yyyy-MM-dd") });
            ViewBag.Types = allOptions;

            if (!String.IsNullOrEmpty(typeData))
            {
                //notifications = await notifications.Where(n => n.Type == typeData).ToListAsync();
                notifications = await _context.tNotifications
                    .Where(n => n.Type == typeData)
                    .OrderByDescending(n => n.Id)
                    .ToListAsync();
            }

            //return RedirectToAction("Index", notifications);
            return View("Index", notifications);
        }
        // handle search
        public async Task<IActionResult> Search(string searchString)
        {
            // select data (Type) in database 
            // sau này lấy thêm datetime (đã note)
            var types = await _context.tNotifications.Select(n => n.Type).Distinct().ToListAsync();
            //var latestDay = _context.Notifications.Max(n => n.Day);
            //var earliestDay = _context.Notifications.Min(n => n.Day);
            var allOptions = new List<SelectListItem>();
            allOptions.Add(new SelectListItem { Text = " -- Select --", Value = "" });
            foreach (var type in types)
            {
                allOptions.Add(new SelectListItem { Text = type, Value = type });
            }
            //allOptions.Add(new SelectListItem { Text = $"Latest day: {latestDay.ToShortDateString()}", Value = latestDay.ToString("yyyy-MM-dd") });
            //allOptions.Add(new SelectListItem { Text = $"Earliest day: {earliestDay.ToShortDateString()}", Value = earliestDay.ToString("yyyy-MM-dd") });
            ViewBag.Types = allOptions;

            // data after search
            var results = await _context.tNotifications
                .Where(n => n.Title.Contains(searchString) ||
                            n.Content.Contains(searchString))
                .OrderByDescending(n => n.Id)
                .ToListAsync();

            return View("Index", results);
        }

        // GET: NotificationUser/Details/5
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

        // GET: NotificationUser/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: NotificationUser/Create
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

        // GET: NotificationUser/Edit/5
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
            return View(tNotification);
        }

        // POST: NotificationUser/Edit/5
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

        // GET: NotificationUser/Delete/5
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

        // POST: NotificationUser/Delete/5
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
