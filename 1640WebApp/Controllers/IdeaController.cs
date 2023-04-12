using _1640WebApp.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Caching.Memory;
using System.Net;
using System.Net.Mail;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Threading.Tasks;


namespace _1640WebApp.Controllers
{
    [Authorize(Roles = "Staff, Admin, Manager, Coordinator")]
    
    
    public class IdeaController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IWebHostEnvironment _env;
        private readonly IMemoryCache _memoryCache;
        //private readonly ISendGridClient _sendGridClient;
        //private readonly IConfiguration _configuration;


        public IdeaController(IMemoryCache memoryCache, IHttpContextAccessor httpContextAccessor, IWebHostEnvironment env, ApplicationDbContext context = null, UserManager<ApplicationUser> userManager = null)
        {
            _context = context;
            _userManager = userManager;
            _env = env;
            _httpContextAccessor = httpContextAccessor;
            _memoryCache = memoryCache;
        }

        // GET: Ideas
        public async Task<IActionResult> Index(int? page)
        {
            int pageSize = 5;
            int pageNumber = (page ?? 1);

            var ideas = await _context.Ideas
                            .Include(i => i.Catogories)
                            .Include(i => i.Submission)
                            .Include(i => i.User)
                            .OrderByDescending(i => i.Id)
                            .Skip((pageNumber - 1) * pageSize)
                            .Take(pageSize)
                            .ToListAsync();

            var count = await _context.Ideas.CountAsync();
            var totalPages = (int)Math.Ceiling(count / (double)pageSize);

            var Catogories = await _context.Catogorys.ToListAsync();
            var Submissions = await _context.Submissions.Select(i => i.Id).Distinct().ToListAsync();
            var latestDay = _context.Ideas.Max(n => n.Datatime);
            var earliestDay = _context.Ideas.Min(n => n.Datatime);

            var allOptions = new List<SelectListItem>();
            allOptions.Add(new SelectListItem { Text = " -- Select --", Value = "" });
            foreach (var Catogory in Catogories)
            {
                allOptions.Add(new SelectListItem { Text = Catogory.Name, Value = "category=" + Catogory.Id.ToString() });
            }
            foreach (var Submission in Submissions)
            {
                allOptions.Add(new SelectListItem { Text = $"Submission Id: " + Submission, Value = "submission=" + Submission.ToString() });
            }
            ViewBag.SelectList = allOptions;
            ViewBag.Cate = Catogories;
            ViewBag.TotalPages = totalPages;
            ViewBag.CurrentPage = pageNumber;

            return View(ideas);
        }
        //ton
        //sort in admin/manager page
        public async Task<IActionResult> FilterAdmin(string typeData)
        {
            var filterTxt = typeData.Split('=')[0];

            var ideas = await _context.Ideas
                            .Include(i => i.Catogories)
                            .Include(i => i.Submission)
                            .Include(i => i.User)
                            .OrderByDescending(i => i.Id)
                            .ToListAsync();

            // Distinct() lọc kết quả trùng
            var Catogories = await _context.Catogorys.ToListAsync();
            var Submissions = await _context.Submissions.Select(i => i.Id).Distinct().ToListAsync();
            var latestDay = _context.Ideas.Max(n => n.Datatime);
            var earliestDay = _context.Ideas.Min(n => n.Datatime);

            var allOptions = new List<SelectListItem>();
            allOptions.Add(new SelectListItem { Text = " -- Select --", Value = "" });
            foreach (var Catogory in Catogories)
            {
                allOptions.Add(new SelectListItem { Text = Catogory.Name, Value = "category=" + Catogory.Id.ToString() });
            }
            foreach (var Submission in Submissions)
            {
                allOptions.Add(new SelectListItem { Text = $"Submission Id: " + Submission, Value = "submission=" + Submission.ToString() });
            }
            ViewBag.SelectList = allOptions;

            if (!String.IsNullOrEmpty(typeData))
            {
                var filter =int.Parse(typeData.Split('=')[1]);
                if (filterTxt == "category")
                {
                    ideas = await _context.Ideas
                                .Include(i => i.Catogories)
                                .Include(i => i.Submission)
                                .Include(i => i.User)
                                .Where(i => i.CatogoryId == filter)
                                .OrderByDescending(i => i.Id)                             
                                .ToListAsync();
                }
                if (filterTxt == "submission")
                {
                    ideas = await _context.Ideas
                                .Include(i => i.Catogories)
                                .Include(i => i.Submission)
                                .Include(i => i.User)
                                .Where(i => i.SubmissionId == filter)
                                .OrderByDescending(i => i.Id)
                                .ToListAsync();
                }
                var count = ideas.Count();                
                ViewBag.Cate = Catogories;
            }
            return View("Index", ideas);
        }

