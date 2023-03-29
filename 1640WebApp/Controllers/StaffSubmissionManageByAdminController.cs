using _1640WebApp.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace _1640WebApp.Controllers
{
    [Authorize(Roles = "Admin")]
    public class StaffSubmissionManageByAdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public StaffSubmissionManageByAdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var submissions = await _context.Submissions.ToListAsync();
            return View(submissions);
        }
    }
}
