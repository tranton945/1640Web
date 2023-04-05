using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using _1640WebApp.Data;
using Microsoft.Ajax.Utilities;
using Microsoft.AspNetCore.Authorization;
using System.Globalization;

namespace _1640WebApp.Controllers
{
    [Authorize(Roles = "Admin, Manager")]
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

            //====================================
            // tạo data cho pink chart
            // Lấy ngày đầu tiên của tuần hiện tại
            DateTime startOfWeek = DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek);

            // Lọc các idea với ngày tạo trong tuần
            List<Idea> ideas = _context.Ideas
                .Where(i => i.Datatime >= startOfWeek && i.Datatime < startOfWeek.AddDays(7))
                .ToList();

            // Tạo mảng salesData từ list idea ở trên
            int[] ideaData = new int[7];
            for (int i = 0; i < 7; i++)
            {
                DateTime dayOfWeek = startOfWeek.AddDays(i);
                int count = ideas.Count(i => i.Datatime.Date == dayOfWeek.Date);
                ideaData[i] = count;
            }
            ViewBag.IdeaData = ideaData;
            //lấy tên các ngày dựa theo setting ngày của hệ thống
            // AbbreviatedDayNames lấy tên ngày ngắn gọn
            // DayNames lấy tên ngày đầy đủ
            ViewBag.DayNames = System.Globalization.DateTimeFormatInfo.CurrentInfo.AbbreviatedDayNames;
            
            //=================================

            //=================================
            // idea in month
            var ideasByMonth = _context.Ideas
                .GroupBy(i => new { i.Datatime.Year, i.Datatime.Month })
                .Select(g => new { Month = g.Key.Month, Count = g.Count() })
                .ToList();

            var currentDate = DateTime.Today;
            var dataIdeas = new int[12];

            for (int i = 0; i < 12; i++)
            {
                var month = i + 1;
                var year = currentDate.Year;
                var count = ideasByMonth.FirstOrDefault(m => m.Month == month)?.Count ?? 0;
                dataIdeas[i] = count;
            }
            ViewBag.IdeaInMonth = dataIdeas;
            // dùng where vì trong cách select DateTimeFormatInfo.CurrentInfo.AbbreviatedMonthNames
            // kết quả là mảng 13 giá trị với giá trị cuối là rỗng
            var MonthNames = DateTimeFormatInfo.CurrentInfo.AbbreviatedMonthNames
                    .Where(m => !string.IsNullOrEmpty(m)).Take(12).ToArray();
            ViewBag.MonthNames = MonthNames;
            //=================================

            //=================================
            // Completed Submission
            var submissionByMonth = _context.Submissions
                .Where(s => s.FinalClosureTime != null)
                .GroupBy(s => new { s.FinalClosureTime.Value.Year, s.FinalClosureTime.Value.Month })
                .Select(g => new { Month = g.Key.Month, Count = g.Count() })
                .ToList();
            // code select tương tự như IdeaInMonth nhưng FinalClosureTime có kiểu dữ liệu DateTime? cho phép null nên dùng s.FinalClosureTime.Value.Year
            var dataSubmission = new int[12];
            for (int i = 0; i < 12; i++)
            {
                var month = i + 1;
                var year = currentDate.Year;
                var count = submissionByMonth.FirstOrDefault(m => m.Month == month)?.Count ?? 0;
                dataSubmission[i] = count;
            }
            ViewBag.SubmissionInMonth = dataSubmission;

            //=================================

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
