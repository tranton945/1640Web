using _1640WebApp.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace _1640WebApp.Controllers
{
    
    [Authorize(Roles = "Staff, Admin, Manager, Coordinator")]

    public class SubmissionController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SubmissionController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Submissions
        public async Task<IActionResult> Index()
        {
            return View(await _context.Submissions.ToListAsync());
        }

        // GET: Submissions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Submissions == null)
            {
                return NotFound();
            }

            var submission = await _context.Submissions
                .FirstOrDefaultAsync(m => m.Id == id);
            if (submission == null)
            {
                return NotFound();
            }

            return View(submission);
        }

        // GET: Submissions/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Submissions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Description,ClosureDate,FinalClosureTime")] Submission submission)
        {
            //if (ModelState.IsValid)
            {
                _context.Add(submission);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(submission);
        }

        // GET: Submissions/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Submissions == null)
            {
                return NotFound();
            }

            var submission = await _context.Submissions.FindAsync(id);
            if (submission == null)
            {
                return NotFound();
            }
            return View(submission);
        }

        // POST: Submissions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,ClosureDate,FinalClosureTime")] Submission submission)
        {
            if (id != submission.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(submission);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SubmissionExists(submission.Id))
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
            return View(submission);
        }
        //-----------------------------------------------------
        // GET: Submissions/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Submissions == null)
            {
                return NotFound();
            }

            var submission = await _context.Submissions
                .FirstOrDefaultAsync(m => m.Id == id);
            if (submission == null)
            {
                return NotFound();
            }

            return View(submission);
        }

        // POST: Submissions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Submissions == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Submissions'  is null.");
            }
            var submission = await _context.Submissions.FindAsync(id);
            if (submission != null)
            {
                _context.Submissions.Remove(submission);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SubmissionExists(int id)
        {
            return _context.Submissions.Any(e => e.Id == id);
        }

        // GET: Submissions/ViewIdeas/5
        public IActionResult ViewIdeas(int SubmissionId)
        {
            Idea idea = new Idea { Id = SubmissionId };
            // Validate the idea object
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            // Add the idea to the repository
            _context.Ideas.Add(idea);
            /*_context.SaveChanges();*/

            return View(idea);
        }


        //------------------------------------------------------
        // GET: Submissions/AddIdeas/1
        public IActionResult AddIdeas(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var submission = _context.Submissions.Include(s => s.Ideas).FirstOrDefault(s => s.Id == id);

            if (submission == null)
            {
                return NotFound();
            }

            return View(submission);
        }

        // POST: Submissions/AddIdeas/1
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddIdeas(int id, [Bind("Id,Title,Text,FilePath,Datatime,CategoryId,UserId,SubmissionId")] Idea idea)
        {
            if (id != idea.SubmissionId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                _context.Add(idea);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(AddIdeas), new { id });
            }

            var submission = _context.Submissions.Include(s => s.Ideas).FirstOrDefault(s => s.Id == id);

            if (submission == null)
            {
                return NotFound();
            }

            return View(submission);
        }



    }
}
