using hallocDoc.DataContext;
using hallocDoc.DataModels;
using hallocDoc.Models;
using hallocDoc.ViewDataModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Session;

namespace hallocDoc.Controllers
{
    public class pDashboardController : Controller
    {
        private readonly ApplicationDbContext _context;
        public pDashboardController( ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> History()
        {
            if(HttpContext.Session.GetString("aspid").ToString() == null)
            {
                return RedirectToAction("Home", "Index");
            }
            
            var userid = HttpContext.Session.GetString("aspid").ToString();
            var userdb = await _context.Aspnetusers.FirstOrDefaultAsync(m => m.Id == userid);
            var username = _context.Users.FirstOrDefaultAsync(m => m.AspNetUserId == userid);
            
            /*if (userdb != null)
            {

            }*/
            return View();
        }
        public IActionResult viewDoc()
        {
            return View();
        }
    }
}