        //ton
        // search in admin/manager page
        public async Task<IActionResult> SearchAdmin(string searchString)
        {
            var ideas = await _context.Ideas
                            .Include(i => i.Catogories)
                            .Include(i => i.Submission)
                            .Include(i => i.User)
                            .OrderByDescending(i => i.Id)
                            .ToListAsync();
            // Distinct() lọc kết quả trùng
            var Catogories = await _context.Catogorys.ToListAsync();
            var Submissions = await _context.Submissions.Select(i => i.Id).Distinct().ToListAsync();
            var latestDay = _context.Ideas.Max(n => n.Datatime);
            var earliestDay = _context.Ideas.Min(n => n.Datatime);

            var allOptions = new List<SelectListItem>();
            allOptions.Add(new SelectListItem { Text = " -- Select --", Value = "" });
            foreach (var Catogory in Catogories)
            {
                allOptions.Add(new SelectListItem { Text = Catogory.Name, Value = "category=" + Catogory.Id.ToString() });
            }
            foreach (var Submission in Submissions)
            {
                allOptions.Add(new SelectListItem { Text = $"Submission Id: " + Submission, Value = "submission=" + Submission.ToString() });
            }
            ViewBag.SelectList = allOptions;


            if (string.IsNullOrEmpty(searchString))
            {
                return RedirectToAction(nameof(Index));
            }

            // data after search
            // .Contains() search gần giống
            var results = await _context.Ideas
                        .Include(i => i.Catogories)
                        .Include(i => i.Submission)
                        .Include(i => i.User)
                        .Where(n => n.Title.Contains(searchString) ||
                                    n.Text.Contains(searchString))
                        .OrderByDescending(n => n.Id)
                        .ToListAsync();
            var count = results.Count();

            return View("Index", results);
        }

        private int getCurrentUserDepartmentId()
        {
            // Get the current user's ID
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Get the current user's department ID
            ApplicationUser user = _context.Users.FirstOrDefault(u => u.Id == userId);
            int departmentId = user.DepartmentId;


            return departmentId;
        }

        // GET: ViewIdeas
        [HttpGet]
        [Route("ViewIdeas")]
        public async Task<IActionResult> ViewIdeas()
        {
            var user = await _userManager.GetUserAsync(User);
            var userImage = user.Image;
            var userName = user.Fullname_;
            var userEmail = user.Email;
            ViewBag.UserImage = userImage;
            ViewBag.UserName = userName;
            ViewBag.UserEmail = userEmail;

            //ton
            var newest = _context.Ideas.Max(n => n.Datatime);
            var oldest = _context.Ideas.Max(n => n.Datatime);

            var allOptions = new List<SelectListItem>();
            allOptions.Add(new SelectListItem { Text = " -- Select --", Value = "" });
            allOptions.Add(new SelectListItem { Text = "newest", Value = "newest" });
            allOptions.Add(new SelectListItem { Text = "oldest", Value = "oldest" });

            ViewBag.SelectList = allOptions;

            var applicationDbContext = await _context.Ideas
                                            .Include(i => i.Submission)
                                            .Include(i => i.Reacts)
                                            .Include(i => i.User)
                                            .OrderBy(i => i.Datatime)
                                            //.Where(i => i.CatogoryId == 0)
                                            .ToListAsync();
            // var a = ideas.Where(i => i.CatogoryId == filter) ;
            return View(applicationDbContext);
        }

