using halloDocEntities.DataContext;
using halloDocEntities.DataModels;
using hallocDoc.Models;
using hallocDoc.ViewDataModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Session;
using Nest;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using System.IO.Compression;
using NuGet.Protocol.Plugins;
using Microsoft.CodeAnalysis.Elfie.Serialization;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace hallocDoc.Controllers
{
    public class pDashboardController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly Microsoft.AspNetCore.Hosting.IHostingEnvironment Environment;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public pDashboardController(ApplicationDbContext context, Microsoft.AspNetCore.Hosting.IHostingEnvironment _environment, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            Environment = _environment;
            _webHostEnvironment = webHostEnvironment;
        }



        public async Task<IActionResult> uploadbtn(History h, int reqid)
        {
            string filename1 = null;
            if (h.myfile != null)
            {
                string folder = "uplodedItems/";
                var key = Guid.NewGuid().ToString();
                folder += key + "_" + h.myfile.FileName;
                filename1 = key + "_" + h.myfile.FileName;
                string serverFolder = Path.Combine(_webHostEnvironment.WebRootPath, folder);

                using (var fileStream = new FileStream(serverFolder, FileMode.Create))
                {
                    fileStream.Position = 0;
                    h.myfile.CopyToAsync(fileStream);
                    fileStream.Flush();

                }
            }
            var rid = reqid;

            var rf = new Requestwisefile();
            rf.RequestId = rid;
            rf.CreatedDate = DateTime.Now;
            rf.FileName = filename1;
            _context.Requestwisefiles.Add(rf);
            _context.SaveChanges();
            return RedirectToAction("History", "pDashboard");
        }

        public IActionResult logout()
        {
            HttpContext.Session.Clear();
            HttpContext.Session.Remove("aspid");

            return RedirectToAction("Index", "Home");
        }
        public IActionResult agreement()
        {
            /*var aspid = HttpContext.Session?.GetString("aspid").ToString();
            var userdb = _context.Users.FirstOrDefault(m => m.AspNetUserId == aspid);

            ViewBag.username = userdb?.FirstName;*/
            return View();
        }
        public async Task<IActionResult> History()
        {
            if (HttpContext.Session?.GetString("aspid")?.ToString() == null)
            {
                return RedirectToAction("Home", "Index");
            }
            /*var sessionValue = HttpContext.Session.GetString("aspId");
            if (sessionValue is null)
            {
                return RedirectToAction("Index", "Home");
            }*/
            var aspid = HttpContext.Session?.GetString("aspid").ToString();
            var userdb = await _context.Users.FirstOrDefaultAsync(m => m.AspNetUserId == aspid);
            var userid = userdb?.UserId;
            ViewBag.username = string.Concat(userdb.FirstName, " ", userdb.LastName);

            var dbreq3 = from r in _context.Requests where r.UserId == userid select new { r };



            List<History> Calc = new List<History>();
            foreach (var item in dbreq3)
            {
                /*var count = _context.Requestwisefiles.Where(rf => rf.RequestId == item.r.RequestId).Select(rf => rf).Count();*/

                var nh = new History();
                nh.guid = item.r.RequestId;
                nh.status = (int)item.r.Status;
                nh.date = item.r.CreatedDate;
                nh.name = item.r.RequestId.ToString();

                Calc.Add(nh);

            }


            ViewBag.history = Calc;

            return View();
        }
        public async Task<IActionResult> viewDoc(int reqid)
        {

            var aspid = HttpContext.Session?.GetString("aspid").ToString();
            var userdb = await _context.Users.FirstOrDefaultAsync(m => m.AspNetUserId == aspid);
            var userid = userdb?.UserId;
            ViewBag.username = string.Concat(userdb.FirstName, " ", userdb.LastName);
            ViewBag.reqid = reqid;
            if (HttpContext.Session?.GetString("aspid").ToString() == "")
            {
                return RedirectToAction("Home", "Index");
            }

            var dbreq2 = from r in _context.Requests
                         join rf in _context.Requestwisefiles on r.RequestId equals rf.RequestId
                         where rf.RequestId == reqid
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


        public IActionResult DonwlodFileAll()
        {
            var aspid = HttpContext.Session?.GetString("aspid").ToString();
            var userdb = _context.Users.FirstOrDefault(m => m.AspNetUserId == aspid);
            var userid = userdb?.UserId;

            var dbreq2 = from r in _context.Requests
                         join rf in _context.Requestwisefiles on r.RequestId equals rf.RequestId
                         where r.UserId == userid
                         select new { r, rf };

            string baseFilePath = Path.Combine(this.Environment.WebRootPath, "uplodedItems");

            MemoryStream zipStream = new MemoryStream();
            using (var zipArchive = new ZipArchive(zipStream, ZipArchiveMode.Create, leaveOpen: true))
            {
                foreach (var document in dbreq2)
                {
                    string fullFilePath = Path.Combine(baseFilePath, document.rf.FileName);

                    string fileName = document.rf.FileName;
                    int index = fileName.LastIndexOf("/");
                    if (index != -1)
                        fileName = fileName.Substring(index + 1);
                    zipArchive.CreateEntryFromFile(fullFilePath, fileName);
                }
            } // disposal of archive will force data to be written to memory stream.
            zipStream.Position = 0; //reset memory stream position.
            return File(zipStream, "application/zip", "MyDocuments.zip");
        }


        [HttpGet]
        public async Task<IActionResult> profile(profile p)
        {
            /*var sessionValue = HttpContext.Session.GetString("aspId");
            if (sessionValue is null)
            {
                return RedirectToAction("Index", "Home");
            }*/
            if (HttpContext.Session?.GetString("aspid").ToString() == "")
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

            if (ModelState.IsValid)
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

            ViewBag.success = "Data updated Successfully..";
            return RedirectToAction("profile", "pDashboard");



        }
    }
}
