using _1640WebApp.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;

namespace _1640WebApp.Controllers
{
    [Authorize(Roles = "Manager, Admin")]
    public class SuperUserController : Controller
    {
        private readonly RoleManager<IdentityRole> roleManager;

        public SuperUserController( RoleManager<IdentityRole> roleManager)
        {
            this.roleManager = roleManager;
        }
        public IActionResult Index()
        {
            var roles = roleManager.Roles.ToList();
            return View(roles);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(AppRole role)
        {
            var roleCreated = await roleManager.RoleExistsAsync(role.UserRole);
            if (!roleCreated)
            {
                var result = await roleManager.CreateAsync(new IdentityRole(role.UserRole));
                return RedirectToAction(nameof(Index));
            }
            return View();
        }

        public async Task<IActionResult> Edit(string id)
        {
            var role = await roleManager.FindByIdAsync(id);
            if (role == null)
            {
                return NotFound();
            }
            var model = new AppRole
            {
                ID = role.Id,
                UserRole = role.Name
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(AppRole model)
        {
            var role = await roleManager.FindByIdAsync(model.ID);
            if (role == null)
            {
                return NotFound();
            }
            role.Name = model.UserRole;
            var result = await roleManager.UpdateAsync(role);
            if(!result.Succeeded)
            {
                ModelState.AddModelError("", "The error occured white updating the role");
                return View(model);
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            var role = await roleManager.FindByIdAsync(id);
            if (role == null)
            {
                return NotFound();
            }

            var result = await roleManager.DeleteAsync(role);
            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "The error occured white deleting the role");
            }
            return RedirectToAction("Index");
        }
    }
}