        private List<Idea> applicationDbContext;
        [HttpGet]
        [Route("ViewIdeas/{submissionId}")]
        public async Task<IActionResult> ViewIdeas(int submissionId)
        {
            int departmentId = getCurrentUserDepartmentId();
            var user = await _userManager.GetUserAsync(User);

            if (user.Email.Contains("coordinator"))
            {
                applicationDbContext = _context.Ideas
                .Where(i => i.SubmissionId == submissionId && i.DepartmentId == departmentId)
                .Include(i => i.Submission)
                .Include(i => i.Reacts)
                .Include(i => i.User)
                .ToList();

                var departmentName = _context.Users
                   .Join(_context.Departments,
                       u => u.DepartmentId,
                       d => d.Id,
                       (u, d) => new { User = u, Department = d })
                   .Where(ud => ud.User.Id == user.Id)
                   .Select(ud => new
                   {
                       DepartmentName = ud.Department.Name
                   })
                   .FirstOrDefault();

                    ViewBag.DepartmentName = departmentName.DepartmentName;
            }
            else
            {
                applicationDbContext = _context.Ideas
                .Where(i => i.SubmissionId == submissionId)
                .Include(i => i.Submission)
                .Include(i => i.Reacts)
                .Include(i => i.User)
                .ToList();
            }
            

            var userImage = user.Image;
            var userName = user.Fullname_;
            var userEmail = user.Email;
            ViewBag.UserImage = userImage;
            ViewBag.UserName = userName;
            ViewBag.UserEmail = userEmail;

            //ton
            var newest = _context.Ideas.Max(n => n.Datatime);
            var oldest = _context.Ideas.Min(n => n.Datatime);

            var allOptions = new List<SelectListItem>();
            allOptions.Add(new SelectListItem { Text = " -- Select --", Value = "" });
            allOptions.Add(new SelectListItem { Text = "newest", Value = "newest" });
            allOptions.Add(new SelectListItem { Text = "oldest", Value = "oldest" });

            ViewBag.SelectList = allOptions;


            return View(applicationDbContext);
        }
        //ton
        // search in user page
        public async Task<IActionResult> Search(string searchString)
        {
            /////////////////////////////
            /// na
            var user = await _userManager.GetUserAsync(User);
            var userImage = user.Image;
            var userName = user.Fullname_;
            var userEmail = user.Email;
            ViewBag.UserImage = userImage;
            ViewBag.UserName = userName;
            ViewBag.UserEmail = userEmail;
            /////////////////////////////
            
            var a = await _context.Ideas
                            .Include(i => i.Submission)
                            .Include(i => i.Reacts)
                            .Include(i => i.User)
                            //.Include(i => i.Catogories)
                            .OrderByDescending(i => i.Id)
                            .ToListAsync();

            var newest = _context.Ideas.Max(n => n.Datatime);
            var oldest = _context.Ideas.Min(n => n.Datatime);

            var allOptions = new List<SelectListItem>();
            allOptions.Add(new SelectListItem { Text = " -- Select --", Value = "" });
            allOptions.Add(new SelectListItem { Text = "newest", Value = "newest" });
            allOptions.Add(new SelectListItem { Text = "oldest", Value = "oldest" });

            ViewBag.SelectList = allOptions;

            if (string.IsNullOrEmpty(searchString))
            {
                return View("ViewIdeas", a);
            }

            // data after search
            // .Contains() search gần giống
            var results = await _context.Ideas
                                .Include(i => i.Submission)
                                .Include(i => i.Reacts)
                                .Include(i => i.User)
                                //.Include(i => i.Catogories)
                                .Where(n => n.Title.Contains(searchString) ||
                                            n.Text.Contains(searchString))
                                .OrderByDescending(n => n.Id) 
                                .ToListAsync();

            return View("ViewIdeas", results);
        }

