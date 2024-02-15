using hallocDoc.DataContext;
using hallocDoc.DataModels;
using hallocDoc.Models;
using hallocDoc.ViewDataModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace hallocDoc.Controllers
{
    public class pDashboardController : Controller
    {
        private readonly ApplicationDbContext _context;
        public pDashboardController( ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult History(string s)
        {
            /*var dbdata =  _context.Aspnetusers.FirstOrDefaultAsync(m => m.Id == s);*/
            return View();
        }
        public IActionResult viewDoc()
        {
            return View();
        }
    }
}
