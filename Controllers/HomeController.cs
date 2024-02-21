    using halloDocEntities.DataContext;
using halloDocEntities.DataModels;
using hallocDoc.Models;
using halloDocEntities.ViewDataModels;
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
using halloDocLogic.Interfaces;

namespace hallocDoc.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        private readonly ILogger<HomeController> _logger;

        private readonly IHome _home;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context, IHome home)
        {
            _logger = logger;
            _context = context;
            _home = home;
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

            var result = _home.setPassWithToken(cemail?.ToString(), ctoken?.ToString(), rp.Password);

            if(await result)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ModelState.AddModelError("wrong", "Something went wrong...");
                return View();
            }

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
                var result = _home.login(pl);
                if (result.ToString() == "notfound")
                {
                    ModelState.AddModelError("NotFound", "User not found, please register first..");
                    return View(pl);
                }

                if (result.ToString() == "success")
                {
                    var dbasp = _context.Aspnetusers.FirstOrDefault(m => m.Email == pl.Email);
                    HttpContext.Session.SetString("aspid", dbasp.Id);
                    return RedirectToAction("History", "pDashboard");
                }
                if(result.ToString() == "fail")
                {
                    ModelState.AddModelError("wrong", "wrong password");
                    return View(pl);
                }
            }
            /*if (ModelState.IsValid)
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
            }*/
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

                var result = _home.sendmail(email, passwordResetLink);

                if(result == true)
                {
                    ModelState.AddModelError("success", "Email sent successfully, please check your Email..");
                    return View();
                }
            }

            ModelState.AddModelError("fail", "Something went wrong please try again..");
            return View();
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
            if(ModelState.IsValid)
            {
                var result = _home.setPass(cp.Email, cp.Password);
                if(await result)
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("NotFound", "User not found, Submit request first..");
                    return View(cp);
                }
            }
            return View(cp);
            
        }
    }
}