        //ton
        // sort in user page
        public async Task<IActionResult> Filter(string typeData)
        {
            /////////////////////////////
            /// na
            var user = await _userManager.GetUserAsync(User);
            var userImage = user.Image;
            var userName = user.Fullname_;
            var userEmail = user.Email;
            ViewBag.UserImage = userImage;
            ViewBag.UserName = userName;
            ViewBag.UserEmail = userEmail;
            /////////////////////////////
            
            var applicationDbContext = await _context.Ideas
                                            .Include(i => i.Submission)
                                            .Include(i => i.Reacts)
                                            .Include(i => i.User)
                                            .ToListAsync();


            var newest = _context.Ideas.Max(n => n.Datatime);
            var oldest = _context.Ideas.Min(n => n.Datatime);

            var allOptions = new List<SelectListItem>();
            allOptions.Add(new SelectListItem { Text = " -- Select --", Value = "" });
            allOptions.Add(new SelectListItem { Text = "newest", Value = "newest" });
            allOptions.Add(new SelectListItem { Text = "oldest", Value = "oldest" });

            ViewBag.SelectList = allOptions;

            if(string.IsNullOrEmpty(typeData))
            {
                return View("ViewIdeas", applicationDbContext);
            }
            if(typeData == "newest")
            {
                //applicationDbContext.OrderByDescending(i => i.Datatime);
                //applicationDbContext = applicationDbContext.OrderByDescending(i => i.Datatime).ToList();
                var a = await _context.Ideas
                                .Include(i => i.Submission)
                                .Include(i => i.Reacts)
                                .Include(i => i.User)
                                .OrderByDescending(i => i.Datatime)
                                .ToListAsync();
                return View("ViewIdeas", a);
            }
            if (typeData == "oldest")
            {
                //applicationDbContext = applicationDbContext.OrderBy(i => i.Datatime).ToList();
                //applicationDbContext.OrderBy(i => i.Datatime);
                var a = await _context.Ideas
                .Include(i => i.Submission)
                .Include(i => i.Reacts)
                .Include(i => i.User)
                .OrderBy(i => i.Datatime)
                .ToListAsync();
                return View("ViewIdeas", a);
            }

            return View("ViewIdeas", applicationDbContext);

        }


        // GET: Ideas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Ideas == null)
            {
                return NotFound();
            }

            var ideas = await _context.Ideas
                .Include(i => i.Submission)
                .Include(i => i.User)
                .Include(i => i.Reacts)
                .Include(i => i.Comments)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (ideas == null)
            {
                return NotFound();
            }

            

            ideas.ViewCount++;

            var user = await _userManager.GetUserAsync(User);
            var userImage = user.Image;
            ViewBag.UserImage = userImage;

            // Create a new CView instance to store the visit information
            var visit = new CView
            {
                IdeaId = ideas.Id,
                VisitTime = DateTime.Now,
                UserId = user.Id // or however you want to get the user ID
            };

            // Add the visit to the database
            _context.CViews.Add(visit);
            await _context.SaveChangesAsync();

            var comments = await _context.Comments
            .Where(c => c.IdeaId == id)
            .ToListAsync();
            ViewBag.Comments = comments;

