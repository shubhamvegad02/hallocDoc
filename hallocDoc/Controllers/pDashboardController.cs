using halloDocEntities.DataContext;
using halloDocEntities.DataModels;
using hallocDoc.Models;
using halloDocEntities.ViewDataModels;
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
using halloDocLogic.Interfaces;
using Org.BouncyCastle.Ocsp;
using Microsoft.Extensions.Hosting;
using halloDocLogic.Repository;
using Newtonsoft.Json.Linq;
using Microsoft.AspNetCore.Authorization;

namespace hallocDoc.Controllers
{
    /*[CustomAuthorize("User")]*/
    public class pDashboardController : Controller
    {
        /*private readonly ApplicationDbContext _context;*/
        private readonly Microsoft.AspNetCore.Hosting.IHostingEnvironment Environment;
        /* private readonly IWebHostEnvironment _webHostEnvironment;*/
        private readonly IPDashboard _pDashboard;
        private readonly IJwtService _jwtService;


        public pDashboardController(ApplicationDbContext context, Microsoft.AspNetCore.Hosting.IHostingEnvironment _environment, IJwtService jwtService, IPDashboard pDashboard)
        {
            /*_context = context;*/
            Environment = _environment;
            /*_webHostEnvironment = webHostEnvironment;*/
            _pDashboard = pDashboard;
            _jwtService = jwtService;

        }



        public async Task<IActionResult> uploadbtn(History h, int reqid)
        {
            if (await _pDashboard.uploadtoid(h, reqid))
            {
                return RedirectToAction("History", "pDashboard");
            }
            return RedirectToAction("viewDoc", "pDashboard");
        }

        public IActionResult logout()
        {
            /*HttpContext.Session.Clear();
            HttpContext.Session.Remove("aspid");*/
            Response.Cookies.Delete("jwt");

            return RedirectToAction("Index", "Home");
        }
        [HttpGet]
        public IActionResult agreement(int Rid)
        {
            ViewBag.rid = Rid;
            return View();
        }

        [HttpPost]
        public IActionResult agreement(int rid, string? s)
        {
            var check = _pDashboard.agreement(rid);
            ModelState.AddModelError("agreesuccess", "Your Response saved successfully..");
            return View();
        }
        public IActionResult popupSubmit(History h, int rid)
        {
            string notes = h.popupNotes;
            var check = _pDashboard.popupSubmit(notes, rid);
            return RedirectToAction("agreement");
        }

        [CustomAuthorize("User")]
        public async Task<IActionResult> History()
        {
            /*if (HttpContext.Session?.GetString("aspid")?.ToString() == null)
            {
                return RedirectToAction("Index", "Home");
            var aspid = HttpContext.Session?.GetString("aspid").ToString();
            }*/

            

            string jwtToken = Request.Cookies["jwt"] ?? "";
            JwtClaimsModel jwtClaims = new JwtClaimsModel();
            jwtClaims = _jwtService.GetClaimsFromJwtToken(jwtToken);
            var aspid = jwtClaims.AspId;


            ViewBag.username = _pDashboard.UserNameFromId(aspid);

            var Calc = _pDashboard.HistoryData(aspid);
            ViewBag.history = Calc;

            return View();
        }

        [CustomAuthorize("User")]
        public async Task<IActionResult> viewDoc(int reqid)
        {
            /*if (HttpContext.Session?.GetString("aspid")?.ToString() == null)
            {
                return RedirectToAction("Index", "Home");
            }
            var aspid = HttpContext.Session?.GetString("aspid").ToString();*/
            string jwtToken = Request.Cookies["jwt"] ?? "";
            JwtClaimsModel jwtClaims = new JwtClaimsModel();
            jwtClaims = _jwtService.GetClaimsFromJwtToken(jwtToken);
            var aspid = jwtClaims.AspId;

            ViewBag.username = _pDashboard.UserNameFromId(aspid);
            ViewBag.reqid = reqid;
            /*var userdb = await _context.Users.FirstOrDefaultAsync(m => m.AspNetUserId == aspid);
            var userid = userdb?.UserId;*/

            var data = _pDashboard.ViewDocData(aspid, reqid);
            ViewBag.history = data;
            return View();
        }



