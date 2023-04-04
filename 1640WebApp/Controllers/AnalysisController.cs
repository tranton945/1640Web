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
    public class AnalysisController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AnalysisController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Analysis
        public async Task<IActionResult> Index()
        {
            var idea = await _context.Ideas.ToListAsync();
            var user = await _context.Users.ToListAsync();
            var todaySubIdea = await _context.Ideas
                                    .Where(i => i.Datatime.Date == DateTime.Today.Date)
                                    .ToListAsync();
            var totlaSubmission = await _context.Submissions.ToListAsync();
            var a = "qqqqq";
            ViewBag.totalUser = user.Count;
            ViewBag.todaySubIdea = todaySubIdea.Count;
            ViewBag.totalSubIdea = idea.Count;
            ViewBag.totlaSubmission = totlaSubmission.Count;

            var applicationDbContext = _context.Ideas.Include(i => i.Submission).Include(i => i.User);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Analysis/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Ideas == null)
            {
                return NotFound();
            }

            var idea = await _context.Ideas
                .Include(i => i.Submission)
                .Include(i => i.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (idea == null)
            {
                return NotFound();
            }

            return View(idea);
        }

        // GET: Analysis/Create
        public IActionResult Create()
        {
            ViewData["SubmissionId"] = new SelectList(_context.Submissions, "Id", "Id");
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id");
            return View();
        }

        // POST: Analysis/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Text,FilePath,Datatime,Img,Data,Anonymous,IdeaId,CreatorEmail,FileName,UserId,DepartmentId,SubmissionId,CatogoryId,ViewCount")] Idea idea)
        {
            if (ModelState.IsValid)
            {
                _context.Add(idea);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["SubmissionId"] = new SelectList(_context.Submissions, "Id", "Id", idea.SubmissionId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", idea.UserId);
            return View(idea);
        }

        // GET: Analysis/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Ideas == null)
            {
                return NotFound();
            }

            var idea = await _context.Ideas.FindAsync(id);
            if (idea == null)
            {
                return NotFound();
            }
            ViewData["SubmissionId"] = new SelectList(_context.Submissions, "Id", "Id", idea.SubmissionId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", idea.UserId);
            return View(idea);
        }

        // POST: Analysis/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Text,FilePath,Datatime,Img,Data,Anonymous,IdeaId,CreatorEmail,FileName,UserId,DepartmentId,SubmissionId,CatogoryId,ViewCount")] Idea idea)
        {
            if (id != idea.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(idea);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!IdeaExists(idea.Id))
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
            ViewData["SubmissionId"] = new SelectList(_context.Submissions, "Id", "Id", idea.SubmissionId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", idea.UserId);
            return View(idea);
        }

        // GET: Analysis/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Ideas == null)
            {
                return NotFound();
            }

            var idea = await _context.Ideas
                .Include(i => i.Submission)
                .Include(i => i.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (idea == null)
            {
                return NotFound();
            }

            return View(idea);
        }

        // POST: Analysis/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Ideas == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Ideas'  is null.");
            }
            var idea = await _context.Ideas.FindAsync(id);
            if (idea != null)
            {
                _context.Ideas.Remove(idea);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool IdeaExists(int id)
        {
          return (_context.Ideas?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
