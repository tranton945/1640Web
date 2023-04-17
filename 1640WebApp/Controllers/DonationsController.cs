using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using _1640WebApp.Data;

namespace _1640WebApp.Controllers
{
    public class DonationsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DonationsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Donations
        public async Task<IActionResult> Index()
        {
              return View(await _context.Donations.ToListAsync());
        } 


        // GET: Donations/AddOrEdit
        public IActionResult AddOrEdit(int id = 0)
        {
            if (id == 0)
                return View(new Donation());
            else
                return View(_context.Donations.Find(id));
        }

        // POST: Donations/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> AddOrEdit([Bind("DonateID,AcccountNumber,BeneficaryName,BankName,SWIFTCode,Amount,DateTime")] Donation donation)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        if (donation.DonateID == 0)
        //        {
        //            donation.DateTime= DateTime.Now;
        //            _context.Add(donation);
        //        }
        //        else
        //            _context.Update(donation);
        //        await _context.SaveChangesAsync();
        //        return RedirectToAction(nameof(Index));
        //    }
        //    return View(donation);
        //}


        // POST: Donations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var donation = await _context.Donations.FindAsync(id);
            _context.Donations.Remove(donation);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Donation donation)
        {
            if (ModelState.IsValid)
            {
                _context.Donations.Add(donation);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(donation);
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var donation = _context.Donations.FirstOrDefault(d => d.DonateID == id);
            if (donation == null)
            {
                return NotFound();
            }

            return View(donation);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]

        public IActionResult Edit(int id, [Bind("DonateID,AcccountNumber,BeneficaryName,BankName,SWIFTCode,Amount,DateTime")] Donation donation)
        {
            if (id != donation.DonateID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(donation);
                    _context.SaveChanges();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DonationExists(donation.DonateID))
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

            return View(donation);
        }

        private bool DonationExists(int id)
        {
            return _context.Donations.Any(e => e.DonateID == id);
        }
    }
}
