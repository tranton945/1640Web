using _1640WebApp.Data;
using Ionic.Zip;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Crypto;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Text;
using CsvHelper;
using CsvHelper.Configuration;
using System.Linq;
using System.Threading.Tasks;


namespace _1640WebApp.Controllers
{
    [Authorize(Roles = "Admin")]
    public class SubmissionManageByAdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public SubmissionManageByAdminController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }
        public async Task<IActionResult> Index()
        {
            var submissions = await _context.Submissions.ToListAsync();
            return View(submissions);
        }

        // downloda zip
        public async Task<IActionResult> DownloadZip(int id) 
        {
            var ideas = _context.Ideas.Where(i => i.SubmissionId == id).ToList();

            var fileDataList = new List<(string Name, byte[]? Data)>();

            foreach (var idea in ideas)
            {
                if (idea.Data != null)
                {
                    var name = idea.FileName;
                    var filePath = Path.Combine(_webHostEnvironment.WebRootPath, "UserFolders", Encoding.UTF8.GetString(idea.Data));
                    var fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);
                    var extension = Path.GetExtension(idea.FilePath);
                    var fName = $"file_{DateTime.Now:yyyyMMddHHmmssffff}{extension}";
                    fileDataList.Add((fName, fileBytes));
                }
            }
            using (var ms = new MemoryStream())
            {
                using (var zip = new Ionic.Zip.ZipFile())
                {
                    zip.UseZip64WhenSaving = Zip64Option.AsNecessary;
                    foreach (var fileData in fileDataList)
                    {
                        zip.AddEntry(fileData.Name, fileData.Data);
                    }
                    zip.Save(ms);
                }
                var fileName = $"submission_{id}_files.zip";
                return File(ms.ToArray(), "application/zip", fileName);
            }
        }

        // CSV
        public async Task<IActionResult> DownloadCSV(int id)
        {
            var ideas = await _context.Ideas
                        .Where(i => i.SubmissionId == id)
                        .Select(i => new {
                            i.Id,
                            i.Title,
                            i.Text,
                            i.FilePath,
                            i.Datatime,
                            i.Img,
                            i.Data,
                            i.Anonymous,
                            i.IdeaId,
                            i.CreatorEmail,
                            i.FileName,
                            i.UserId,
                            i.DepartmentId,
                            i.ViewCount
                        })
                        .ToListAsync();


            var CsvConfig = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                // Cấu hình các thiết lập cho file CSV
                // Delimiter nên dùng dấu "," để định dạng các trường là một cột
                Delimiter = ",",
                HasHeaderRecord = true,
                Quote = '"'
            };
            using var writer = new StringWriter();
            using var csv = new CsvWriter(writer, CsvConfig);

            // Ghi dữ liệu cho file CSV
            csv.WriteRecords(ideas);

            // Ghi header cho file CSV
            csv.WriteHeader<Idea>();
            foreach (var idea in ideas)
            {
                csv.WriteRecord(idea);
            }
            var sub = await _context.Submissions.FirstOrDefaultAsync(o => o.Id == id);
            var Name = sub?.Name + ".csv";
            // dùng Replace để đổi các khoảng trắng " " thành "_"
            var fileName = Name.Replace(" ", "_");
            if (Name == null)
            {
                return File(new MemoryStream(writer.Encoding.GetBytes(writer.ToString())), "text/csv", "ideas.csv");
            }
            // Trả về file CSV
            return File(new MemoryStream(writer.Encoding.GetBytes(writer.ToString())), "text/csv", fileName);
        }


        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Submission submission)
        {
            if (ModelState.IsValid)
            {
                _context.Add(submission);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(submission);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var submission = await _context.Submissions.FindAsync(id);
            if (submission == null)
            {
                return NotFound();
            }
            return View(submission);
        }
        private bool SubmissionExists(int id)
        {
            return _context.Submissions.Any(e => e.Id == id);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, Submission submission)
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
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> Delete(int id)
        {
            var submission = await _context.Submissions.FindAsync(id);
            if (submission == null)
            {
                return NotFound();
            }

            _context.Submissions.Remove(submission);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        
        public async Task<IActionResult> Close(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var submission = await _context.Submissions.FindAsync(id);
            if (submission == null)
            {
                return NotFound();
            }

            submission.IsClosed = true;
            _context.Update(submission);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

    }
}
