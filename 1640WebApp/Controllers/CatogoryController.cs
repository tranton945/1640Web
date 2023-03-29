using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using _1640WebApp.Data;
using Microsoft.EntityFrameworkCore;



namespace _1640WebApp.Controllers
{
    [Authorize(Roles = "Admin")]
    public class CatogoryController : Controller
    {
        private readonly ApplicationDbContext _context;
        public CatogoryController(ApplicationDbContext context)
        {
            _context = context;
        }
        private bool CatogoryExists(int id)
        {
            return _context.Catogorys.Any(e => e.Id == id);
        }

        public IActionResult Index()
        {
            var catogorys = _context.Catogorys.ToList();
            return View(catogorys);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Catogory catogory)
        {
            if (ModelState.IsValid)
            {
                _context.Catogorys.Add(catogory);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(catogory);
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var catogory = _context.Catogorys.FirstOrDefault(d => d.Id == id);
            if (catogory == null)
            {
                return NotFound();
            }

            return View(catogory);
        }

        [HttpPost]
        public IActionResult Edit(int id, [Bind("Id,Name")] Catogory catogory)
        {
            if (id != catogory.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(catogory);
                    _context.SaveChanges();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CatogoryExists(catogory.Id))
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

            return View(catogory);
        }

       
        public async Task<IActionResult> Delete(int id)
        {
            var catogory = await _context.Catogorys.FindAsync(id);
            if (catogory == null)
            {
                return NotFound();
            }

            _context.Catogorys.Remove(catogory);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


    }
}
