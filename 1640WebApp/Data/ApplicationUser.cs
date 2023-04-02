using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.ConstrainedExecution;

namespace _1640WebApp.Data
{
    public class ApplicationUser:IdentityUser
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? StaffNumber { get; set; }
        public string? Fullname_ { get; set; }
        public string? Address { get; set; }
        public string? HomeTown { get; set; }
        public string? SocialMedia { get; set; }

        public string? Image { get; set; }

        public int DepartmentId { get; set; }
        public virtual ICollection<Department> Departments { get; set; }

    }


    [Table("Roles")]
    public class AppRole
    {
        
        public string ID { get; set; }
        public string UserRole { get; set; }

    }

    [Table("Ideas")]
    public class Idea
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Text { get; set; }
        public string? FilePath { get; set; }
        public DateTime Datatime { get; set; }

        public byte[]? Img { get; set; }
        public byte[]? Data { get; set; }
        public bool Anonymous { get; set; } = false;
        public int IdeaId { get; set; }
        public string CreatorEmail { get; set; } = "";
        public string? FileName { get; set; }
        public string? UserId { get; set; }
        public int? DepartmentId { get; set; }
        public virtual ApplicationUser? User { get; set; }

        public int? SubmissionId { get; set; }
        public virtual Submission? Submission { get; set; }

        public int? CatogoryId { get; set; }
        //public virtual Catogory? Catogory { get; set; }

        public int ViewCount { get; set; }
       

        public virtual ICollection<Comment>? Comments { get; set; }
        public virtual ICollection<React>? Reacts { get; set; }
        public virtual ICollection<CView>? Views { get; set; }
        public virtual ICollection<Catogory>? Catogories { get; set; }

    }

    [Table("Catogorys")]

    public class Catogory
    {
        public int Id { get; set; }
        public string? Name { get; set; }    
    }

    [Table("Departments")]
    public class Department
    {
        public int Id { get; set; }
        public string? Name { get; set; }

    }

    [Table("Comments")]

    public class Comment
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public DateTime? Datetime { get; set; }
        public string CreatorComment { get; set; }

        public string? UserId { get; set; }
        public virtual ApplicationUser? User { get; set; }

        public int? IdeaId { get; set; }
        public virtual Idea? Idea { get; set; }

    }

    [Table("CViews")]

    public class CView
    {
        public int Id { get; set; }
        public DateTime VisitTime { get; set; }

        public string? UserId { get; set; }
        public virtual ApplicationUser? User { get; set; }

        public int? IdeaId { get; set; }
        public virtual Idea? Idea { get; set; }

    }

    [Table("Reacts")]

    public class React
    {
        public int Id { get; set; }
        public int Reaction { get; set; }
        public int TotalReacts { get; set; }
        public string CreatorComment { get; set; }
        public string? UserId { get; set; }
        public virtual ApplicationUser? User { get; set; }
        public DateTime DateTime { get; set; }
        public int? IdeaId { get; set; }
        public virtual Idea? Idea { get; set; }

    }


    [Table("Submissions")]

    public class Submission
    {
        public int Id { get; set; }
        public string? Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime? ClosureDate { get; set; }
        public DateTime? FinalClosureTime { get; set; }
        public bool IsClosed { get; set; }

        public virtual ICollection<Idea>? Ideas { get; set; }
    }

    public class Notification
    {
        public string Title { get; set; }
        public string Message { get; set; }
    }

    public class Email
    {
        public string To { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public string From = "demotest579@gmail.com";
        public string Password = "wqefgrtfbacntumf";

    }






}
