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
        public pDashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> History()
        {
            if (HttpContext.Session?.GetString("aspid").ToString() == null)
            {
                return RedirectToAction("Home", "Index");
            }

            var aspid = HttpContext.Session?.GetString("aspid").ToString();
            var userdb = await _context.Users.FirstOrDefaultAsync(m => m.AspNetUserId == aspid);
            var userid = userdb?.UserId;
            ViewBag.username = userdb?.FirstName;

            /*var dbreq2 = from x in _context.Requests where x.UserId == userid select x;*/
            var dbreq2 = from r in _context.Requests
                         join rf in _context.Requestwisefiles on r.RequestId equals rf.RequestId
                         where r.UserId == userid
                         select new { r, rf };



            List<History> Calc = new List<History>();
            foreach (var item in dbreq2)
            {
                var nh = new History();
                nh.guid = item.r.RequestId;
                nh.status = (int)item.r.Status;
                nh.date = item.r.CreatedDate;
                nh.name = item.rf.FileName;

                /*nh.guid = item.RequestId;
                nh.status = (int)item.Status;
                nh.date = item.CreatedDate;*/


                Calc.Add(nh);

            }
            ViewBag.history = Calc;

            List<History> a = new List<History>();
            {
                var h = new History();
                h.status = 3;
                h.date = DateTime.Now;
                h.name = "SHUBHAM";
            }
            ViewBag.history2 = a;


            return View();
        }
        public IActionResult viewDoc()
        {
            return View();
        }
    }
}
