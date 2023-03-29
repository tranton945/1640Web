using _1640WebApp.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;

namespace _1640WebApp.Controllers
{
    [Authorize(Roles = "Coordinator")]
    public class StaffManageByCoordinatorController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ApplicationDbContext _context;
        private readonly IMemoryCache _memoryCache;
        public StaffManageByCoordinatorController(IMemoryCache memoryCache, IHttpContextAccessor httpContextAccessor, ApplicationDbContext context = null, UserManager<ApplicationUser> userManager = null)
        {
            _context = context;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
            _memoryCache = memoryCache;
           
        }
        public async Task< IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);

            var departmentName = _context.Users
                .Join(_context.Departments,
                    u => u.DepartmentId,
                    d => d.Id,
                    (u, d) => new { User = u, Department = d })
                .Where(ud => ud.User.Id == user.Id)
                .Select(ud => new {
                    UserName = ud.User.Fullname_,
                    DepartmentName = ud.Department.Name
                })
                .FirstOrDefault();

            var staffs = await _context.Users
                .Where(u => u.DepartmentId == user.DepartmentId)
                .Select(u => new {
                    User = u,
                    IdeaCount = _context.Ideas.Count(i => i.UserId == u.Id)
                })
                .ToListAsync();

            var staffsWithRoles = new List<dynamic>();

            foreach (var staff in staffs)
            {
                var roles = await _userManager.GetRolesAsync(staff.User);
                if (roles.Contains("Staff"))
                {
                    staffsWithRoles.Add(new
                    {
                        UserNumber = staff.User.StaffNumber,
                        UserName = staff.User.Fullname_,
                        UserEmail = staff.User.Email,
                        UserPhoneNumber = staff.User.PhoneNumber,
                        IdeaCount = staff.IdeaCount
                    });
                }
            }



            var departmentIdeas = _context.Ideas
               .Join(_context.Submissions,
                   i => i.SubmissionId,
                   s => s.Id,
                   (i, s) => new { Idea = i, SubmissionName = s.Name })

               .Join(_context.Users,
                   i => i.Idea.UserId,
                   u => u.Id,
                   (i, u) => new { i.Idea, i.SubmissionName, StaffNumber = u.StaffNumber })
               .Where(i => i.Idea.DepartmentId == user.DepartmentId)
               .Select(i => new {
                   Title = i.Idea.Title,
                   Text = i.Idea.Text,
                   FilePath = i.Idea.FilePath,
                   Datatime = i.Idea.Datatime,
                   StaffNumber = i.StaffNumber,
                   SubmissionName = i.SubmissionName,


               })
               .ToList();

            var labels = staffsWithRoles.Select(s => s.UserName).ToArray();
            var data = staffsWithRoles.Select(s => s.IdeaCount).ToArray();
            var colors = Enumerable.Range(0, staffsWithRoles.Count).Select(i => $"hsl({i * 360 / staffsWithRoles.Count}, 70%, 50%)").ToArray();

            ViewData["StaffsWithIdeaCount"] = new
            {
                Labels = labels,
                Data = data,
                Colors = colors
            };

            ViewBag.DepartmentName = departmentName.DepartmentName;


            ViewData["DepartmentIdeas"] = departmentIdeas.Cast<dynamic>().ToList();

            
            return View(staffsWithRoles);
        }

        public async Task<IActionResult> ViewSubmission()
        {
            int departmentId = getCurrentUserDepartmentId();

            var user = await _userManager.GetUserAsync(User);

            var departmentName = _context.Users
                .Join(_context.Departments,
                    u => u.DepartmentId,
                    d => d.Id,
                    (u, d) => new { User = u, Department = d })
                .Where(ud => ud.User.Id == user.Id)
                .Select(ud => new {
                    DepartmentName = ud.Department.Name
                })
                .FirstOrDefault();

            var submissions = await _context.Submissions.ToListAsync();
            ViewBag.DepartmentName = departmentName.DepartmentName;

            //var title = HttpContext.Session.GetString("IdeaNotificationTitle");
            //var message = HttpContext.Session.GetString("IdeaNotificationMessage");

            //// Xóa các giá trị đã lưu trong Session
            //ViewBag.IdeaNotificationTitle = _httpContextAccessor.HttpContext.Session.GetString("IdeaNotificationTitle");
            //ViewBag.IdeaNotificationMessage = _httpContextAccessor.HttpContext.Session.GetString("IdeaNotificationMessage");

            var notification = _memoryCache.Get<Notification>("IdeaNotification");

            if (notification != null)
            {
                TempData["IdeaNotificationTitle"] = notification.Title;
                TempData["IdeaNotificationMessage"] = notification.Message;

                // Xóa thông báo khỏi bộ nhớ cache để ngăn không cho hiển thị lại nhiều lần
                _memoryCache.Remove("IdeaNotification");
            }



            return View(submissions);
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


        public async Task<IActionResult> ViewIdeas(int id)
        {
            int departmentId = getCurrentUserDepartmentId();

            // Get all ideas with the given submissionId and departmentId
            List<Idea> ideas = _context.Ideas
                .Where(i => i.SubmissionId == id && i.DepartmentId == departmentId)
                .Include(i => i.User)
                .ToList();

            var user = await _userManager.GetUserAsync(User);
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
            // Pass the ideas to the view
            return View(ideas);
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


        // Reaction
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
            var result = new
            {
                totalReacts = react.TotalReacts,
                reactionButtonId = GetReactionButtonId(react.Reaction, idea.Id)
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
                default:
                    return "";
            }
        }

        public async Task<IActionResult> Music()
        {
            var ideas = _context.Ideas.Include(i => i.Catogories).Include(i => i.Submission).Include(i => i.User);

            return View(await ideas.ToListAsync());
        }






    }
}
