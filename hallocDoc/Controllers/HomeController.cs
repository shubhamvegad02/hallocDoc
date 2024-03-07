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
using Microsoft.AspNetCore.Mvc.Filters;
using halloDocLogic.Repository;
using NuGet.Common;
using System.IdentityModel.Tokens.Jwt;
using Newtonsoft.Json.Linq;

namespace hallocDoc.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<HomeController> _logger;
        private readonly IHome _home;
        private readonly IJwtService _jwtservice;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context, IHome home, IJwtService jwtservice)
        {
            _logger = logger;
            _context = context;
            _home = home;
            _jwtservice = jwtservice;
        }

        public IActionResult UnauthorizedAccess()
        {
            return View();
        }

        public IActionResult CreateuserfromLink(string email, string token, createPatient cp)
        {
            TempData["Email"] = email;
            TempData["Token"] = token;
            cp.Email = email;
            return View(cp);
        }
        [HttpPost]
        public async Task<IActionResult> CreateuserfromLink(createPatient cp)
        {
            var cemail = TempData["Email"];
            var ctoken = TempData["Token"];
            var result = _home.setPassWithToken(cemail?.ToString(), ctoken?.ToString(), cp.Password);

            if (await result)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ModelState.AddModelError("wrong", "Something went wrong...");
                return View();
            }
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
            string jwtTokenCheck = Request.Cookies["jwt"] ?? "";
            if (_jwtservice.ValidateToken(jwtTokenCheck, out JwtSecurityToken jwtToken1))
            {
                JwtClaimsModel jwtClaims = new JwtClaimsModel();
                jwtClaims = _jwtservice.GetClaimsFromJwtToken(jwtTokenCheck);
                string userRole = jwtClaims?.Role;

                if (userRole == "User")
                {
                    return RedirectToAction("History", "pDashboard");
                }
                else if (userRole == "Admin")
                {
                    return RedirectToAction("Dmain", "ADashboard");
                }
            }
            return View();
        }

        [HttpPost]
        public  IActionResult Index(patientLogin pl)
        {
            
                if (ModelState.IsValid)
                {
                    var result = _home.login(pl);
                if (result.Status == "notfound")
                {
                    ModelState.AddModelError("NotFound", "User not found, please register first..");
                    return View(pl);
                }

                if (result.Status == "success")
                {
                    Aspnetuser aspnetuser = _home.AspDataFromId(result?.Aspid);
                    var jwtToken = _jwtservice.GenerateJwtToken(aspnetuser);
                    Response.Cookies.Append("jwt", jwtToken);

                    JwtClaimsModel jwtClaims = new JwtClaimsModel();
                    jwtClaims = _jwtservice.GetClaimsFromJwtToken(jwtToken);
                    string userRole = jwtClaims.Role;

                    if(userRole == "User")
                    {
                    return RedirectToAction("History", "pDashboard");
                    }
                    else if(userRole == "Admin")
                    {
                    return RedirectToAction("Dmain", "ADashboard");
                    }
                    else
                    {
                    return RedirectToAction("UnauthorizedAccess", "Home");

                    }
                    /*string aspid = jwtClaims.AspId;
                    HttpContext.Session.SetString("aspid", aspid);*/

                }
                if(result.Status == "fail")
                {
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
            
            if(!_home.checkuser(fp.Email))
            {
                ModelState.AddModelError("Notfound", "User Not Found..");
                return View();
            }
            else
            {
                var token = _home.generateToken(fp.Email);
                var email = fp.Email;
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
            cp.Email = _home.getEmailFromId(aspid.ToString());
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