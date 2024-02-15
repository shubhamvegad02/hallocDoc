using hallocDoc.DataContext;
using hallocDoc.DataModels;
using hallocDoc.Models;
using hallocDoc.ViewDataModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;


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
            if(ModelState.IsValid)
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
                        HttpContext.Session.SetString("aspid", dbdata.Id.ToString());
                        return RedirectToAction("History", "pDashboard", dbdata.Id);
                    }
                    ModelState.AddModelError("wrong", "wrong password");
                    return View(pl);

                }
            }
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}