using hallocDoc.DataContext;
using hallocDoc.DataModels;
using hallocDoc.Models;
using hallocDoc.ViewDataModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Session;
using Nest;
using System.IO;
using Microsoft.AspNetCore.Hosting;


namespace hallocDoc.Controllers
{
    public class pDashboardController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly Microsoft.AspNetCore.Hosting.IHostingEnvironment Environment;

        public pDashboardController(ApplicationDbContext context, Microsoft.AspNetCore.Hosting.IHostingEnvironment _environment)
        {
            _context = context;
            Environment = _environment;
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
                var temp = item.rf.FileName;
                temp = temp.Substring(temp.IndexOf("_") + 1);
                nh.name = temp;

                Calc.Add(nh);

            }
            ViewBag.history = Calc;

            return View();
        }
        public async Task<IActionResult> viewDoc()
        {
            if (HttpContext.Session?.GetString("aspid").ToString() == null)
            {
                return RedirectToAction("Home", "Index");
            }

            var aspid = HttpContext.Session?.GetString("aspid").ToString();
            var userdb = await _context.Users.FirstOrDefaultAsync(m => m.AspNetUserId == aspid);
            var userid = userdb?.UserId;
            ViewBag.username = string.Concat(userdb.FirstName, " ", userdb.LastName);

            var dbreq2 = from r in _context.Requests
                         join rf in _context.Requestwisefiles on r.RequestId equals rf.RequestId
                         where r.UserId == userid
                         select new { r, rf };



            List<History> data = new List<History>();
            foreach (var item in dbreq2)
            {
                var nh = new History();
                nh.guid = item.r.RequestId;
                nh.uploder = string.Concat(item.r.FirstName, " ", item.r.LastName);
                nh.date = item.r.CreatedDate;
                nh.name = item.rf.FileName;

                data.Add(nh);

            }
            ViewBag.history = data;



            return View();
        }

        public FileResult DonwlodFile(string filename)
        {
            string path = Path.Combine(this.Environment.WebRootPath, "uplodedItems/") + filename;
            byte[] bytes = System.IO.File.ReadAllBytes(path);
            return File(bytes, "application/octet-stream", filename);
        }


        public FileResult DonwlodFileAll()
        {
            var aspid = HttpContext.Session?.GetString("aspid").ToString();
            var userdb = _context.Users.FirstOrDefault(m => m.AspNetUserId == aspid);
            var userid = userdb?.UserId;

            var dbreq2 = from r in _context.Requests
                         join rf in _context.Requestwisefiles on r.RequestId equals rf.RequestId
                         where r.UserId == userid
                         select new { r, rf };
            foreach (var item in dbreq2)
            {
                string path = Path.Combine(this.Environment.WebRootPath, "uplodedItems/") + item.rf.FileName;
                byte[] bytes = System.IO.File.ReadAllBytes(path);
                return File(bytes, "application/octet-stream");
            }
            return null;

        }

        [HttpGet]
        public async Task<IActionResult> profile(profile p)
        {
            if (HttpContext.Session?.GetString("aspid").ToString() == null)
            {
                return RedirectToAction("Home", "Index");
            }

            var aspid = HttpContext.Session?.GetString("aspid").ToString();
            var userdb = await _context.Users.FirstOrDefaultAsync(m => m.AspNetUserId == aspid);
            var userid = userdb?.UserId;
            ViewBag.username = string.Concat(userdb.FirstName, " ", userdb.LastName);

            p.FirstName = userdb.FirstName;
            p.LastName = userdb.LastName;
            p.CreatedDate = userdb.CreatedDate;
            p.Mobile = userdb.Mobile;
            p.Email = userdb.Email;
            p.Street = userdb.Street;
            p.City = userdb.City;
            p.State = userdb.State;
            p.ZipCode = userdb.ZipCode;

            return View(p);
        }

        [HttpPost]
        public async Task<IActionResult> profilesubmit(profile p)
        {
            var aspid = HttpContext.Session?.GetString("aspid").ToString();
            var userdb = await _context.Users.FirstOrDefaultAsync(m => m.AspNetUserId == aspid);
            var userid = userdb?.UserId;

            if(ModelState.IsValid)
            {
                userdb.FirstName = p?.FirstName;
                userdb.LastName = p?.LastName;
                userdb.CreatedDate = p.CreatedDate;
                userdb.Mobile = p?.Mobile;
                userdb.Email = p?.Email;
                userdb.Street = p?.Street;
                userdb.City = p?.City;
                userdb.State = p?.State;
                userdb.ZipCode = p?.ZipCode;
                _context.Users.Update(userdb);
                _context.SaveChanges();
            }



            /*var currentUser =  _context.Users.FirstOrDefaultAsync(p => p.UserId == userid);
            if(currentUser != null)
            {
                currentUser.firstName
            }*/

            /*ModelState.AddModelError("success", "Data Updated successfully..");*/
            ViewBag.success = "Data updated Successfully..";
            return RedirectToAction("profile", "pDashboard");



        }
    }
}
