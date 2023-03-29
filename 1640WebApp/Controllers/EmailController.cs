using _1640WebApp.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Mail;
using _1640WebApp.Data;
using Microsoft.AspNetCore.Authorization;


namespace _1640WebApp.Controllers
{
    [Authorize(Roles = "Manager")]
    public class EmailController : Controller
    {
        public IActionResult Form()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Form(Email model)
        {
            using(MailMessage message = new MailMessage(model.From, model.To))
            {
                message.Subject = model.Subject;
                message.Body = model.Body;
                message.IsBodyHtml = false;
                using (SmtpClient smtp = new SmtpClient())
                {
                    smtp.Host = "smtp.gmail.com";
                    smtp.EnableSsl = true;
                    NetworkCredential NetCre = new NetworkCredential(model.From, model.Password);
                    smtp.UseDefaultCredentials = false;
                    smtp.Credentials = NetCre;
                    smtp.Port = 587;
                    smtp.Send(message);
                    ViewBag.Message = "Email Sent Successfully";

                }
                
                return View();
            }
        }
        
    }
}