        public FileResult DonwlodFile(string filename)
        {
            string path = Path.Combine(this.Environment.WebRootPath, "uplodedItems/") + filename;
            byte[] bytes = System.IO.File.ReadAllBytes(path);
            return File(bytes, "application/octet-stream", filename);
        }


        public IActionResult DonwlodFileAll(string aspId)
        {
            string aspid;
            if (aspId != null)
            {
                aspid = aspId;
            }
            else
            {
                /*aspid = HttpContext.Session?.GetString("aspid").ToString();*/
                string jwtToken = Request.Cookies["jwt"] ?? "";
                JwtClaimsModel jwtClaims = new JwtClaimsModel();
                jwtClaims = _jwtService.GetClaimsFromJwtToken(jwtToken);
                aspid = jwtClaims.AspId;
            }
            var zipstream = _pDashboard.downloadAll(aspid);
            /*var userdb = _context.Users.FirstOrDefault(m => m.AspNetUserId == aspid);
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
            zipStream.Position = 0; //reset memory stream position.*/
            return File(zipstream, "application/zip", "MyDocuments.zip");
        }

        public IActionResult DownloadSelected([FromBody] string[] filenames)
        {
            string baseFilePath = Path.Combine(this.Environment.WebRootPath, "uplodedItems");

            MemoryStream zipStream = new MemoryStream();
            using (var zipArchive = new ZipArchive(zipStream, ZipArchiveMode.Create, leaveOpen: true))
            {
                foreach (var item in filenames)
                {
                    string fullFilePath = Path.Combine(baseFilePath, item);

                    string fileName = item;
                    int index = fileName.LastIndexOf("/");
                    if (index != -1)
                        fileName = fileName.Substring(index + 1);
                    zipArchive.CreateEntryFromFile(fullFilePath, fileName);
                }
            } // disposal of archive will force data to be written to memory stream.
            zipStream.Position = 0; //reset memory stream position.
            return File(zipStream, "application/zip", "MyDocuments.zip");
        }

        [CustomAuthorize("User")]
        [HttpGet]
        public async Task<IActionResult> profile(profile p)
        {
            /*if (HttpContext.Session?.GetString("aspid")?.ToString() == null)
            {
                return RedirectToAction("Index", "Home");
            }
            var aspid = HttpContext.Session?.GetString("aspid").ToString();*/

            string jwtToken = Request.Cookies["jwt"] ?? "";
            JwtClaimsModel jwtClaims = new JwtClaimsModel();
            jwtClaims = _jwtService.GetClaimsFromJwtToken(jwtToken);
            var aspid = jwtClaims.AspId;
            ViewBag.username = _pDashboard.UserNameFromId(aspid);

            var pr = _pDashboard.ProfileData(aspid, p);
            return View(p);
        }

        [CustomAuthorize("User")]
        [HttpPost]
        public IActionResult profilesubmit(profile p)
        {
            if (ModelState.IsValid)
            {
                /*var aspid = HttpContext.Session?.GetString("aspid").ToString();*/
                string jwtToken = Request.Cookies["jwt"] ?? "";
                JwtClaimsModel jwtClaims = new JwtClaimsModel();
                jwtClaims = _jwtService.GetClaimsFromJwtToken(jwtToken);
                var aspid = jwtClaims.AspId;

                if (_pDashboard.ProfileSubmit(aspid, p))
                {
                    ViewBag.success = "Data updated Successfully..";
                    ViewBag.toast = true;
                    return RedirectToAction("History", "pDashboard");

                }

            }

            return RedirectToAction("profile", "pDashboard");
        }
    }
}
