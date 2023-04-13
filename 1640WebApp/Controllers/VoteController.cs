using _1640WebApp.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Crypto;
using System.Diagnostics;
//using _1640WebApp.Models;

namespace _1640WebApp.Controllers
{

    public class VoteController : Controller
    {

        private readonly ApplicationDbContext _context;

        public VoteController(ApplicationDbContext context)
        {
            _context = context;
        }
        private bool VoteExists(int idVote)
        {
            return _context.Votes.Any(e => e.IdVote == idVote);
        }

        // GET: VoteController
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> Index()
        {
            return View(await _context.Votes.ToListAsync());
        }

        // GET: VoteController/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Votes == null)
            {
                return NotFound();
            }

            var vote = await _context.Votes.FirstOrDefaultAsync(m => m.IdVote == id);
            if (vote == null)
            {
                return NotFound();
            }

            return View(vote);


        }

        // GET: Submissions/Create
        [Authorize(Roles = "Admin")]

        public IActionResult Create()
        {
            return View();
        }


        [Authorize(Roles = "Admin")]
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Vote vote)
        {
            if (ModelState.IsValid)
            {
                _context.Votes.Add(vote);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(vote);
        }

        [Authorize(Roles = "Admin")]

        // GET: VoteController/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Votes == null)
            {
                return NotFound();
            }

            var vote = await _context.Votes.FindAsync(id);
            if (vote == null)
            {
                return NotFound();
            }
            return View(vote);
        }

        // POST: VoteController/Edit/5
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Edit(int id, [Bind("IdVote,Title,Description,CreatedDate,ClosedDate,Option1,Option2,Option3,Option4")] Vote vote)
        {
            if (id != vote.IdVote)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(vote);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VoteExists(vote.IdVote))
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
            return View(vote);
        }




        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> Delete(int id)
        {
            var vote = await _context.Votes.FindAsync(id);
            if (vote == null)
            {
                return NotFound();
            }

            _context.Votes.Remove(vote);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        public async Task<IActionResult> ViewVote()
        {
            return View(await _context.Votes.ToListAsync());
        }

        public IActionResult ViewVoteDetail(int id)
        {
            var vote = _context.Votes
                .Where(d => d.IdVote == id)
                .Select(d => new {
                    IdVote = d.IdVote,
                    Title = d.Title,
                    Description = d.Description,
                    CreatedDate = d.CreatedDate,
                    ClosedDate = d.ClosedDate,
                    Option1 = d.Option1,
                    Option2 = d.Option2,
                    Option3 = d.Option3,
                    Option4 = d.Option4,

                })
                .ToList<object>();

            return View(vote.Cast<object>().ToList());
        }

        [HttpPost]
        public async Task<IActionResult> ViewVoteDetail(int id, int option)
        {


            var vote = await _context.Votes
                 .FirstOrDefaultAsync(m => m.IdVote == id);
            if (vote == null)
            {
                return NotFound();
            }
            switch (option)
            {
                case 1:
                    vote.Option1Count++;
                    break;
                case 2:
                    vote.Option2Count++;
                    break;
                case 3:
                    vote.Option3Count++;
                    break;
                case 4:
                    vote.Option4Count++;
                    break;
            }

            await _context.SaveChangesAsync();


            return RedirectToAction("Index");
        }

        public async Task<IActionResult> ViewVoteAdmin(int id)
        {
            var vote = _context.Votes
                 .Where(d => d.IdVote == id)
                 .Select(d => new {
                     IdVote = d.IdVote,
                     Title = d.Title,
                     Description = d.Description,
                     CreatedDate = d.CreatedDate,
                     ClosedDate = d.ClosedDate,
                     Option1 = d.Option1,
                     Option2 = d.Option2,
                     Option3 = d.Option3,
                     Option4 = d.Option4,
                     Option1Count = d.Option1Count,
                     Option2Count = d.Option2Count,
                     Option3Count = d.Option3Count,
                     Option4Count = d.Option4Count,


                 })
                 .ToList<object>();

            return View(vote.Cast<object>().ToList());
        }

    }
}
