using _1640WebApp.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace _1640WebApp.Controllers
{
    [Authorize(Roles = "Admin")]
    public class DepartmentController : Controller
    {
        private readonly ApplicationDbContext _context;
        public DepartmentController(ApplicationDbContext context)
        {
            _context = context;
        }
        private bool DepartmentExists(int id)
        {
            return _context.Departments.Any(e => e.Id == id);
        }
        public IActionResult Index()
        {
            
            var departmentUsers = _context.Departments
            .Select(d => new {
                DepartmentId = d.Id,
                DepartmentName = d.Name,
                UserCount = _context.Users.Count(u => u.DepartmentId == d.Id),
                CoordinatorCount = _context.Users
                .Join(_context.UserRoles,
                    u => u.Id,
                    ur => ur.UserId,
                    (u, ur) => new { User = u, UserRole = ur })
                .Join(_context.Roles,
                    ur => ur.UserRole.RoleId,
                    r => r.Id,
                    (ur, r) => new { User = ur.User, RoleName = r.Name })
                .Count(ur => ur.User.DepartmentId == d.Id && ur.RoleName == "Coordinator"),
                StaffCount = _context.Users
                .Join(_context.UserRoles,
                    u => u.Id,
                    ur => ur.UserId,
                    (u, ur) => new { User = u, UserRole = ur })
                .Join(_context.Roles,
                    ur => ur.UserRole.RoleId,
                    r => r.Id,
                    (ur, r) => new { User = ur.User, RoleName = r.Name })
                .Count(ur => ur.User.DepartmentId == d.Id && ur.RoleName == "Staff"),
                IdeaCount = _context.Ideas
                .Count(i => i.DepartmentId == d.Id)
            })
            .ToList<object>();

            return View(departmentUsers);
        }

        [HttpGet] 
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Department department)
        {
            if (ModelState.IsValid)
            {
                _context.Departments.Add(department);   
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(department);
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var department = _context.Departments.FirstOrDefault(d => d.Id == id);
            if (department == null)
            {
                return NotFound();
            }

            return View(department);
        }

        [HttpPost]
        public IActionResult Edit(int id, [Bind("Id,Name")] Department department)
        {
            if (id != department.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(department);
                    _context.SaveChanges();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DepartmentExists(department.Id))
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

            return View(department);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> Delete(int id)
        {
            var department = await _context.Departments.FindAsync(id);
            if (department == null)
            {
                return NotFound();
            }

            _context.Departments.Remove(department);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
       

        public IActionResult Detail(int id)
        {
            var coordinators = _context.Users
            .Join(_context.UserRoles,
                u => u.Id,
                ur => ur.UserId,
                (u, ur) => new { User = u, UserRole = ur })
            .Join(_context.Roles,
                ur => ur.UserRole.RoleId,
                r => r.Id,
                (ur, r) => new { User = ur.User, RoleName = r.Name })
            .Where(ur => ur.User.DepartmentId == id && ur.RoleName == "Coordinator")
            .Select(ur => new {
                UserImage = ur.User.Image,
                UserNumber = ur.User.StaffNumber,
                UserName = ur.User.Fullname_,
                UserEmail = ur.User.Email,
                UserPhoneNumber = ur.User.PhoneNumber,
                IdeaCount = _context.Ideas.Count(i => i.UserId == ur.User.Id)
            })
            .ToList();

            var staffs = _context.Users
            .Join(_context.UserRoles,
                u => u.Id,
                ur => ur.UserId,
                (u, ur) => new { User = u, UserRole = ur })
            .Join(_context.Roles,
                ur => ur.UserRole.RoleId,
                r => r.Id,
                (ur, r) => new { User = ur.User, RoleName = r.Name })
            .Where(ur => ur.User.DepartmentId == id && ur.RoleName == "Staff")
            .Select(ur => new {
                UserImage = ur.User.Image,
                UserNumber = ur.User.StaffNumber,
                UserName = ur.User.Fullname_,
                UserEmail = ur.User.Email,
                UserPhoneNumber = ur.User.PhoneNumber,
                IdeaCount = _context.Ideas.Count(i => i.UserId == ur.User.Id)
            })
            .ToList();

            

            var department = _context.Departments
                .Where(d => d.Id == id)
                .Select(d => new {
                    DepartmentId = d.Id,
                    DepartmentName = d.Name,
                    UserCount = _context.Users.Count(u => u.DepartmentId == d.Id),
                    CoordinatorCount = coordinators.Count,
                    StaffCount = staffs.Count,
                    IdeaCount = _context.Ideas.Count(i => i.DepartmentId == d.Id)
                })
                //.SingleOrDefault();
                .ToList<object>();


            var departmentIdeas = _context.Ideas
                .Join(_context.Submissions,
                    i => i.SubmissionId,
                    s => s.Id,
                    (i, s) => new { Idea = i, SubmissionName = s.Name })
               
                .Join(_context.Users,
                    i => i.Idea.UserId,
                    u => u.Id,
                    (i, u) => new { i.Idea, i.SubmissionName,  StaffNumber = u.StaffNumber })
                .Where(i => i.Idea.DepartmentId == id)
                .Select(i => new {
                    Title =  i.Idea.Title,
                    Text = i.Idea.Text,
                    FilePath = i.Idea.FilePath,
                    Datatime = i.Idea.Datatime,
                    StaffNumber = i.StaffNumber,
                    SubmissionName = i.SubmissionName,
                    
                    
                })
                .ToList();


            if (department == null)
            {
                return NotFound();
            }


            ViewData["Coordinators"] = coordinators.Cast<dynamic>().ToList();
            ViewData["Staffs"] = staffs.Cast<dynamic>().ToList();
            ViewData["DepartmentIdeas"] = departmentIdeas.Cast<dynamic>().ToList();


            //return View(department);
            return View(department.Cast<object>().ToList());
        }


    }
}
