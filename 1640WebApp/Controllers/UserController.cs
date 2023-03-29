using _1640WebApp.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace _1640WebApp.Controllers
{

    [Authorize(Roles = "Admin")]
    public class UserController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        public UserController(ApplicationDbContext context = null, UserManager<ApplicationUser> userManager = null)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var users = await _userManager.Users.ToListAsync();
            return View(users);
        }

        public async Task<IActionResult> Edit(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            var departments = await _context.Departments.ToListAsync();
            if (user == null)
            {
                return NotFound();
            }
            var model = new ApplicationUser
            {
                StaffNumber = user.StaffNumber,
                Fullname_ = user.Fullname_,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                DepartmentId = user.DepartmentId,
                Departments = departments,
                
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(ApplicationUser model)
        {
            var user = await _userManager.FindByIdAsync(model.Id);
            var departments = await _context.Departments.ToListAsync();
            if (user == null)
            {
                return NotFound();
            }
            user.StaffNumber = model.StaffNumber;
            user.Fullname_ = model.Fullname_;
            user.Email = model.Email;
            user.PhoneNumber = model.PhoneNumber;
            user.DepartmentId = model.DepartmentId;
            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "The error occured white updating the user");
                return View(model);
            }
            return RedirectToAction("Index");
        }

        
        public async Task<IActionResult> Delete(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "The error occured while deleting the user");
                return RedirectToAction("Index");
            }
            return RedirectToAction("Index");
        }
    }
    
}