            return View(ideas);
        }

        // GET: Ideas/Create
        //public async Task< IActionResult> Create(int submissionId, int fileId)
        //{
        //    var user = await _userManager.GetUserAsync(HttpContext.User);
        //    var currentUserId = user.Id;
        //    var departmentId = user.DepartmentId;
        //    ViewBag.SubmissionId = submissionId;
        //    ViewBag.Categories = _context.Catogorys.ToList();
        //    ViewData["UserId"] = new SelectList(new List<SelectListItem> { new SelectListItem { Value = currentUserId, Text = currentUserId } }, "Value", "Text");
        //    ViewData["DepartmentId"] = new SelectList(new List<SelectListItem> { new SelectListItem { Value = departmentId.ToString(), Text = departmentId.ToString() } }, "Value", "Text");

        //    return View();
        //}
        // GET: Ideas/Create
        public async Task<IActionResult> Create(int submissionId, int fileId)
        {
            var idea = new Idea();
            idea.Datatime = DateTime.Now;
            var user = await _userManager.GetUserAsync(HttpContext.User);
            var currentUserId = user.Id;
            var departmentId = user.DepartmentId;
            ViewBag.SubmissionId = submissionId;
            ViewBag.Categories = _context.Catogorys.ToList();
            ViewData["UserId"] = new SelectList(new List<SelectListItem> { new SelectListItem { Value = currentUserId, Text = currentUserId } }, "Value", "Text");
            ViewData["DepartmentId"] = new SelectList(new List<SelectListItem> { new SelectListItem { Value = departmentId.ToString(), Text = departmentId.ToString() } }, "Value", "Text");

            return View(idea);
        }


        // POST: Ideas/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int submissionId, Idea idea, IFormCollection form, bool anonymous, int[] categories)
        {

            if (form.Files.Count >  0)
            {
                var imgFile = form.Files.FirstOrDefault(f => f.Name == "imgInput");
                if (imgFile != null && imgFile.Length > 0)
                {
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(imgFile.FileName);
                    var imagePath = Path.Combine(_env.WebRootPath, "UserImages", fileName);
                    using (var fileStream = new FileStream(imagePath, FileMode.Create))
                    {
                        await imgFile.CopyToAsync(fileStream);
                    }
                    idea.Img = Encoding.UTF8.GetBytes(fileName);
                }

                var dataFile = form.Files.FirstOrDefault(f => f.Name == "dataInput");
                if (dataFile != null && dataFile.Length > 0)
                {
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(dataFile.FileName);
                    var dataPath = Path.Combine(_env.WebRootPath, "UserFolders", fileName);
                    using (var fileStream = new FileStream(dataPath, FileMode.Create))
                    {
                        await dataFile.CopyToAsync(fileStream);
                    }
                    idea.FileName = dataFile.FileName;
                    idea.Data = Encoding.UTF8.GetBytes(fileName);
                    idea.FilePath = Path.Combine("UserFolders", idea.FileName);
                }
            }


            var user = await _userManager.GetUserAsync(HttpContext.User);
            string currentUserId = user.Id;
            int departmentId = user.DepartmentId;

            idea.UserId = currentUserId;
            idea.DepartmentId = departmentId;
            idea.CreatorEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            //ton
            int? categoryIdSelect = categories.FirstOrDefault();
            idea.CatogoryId = categoryIdSelect;
            //
            var categoryIds = form["categories"].ToString().Split(",");
            idea.Catogories = new List<Catogory>(); // khởi tạo list categories trước khi thêm vào           
            foreach (var categoryId in categoryIds)
            {
                if (int.TryParse(categoryId, out int categoryIdInt))
                {
                    var category = _context.Catogorys.Find(categoryIdInt);
                    if (category != null)
                    {
                        idea.Catogories.Add(category);
                    }
                }
            }
            // Đăng bài ẩn danh
            // Xác định trạng thái của checkbox và gán vào thuộc tính `Anonymous` của `idea`
            idea.Anonymous = anonymous;

            var newIdea = new Idea { SubmissionId = submissionId };
            var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;


            idea.UserId = userId;

            _context.Add(idea);

            var submission = await _context.Submissions.FindAsync(idea.SubmissionId);

            if (submission == null)
            {
                // Trả về thông báo lỗi nếu Submission không tồn tại.
                return NotFound();
            }
            else if (submission.IsClosed)
            {
                // Trả về thông báo lỗi nếu Submission đã bị đóng.
                return BadRequest("Cannot add new ideas to a closed submission.");
            }

            // Thêm Idea mới vào Submission nếu Submission chưa bị đóng.
            submission.Ideas.Add(idea);
            await _context.SaveChangesAsync();

            var coordinator = _context.Users
                .Where(u => u.Email.Contains("coordinator") && u.DepartmentId == user.DepartmentId).ToList();
            var coordinatorEmail = coordinator.Select(u => u.Email);

            if (coordinator != null)
            {
                var notification = new Notification
                {
                    Title = "New Idea Added",
                    Message = $"Idea with the title \"{idea.Title}\" has been submitted."
                };

                _memoryCache.Set("IdeaNotification", notification, TimeSpan.FromMinutes(5));

                var apiKey = "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";
                var client = new SendGridClient(apiKey);
                var from = new EmailAddress("navo7036@gmail.com", "nana");
                var subject = $"A Staff named \"{user.Fullname_}\" just submitted an Idea titled \"{idea.Title}\"  ";
                var to = new EmailAddress(string.Join(",", coordinatorEmail));
                var plainTextContent = "The Idea have just submitted";
                var htmlContent = "<strong>Please check the Idea in Submission Link</strong>";
                var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
                var response = await client.SendEmailAsync(msg);

                return RedirectToAction(nameof(Index));
            }

            return RedirectToAction(nameof(Index));
        }



            // GET: Ideas/Edit/5
            public async Task<IActionResult> Edit(int? id, int submissionId)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            var currentUserId = user.Id;
            var departmentId = user.DepartmentId;
            var idea = await _context.Ideas.FindAsync(int.Parse(id?.ToString()));
            var currentUserEmail = user.Email;
            if (idea.CreatorEmail != currentUserEmail)
            {
                return Unauthorized();
            }

            if (id == null || _context.Ideas == null)
            {
                return NotFound();
            }

            /*var idea = await _context.Ideas.FindAsync(id);*/
            if (idea == null)
            {
                return NotFound();
            }
            ViewBag.SubmissionId = submissionId;
            ViewData["SubmissionId"] = new SelectList(_context.Submissions, "Id", "Id", idea.SubmissionId);

            ViewBag.Categories = _context.Catogorys.ToList();
            ViewData["UserId"] = new SelectList(new List<SelectListItem> { new SelectListItem { Value = currentUserId, Text = currentUserId } }, "Value", "Text");
            ViewData["DepartmentId"] = new SelectList(new List<SelectListItem> { new SelectListItem { Value = departmentId.ToString(), Text = departmentId.ToString() } }, "Value", "Text");
            return View(idea);
        }

        // POST: Ideas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Idea idea, IFormCollection form, bool anonymous)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            var currentUserId = user.Id;
            var departmentId = user.DepartmentId;
            idea.UserId = currentUserId;
            idea.DepartmentId = departmentId;
            if (id != idea.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                if (form.Files.Count > 0)
                {
                    // Upload hình ảnh mới
                    var imgFile = form.Files.FirstOrDefault(f => f.Name == "imgInput");
                    if (imgFile != null && imgFile.Length > 0)
                    {
                        var fileName = Guid.NewGuid().ToString() + Path.GetExtension(imgFile.FileName);
                        var imagePath = Path.Combine(_env.WebRootPath, "UserImages", fileName);
                        using (var fileStream = new FileStream(imagePath, FileMode.Create))
                        {
                            await imgFile.CopyToAsync(fileStream);
                        }
                        idea.Img = Encoding.UTF8.GetBytes(fileName);
                    }

                    // Upload file mới
                    var dataFile = form.Files.FirstOrDefault(f => f.Name == "dataInput");
                    if (dataFile != null && dataFile.Length > 0)
                    {
                        var fileName = Guid.NewGuid().ToString() + Path.GetExtension(dataFile.FileName);
                        var dataPath = Path.Combine(_env.WebRootPath, "UserFolders", fileName);
                        using (var fileStream = new FileStream(dataPath, FileMode.Create))
                        {
                            await dataFile.CopyToAsync(fileStream);
                        }
                        idea.FileName = dataFile.FileName;
                        idea.Data = Encoding.UTF8.GetBytes(fileName);
                        idea.FilePath = Path.Combine("UserFolders", idea.FileName);
                    }
                }

                // Cập nhật thông tin ý tưởng
                idea.CreatorEmail = User.FindFirst(ClaimTypes.Email)?.Value;
                var categoryIds = form["categories"].ToString().Split(",");
                idea.Catogories = new List<Catogory>();
                foreach (var categoryId in categoryIds)
                {
                    if (int.TryParse(categoryId, out int categoryIdInt))
                    {
                        var category = _context.Catogorys.Find(categoryIdInt);
                        if (category != null)
                        {
                            idea.Catogories.Add(category);
                        }
                    }
                }

                // Cập nhật thuộc tính `Anonymous`
                idea.Anonymous = anonymous;

                // Cập nhật UserId
                idea.UserId = User.FindFirst(ClaimTypes.NameIdentifier).Value;

                try
                {
                    _context.Update(idea);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Ideas.Any(e => e.IdeaId == id))
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

        // GET: Ideas/Delete/5
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
            var currentUserEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            if (idea.CreatorEmail != currentUserEmail)
            {
                return Unauthorized();
            }

            return View(idea);
        }

        // POST: Ideas/Delete/5
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
            return _context.Ideas.Any(e => e.Id == id);
        }

        // Thêm idea ở đây
        [HttpPost]
        public JsonResult AddComment(string text, int ideaId)
        {
            var comment = new Comment
            {
                Text = text,
                Datetime = DateTime.Now,
                CreatorComment = User.Identity.Name, // Lấy tên người dùng đang đăng nhập
                IdeaId = ideaId
            };

            _context.Comments.Add(comment);
            _context.SaveChanges();

            return Json(new { text = comment.Text });
        }

        public JsonResult GetRecentComments(int postId)
        {
            // Lấy danh sách 3 comment gần nhất của Idea có Id là postId
            var comments = _context.Comments.Where(c => c.IdeaId == postId).OrderByDescending(c => c.Datetime).Take(3).ToList();

            // Chuyển đổi danh sách comment sang định dạng JSON và trả về cho client
            var commentData = comments.Select(c => new {
                text = c.Text,
                datetime = String.Format("{0:dd/MM/yyyy HH:mm}", c.Datetime)
            }).ToList();

            return new JsonResult(commentData);
        }


        

        [HttpPost]
        public IActionResult UpdateReaction(int ideaId, int reaction)
        {
            // Lấy thông tin người dùng hiện tại
            var currentUser = _userManager.GetUserAsync(HttpContext.User).Result;

            // Kiểm tra xem người dùng đã đăng nhập hay chưa
            if (currentUser == null)
            {
                return Unauthorized();
            }

            // Lấy thông tin Idea từ cơ sở dữ liệu
            var idea = _context.Ideas.Find(ideaId);

            if (idea == null)
            {
                return NotFound();
            }

            // Lấy thông tin React từ cơ sở dữ liệu
            var react = _context.Reacts
                .FirstOrDefault(r => r.IdeaId == ideaId && r.UserId == currentUser.Id);

            // Nếu React chưa tồn tại trong cơ sở dữ liệu, tạo mới React và lưu vào cơ sở dữ liệu
            if (react == null)
            {
                react = new React
                {
                    IdeaId = ideaId,
                    UserId = currentUser.Id,
                    Reaction = reaction,
                    CreatorComment = User.Identity.Name,
                    TotalReacts = 1
                };

                _context.Reacts.Add(react);
            }
            else // Ngược lại, cập nhật thông tin React và lưu vào cơ sở dữ liệu
            {
                react.Reaction = reaction;
                react.TotalReacts++;
                _context.Reacts.Update(react);
            }

            _context.SaveChanges();

            // Trả về kết quả cho client
            var likeCount = _context.Reacts.Count(r => r.IdeaId == ideaId && r.Reaction == 1);
            var loveCount = _context.Reacts.Count(r => r.IdeaId == ideaId && r.Reaction == 2);
            var hahaCount = _context.Reacts.Count(r => r.IdeaId == ideaId && r.Reaction == 3);
            var dislikeCount = _context.Reacts.Count(r => r.IdeaId == ideaId && r.Reaction == 4);

            var result = new
            {
                totalReacts = idea.Reacts.Count,
                likeCount = likeCount,
                loveCount = loveCount,
                hahaCount = hahaCount,
                dislikeCount = dislikeCount
            };

            return Json(result);
        }

        // Hàm trợ giúp để lấy ID của nút reaction tương ứng
        private string GetReactionButtonId(int reaction, int ideaId)
        {
            switch (reaction)
            {
                case 1:
                    return $"like-{ideaId}";
                case 2:
                    return $"love-{ideaId}";
                case 3:
                    return $"haha-{ideaId}";
                case 4:
                    return $"dislike-{ideaId}";
                default:
                    return "";
            }
        }

        

        // GET: ViewIdeas Nếu muốn một ideas theo id của nó
        public async Task<IActionResult> ViewIdeaOnly(int ideaId)
        {
            var idea = await _context.Ideas
                .Include(i => i.Submission)
                .Include(i => i.Reacts)
                .Include(i => i.User)
                .FirstOrDefaultAsync(i => i.Id == ideaId);

            if (idea == null)
            {
                return NotFound();
            }

            var ideas = new List<Idea> { idea };
            return View(ideas);
        }

        public async Task<IActionResult> Music()
        {
            var ideas = _context.Ideas.Include(i => i.Catogories).Include(i => i.Submission).Include(i => i.User);

            return View(await ideas.ToListAsync());
        }

        public async Task<IActionResult> Game()
        {
            var ideas = _context.Ideas.Include(i => i.Catogories).Include(i => i.Submission).Include(i => i.User);

            return View(await ideas.ToListAsync());
        }




    }
}