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
using System.IO.Compression;
using NuGet.Protocol.Plugins;

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



        public async Task<IActionResult> uploadbtn(History h, int reqid) {
            string filename1 = null;
            if (h.myfile != null)
            {
                string folder = "uplodedItems/";
                var key = Guid.NewGuid().ToString();
                folder += key + "_" +h.myfile.FileName;
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
            return RedirectToAction("viewDoc", "pDashboard");
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
            if (HttpContext.Session?.GetString("aspid").ToString() == null)
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
            ViewBag.username = userdb?.FirstName;

            /*var dbreq2 = from x in _context.Requests where x.UserId == userid select x;*/
            /*var dbreq2 = from r in _context.Requests
                         join rf in _context.Requestwisefiles on r.RequestId equals rf.RequestId
                         where r.UserId == userid
                         group r by r.UserId into g
                         select new { r=g.key, rf };*/
            var dbreq3 = from r in _context.Requests where r.UserId == userid select new { r };



            List<History> Calc = new List<History>();
            foreach (var item in dbreq3)
            {
                var nh = new History();
                nh.guid = item.r.RequestId;
                nh.status = (int)item.r.Status;
                nh.date = item.r.CreatedDate;
                /*var temp = item.rf.FileName;
                temp = temp.Substring(temp.IndexOf("_") + 1);
                nh.name = temp;*/
                nh.name = item.r.RequestId.ToString();

                Calc.Add(nh);

            }
            ViewBag.history = Calc;

            return View();
        }
        public async Task<IActionResult> viewDoc(int reqid)
        {
            int b = reqid;
            ViewBag.reqid = b;
            if (HttpContext.Session?.GetString("aspid").ToString() == "")
            {
                return RedirectToAction("Home", "Index");
            }
            /*var sessionValue = HttpContext.Session.GetString("aspId");
            if (sessionValue is null)
            {
                return RedirectToAction("Index", "Home");
            }*/

            /*var aspid = HttpContext.Session?.GetString("aspid").ToString();
            var userdb = await _context.Users.FirstOrDefaultAsync(m => m.AspNetUserId == aspid);
            var userid = userdb?.UserId;
            ViewBag.username = string.Concat(userdb.FirstName, " ", userdb.LastName);

            var dbreq2 = from r in _context.Requests
                         join rf in _context.Requestwisefiles on r.RequestId equals rf.RequestId
                         where r.UserId == userid
                         select new { r, rf };*/

            /*var reqfiledb = from r in _context.Requestwisefiles where r.RequestId == reqid select r;*/
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
            /*foreach (var item in dbreq2)
            {
                string path = Path.Combine(this.Environment.WebRootPath, "uplodedItems/") + item.rf.FileName;
                byte[] bytes = System.IO.File.ReadAllBytes(path);
                return File(bytes, "application/octet-stream");
            }
            return null;*/
            string baseFilePath = Path.Combine(this.Environment.WebRootPath, "uplodedItems");

            using (ZipArchive zip = new ZipArchive(new MemoryStream(), ZipArchiveMode.Update, true))
            {
                foreach (var filename in dbreq2)
                {
                    // Construct the full file path
                    string fullFilePath = Path.Combine(baseFilePath, filename.rf.FileName);

                    // Ensure the file exists before adding to the zip
                    if (!System.IO.File.Exists(fullFilePath))
                    {
                        return NotFound("File not found: "); // Or handle differently
                    }

                    // Add the file to the zip archive, preserving directory structure
                    ZipArchiveEntry zipEntry = zip.CreateEntry(Path.GetFileName(filename.rf.FileName));
                    using (Stream entryStream = zipEntry.Open())
                    using (FileStream fileStream = System.IO.File.OpenRead(fullFilePath))
                    {
                        fileStream.CopyTo(entryStream);
                    }
                }

                // Access the first entry after adding all files
                ZipArchiveEntry entry = zip.Entries.First();
                return File(entry.Open(), "application/zip", "all_files.zip");
            }

        }

        [HttpGet]
        public async Task<IActionResult> profile(profile p)
        {
            var sessionValue = HttpContext.Session.GetString("aspId");
            if (sessionValue is null)
            {
                return RedirectToAction("Index", "Home");
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
