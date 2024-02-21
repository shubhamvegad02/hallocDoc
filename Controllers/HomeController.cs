using hallocDoc.DataContext;
using hallocDoc.DataModels;
using hallocDoc.Models;
using hallocDoc.ViewDataModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using Microsoft.AspNetCore.Session;
using Microsoft.AspNetCore.Identity;
using System.Net.Mail;
using System.Net;
using NuGet.Protocol;
using Nest;
using System.Xml.Linq;

namespace hallocDoc.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }
        [HttpGet]
        public IActionResult resetPass(string email, string token)
        {
            /*ViewBag.userdata=email;*/
            TempData["Email"] = email;
            TempData["Token"] = token;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> resetPass(resetPass rp)
        {

            var cemail = TempData["Email"];
            var ctoken = TempData["Token"];

            var dbasp = await _context.Aspnetusers.FirstOrDefaultAsync(m => m.Email == cemail);
            if (dbasp != null)
            {
                /*if (dbasp.Token == ctoken)*/
                if (String.Equals(dbasp.Token, ctoken.ToString()))
                {
                    dbasp.PasswordHash = password.encry(rp.Password);
                    _context.Aspnetusers.Update(dbasp);
                    _context.SaveChanges();
                    return RedirectToAction("Index", "Home");
                }
            }


            ModelState.AddModelError("wrong", "Something went wrong...");
            return View();






        }
        public IActionResult first()
        {
            return View();
        }


        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(patientLogin pl)
        {
            if (ModelState.IsValid)
            {
                var dbdata = await _context.Aspnetusers.FirstOrDefaultAsync(m => m.Email == pl.Email);
                if (dbdata == null)
                {
                    ModelState.AddModelError("NotFound", "User not found, please register first..");
                    return View(pl);

                }
                else
                {
                    if (password.decry(dbdata.PasswordHash) == pl.Password)
                    {
                        HttpContext.Session.SetString("aspid", dbdata.Id);
                        return RedirectToAction("History", "pDashboard");
                    }
                    ModelState.AddModelError("wrong", "wrong password");
                    return View(pl);

                }
            }
            return View();
        }

        [HttpGet]
        public IActionResult Privacy()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Privacy(forgotpass fp)
        {


            var dbasp = await _context.Aspnetusers.FirstOrDefaultAsync(m => m.Email == fp.Email);
            if (dbasp == null)
            {
                ModelState.AddModelError("Notfound", "User Not Found..");
                return View();
            }
            else
            {
                var email = fp.Email;
                var token = Guid.NewGuid().ToString();
                dbasp.Token = token;
                _context.Aspnetusers.Update(dbasp);
                _context.SaveChanges();

                var passwordResetLink = Url.Action("resetPass", "Home", new { Email = email, Token = token }, protocol: HttpContext.Request.Scheme);


                var smtpClient = new SmtpClient("smtp.office365.com")
                {
                    Port = 587,
                    Credentials = new NetworkCredential("tatva.dotnet.shubhamvegad@outlook.com", "Vegad@12"),
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false
                };

                var mailMessage = new MailMessage
                {
                    From = new MailAddress("tatva.dotnet.shubhamvegad@outlook.com"),
                    Subject = "Reset Your Password for halloDoc",
                    
                    Body = "<div> Hello " + email + "</div><p>We received a request to reset your password for your account on halloDoc. If you initiated this request, please click the link below to choose a new password:</p><p>" + passwordResetLink + "</p>",
                    IsBodyHtml = true,
                };
                mailMessage.To.Add(email);

                smtpClient.Send(mailMessage);
            }

            ModelState.AddModelError("success", "Email sent successfully, please check your Email..");
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        public async Task<IActionResult> createPatient(createPatient cp, string aspid)
        {
            var dbasp = await _context.Aspnetusers.FirstOrDefaultAsync(m => m.Id == aspid.ToString());
            if (dbasp != null)
            {
                cp.Email = dbasp.Email;
            }
            return View(cp);
        }

        [HttpPost]
        public async Task<IActionResult> createPatient(createPatient cp)
        {
            var dbasp = await _context.Aspnetusers.FirstOrDefaultAsync(m => m.Email == cp.Email);
            if (dbasp == null)
            {
                ModelState.AddModelError("NotFound", "User not found, Submit request first..");
                return View(cp);
            }
            else
            {
                if (ModelState.IsValid)
                {
                    dbasp.PasswordHash = password.encry(cp.Password);
                    _context.Aspnetusers.Update(dbasp);
                    _context.SaveChanges();
                    return RedirectToAction("Index", "Home");
                }
                return View(cp);
            }

        }
    }
}