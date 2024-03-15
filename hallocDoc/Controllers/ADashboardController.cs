using halloDocEntities.DataContext;
using halloDocEntities.ViewDataModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Nest;
using System;
using System.Collections;
using System.Xml.Linq;
using System.Text;
using halloDocLogic.Interfaces;
using System.Data.Common;
using halloDocEntities.DataModels;
using System.Security.Cryptography;
using Org.BouncyCastle.Ocsp;
using Microsoft.CodeAnalysis.Elfie.Serialization;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;
using halloDocLogic.Repository;

namespace hallocDoc.Controllers
{
    [CustomAuthorize("Admin")]
    public class ADashboardController : Controller
    {
        private readonly IADashboard _iadash;
        private readonly IPDashboard _pDashboard;
        private readonly IHostingEnvironment _hostEnvironment;
        private readonly IJwtService _jwtService;

        public ADashboardController(IADashboard dashboard, IPDashboard pDashboard, IHostingEnvironment hostingEnvironment, IJwtService jwtService)
        {
            _iadash = dashboard;
            _pDashboard = pDashboard;
            _hostEnvironment = hostingEnvironment;
            _jwtService = jwtService;
        }


        public IActionResult MyProfilePost(int Aid, AdminProfile ap)
        {
            bool check = _iadash.MyProfilePost(Aid, ap);
            if (check)
            {
                TempData["SuccessMessage"] = "Data Updated Successfully..";
            }
            return RedirectToAction("MyProfile", "ADashboard", new { Aid = Aid });
        }

        public IActionResult ProfilePasswordSubmit(int Aid, AdminProfile ap)
        {
            bool check = _iadash.ProfilePasswordSubmit(Aid, ap);
            if (check)
            {
                TempData["SuccessMessage"] = "Data Updated Successfully..";
            }
            return RedirectToAction("MyProfile", "ADashboard", new { Aid = Aid });
        }

        public async Task<IActionResult> MyProfile(int? Aid)
        {
                TempData["SuccessMessage"] = "Data Updated Successfully..";
            int aid = 0;
            if (Aid == null)
            {
                string jwtToken = Request.Cookies["jwt"] ?? "";
                JwtClaimsModel jwtClaims = new JwtClaimsModel();
                jwtClaims = _jwtService.GetClaimsFromJwtToken(jwtToken);
                var aspid = jwtClaims.AspId;

                int NewAid = _iadash.AdminidFromAspid(aspid);
                aid = NewAid;
                ViewBag.Aid = NewAid;

            }
            else
            {
                ViewBag.Aid = Aid;
                aid = Aid ?? 0;
            }
            AdminProfile ap = await _iadash.MyProfile(aid);
            return View(ap);
        }

        public async Task<IActionResult> SendFilesInMail(int rid, [FromBody] string[] filenames)
        {

            string email = _iadash.EmailFromRid(rid);
            string subject = "HalloDoc Attachement";
            string message = "Please Check Attached Files for your Request From HalloDoc..";
            List<string> files = new List<string>();
            var check = _iadash.sendMail(email, subject, message, filenames);
            return RedirectToAction("ViewUpload", "ADashboard", new { rid = rid });
        }


        [HttpPost]
        public async Task<IActionResult> Encounter(int rid, EncounterData ed, string? s)
        {
            int status = await _iadash.EncounterPost(rid, ed);

            return RedirectToAction("Dmain", "ADashboard", new { id = status });
        }

        public async Task<IActionResult> Encounter(int rid, EncounterData ed)
        {
            var encounterData = await _iadash.Encounter(rid, ed);
            return View(encounterData);
        }


        public async Task<IActionResult> closeCasefinal(int rid)
        {
            int status = await _iadash.closeCasefinal(rid);
            return RedirectToAction("Dmain", "ADashboard", new { id = status });
        }


        public async Task<IActionResult> closeCase(int id, AViewNoteCase vnc)
        {
            int status = await _iadash.closeCasePost(id, vnc);

            return RedirectToAction("Dmain", "ADashboard", new { id = status });
        }

        [HttpGet]
        public async Task<IActionResult> closeCase(int rid)
        {
            ViewBag.FileData = await _iadash.closeCase(rid);
            AViewNoteCase vnc = _iadash.VCData(rid);
            return View(vnc);
        }

        [HttpPost]
        public IActionResult sendAgreement(int rid)
        {
            int status = _iadash.sendAgreement(rid);
            return RedirectToAction("Dmain", "ADashboard", new { id = status });
        }

        [HttpPost]
        public IActionResult ClearCase(int id)
        {
            int status = _iadash.ClearCase(id);
            return RedirectToAction("Dmain", "ADashboard", new { id = status });
        }

        public FileResult DonwlodFile(string filename)
        {
            string path = Path.Combine(_hostEnvironment.WebRootPath, "uplodedItems/") + filename;
            byte[] bytes = System.IO.File.ReadAllBytes(path);
            return File(bytes, "application/octet-stream", filename);
        }

        [HttpPost]
        public async Task<IActionResult> TransferCase(int rid, ADashTable adt)
        {
            bool check = await _iadash.TransferCase(rid, adt);
            return RedirectToAction("Dmain", "ADashboard", new { id = 2 });
        }
        public async Task<IActionResult> Logout()
        {
            Response.Cookies.Delete("jwt");
            return RedirectToAction("Index", "Home");
        }
        public async Task<IActionResult> GetOrderData(int businessId)
        {
            var business = await _iadash.GetOrderData(businessId);

            return Json(business);
        }

        public async Task<IActionResult> Order(int rid)
        {
            int status = await _iadash.statusFromRid(rid);

            TempData["rid"] = rid;
            TempData["status"] = status;
            ViewBag.status = status;

            SendOrder so = await _iadash.order();

            return View(so);
        }

        [HttpPost]
        public async Task<IActionResult> Order(SendOrder so)
        {
            var rid = TempData["rid"] as int?;
            int nrid = rid ?? 0;
            var status = TempData["status"] as int?;

            /*var dbprofessional = await _context.Healthprofessionals.FirstOrDefaultAsync(m => m.VendorName == so.business);
            int? vendor = dbprofessional?.VendorId;*/

            bool check = _iadash.OrderPost(nrid, so);

            ModelState.AddModelError("ordersuccess", "Order placed Successfully..");
            return RedirectToAction("Dmain", "ADashboard", new { id = status });
        }
        public async Task<IActionResult> uploadbtn(History h, int reqid)
        {
            if (await _pDashboard.uploadtoid(h, reqid))
            {
                return RedirectToAction("ViewUpload", "ADashboard", new { rid = reqid });
            }
            return RedirectToAction("ViewUpload", "ADashboard", new { rid = reqid });
        }
        public async Task<IActionResult> DeleteFiles(int reqId, [FromBody] string[] filenames)
        {
            if (filenames == null || filenames.Length == 0)
            {
                return BadRequest("No files selected");
            }
            int x = 0;
            foreach (var item in filenames)
            {

                x = await _iadash.DeleteFile(item);
            }

            return RedirectToAction("ViewUpload", "ADashboard", new { rid = x });

        }
        public async Task<IActionResult> DeleteFile(int fileId)
        {
            string fileName = _iadash.fileNameFromId(fileId);
            int x = await _iadash.DeleteFile(fileName);
            ModelState.AddModelError("deleted", "File Deleted Successfully..");
            return RedirectToAction("ViewUpload", "ADashboard", new { rid = x });
        }


        public async Task<IActionResult> ViewUpload(int rid)
        {
            var data = await _iadash.ViewUpload(rid);

            foreach (var item in data)
            {
                ViewBag.PatientName = item.patientName;
                ViewBag.confirmation = item.confirmation;
            }
            ViewBag.reqid = rid;
            ViewBag.FileData = data;
            return View();
        }
        public async Task<IActionResult> BlockCase(int rid, ADashTable dt)
        {

            int status1 = await _iadash.BlockCase(rid, dt);
            return RedirectToAction("Dmain", "ADashboard", new { id = status1 });
        }
        public async Task<IActionResult> CancelCase(int rid, ADashTable dt)
        {
            int status1 = await _iadash.CancelCase(rid, dt);

            return RedirectToAction("Dmain", "ADashboard", new { id = status1 });
        }
        public async Task<IActionResult> AssignCase(int rid, ADashTable dt)
        {
            return RedirectToAction("Dmain");
        }
        [HttpGet]
        public async Task<IActionResult> ViewNote(int rid, AViewNoteCase vnc)
        {
            var result = _iadash.VNData(rid, vnc);


            return View(result);
        }
        [HttpPost]
        public async Task<IActionResult> ViewNote(AViewNoteCase vnc)
        {

            var check = await _iadash.VNDatapost(vnc);
            /*if(check)
            {
                ModelState.AddModelError("success", "Note Updated Successfully...");
            }*/
            ModelState.AddModelError("success", "Note Updated Successfully...");
            return View(vnc);
        }

        public async Task<IActionResult> ViewCase(int rid)
        {
            var vnc = _iadash.VCData(rid);
            return View(vnc);
        }

        [HttpPost]
        public async Task<IActionResult> ViewCase(AViewNoteCase vnc)
        {
            var result = _iadash.VCDataPost(vnc);
            if (await result)
            {
                ModelState.AddModelError("success", "Data Updated Successfully...");
            }
            return View(vnc);
        }

        [HttpPost]
        public FileResult Export(string GridHtml)
        {
            return File(Encoding.ASCII.GetBytes(GridHtml), "application/vnd.ms-excel", "Grid.xls");
        }


        public async Task<IActionResult> Dmain(int id, ADashTable d)
        {
            var n = id;
            if (n == 0)
            {
                n = 1;
            }
            ARequestCount arc = _iadash.ReqCount();
            ViewBag.newc = arc.newc;
            ViewBag.penc = arc.pendingc;
            ViewBag.actc = arc.activec;
            ViewBag.conc = arc.concludec;
            ViewBag.closec = arc.closec;
            ViewBag.unpaidc = arc.unpaidc;

            List<Region> rlist = _iadash.getRegiondata();
            ViewBag.rlist = rlist;

            List<Physician> plist = _iadash.getPhysiciandata();
            ViewBag.plist = plist;

            ViewBag.heading = TempData["heading"];

            var dtable = await _iadash.ADashTableData(n);
            ViewBag.tableData = dtable;
            string name = "c" + n.ToString();
            ViewBag.cardid = name;
            ViewBag.n = n;

            return View();
        }
        public IActionResult DmainReq(int n)
        {
            switch (n)
            {
                case 1:
                    TempData["heading"] = "New";
                    break;
                case 2:
                    TempData["heading"] = "Pending";
                    break;
                case 3:
                    TempData["heading"] = "Active";
                    break;
                case 4:
                    TempData["heading"] = "Conclude";
                    break;
                case 5:
                    TempData["heading"] = "ToClose";
                    break;
                case 6:
                    TempData["heading"] = "Unpaid";
                    break;
            }

            return RedirectToAction("Dmain", "ADashboard", new { id = n });
        }

        public async Task<IActionResult> DownloadExcel()
        {
            // Replace these with your actual data retrieval logic
            var n = 1;
            var dtable = await _iadash.ADashTableData(n);
            if (dtable.Count == 0)
            {
                return NotFound(); // Handle empty data case
            }

            using (var memoryStream = new MemoryStream())
            {
                using (var writer = new StreamWriter(memoryStream, Encoding.UTF8))
                {
                    // Create headers based on your data properties
                    writer.WriteLine(string.Join(",", typeof(ADashTable).GetProperties().Select(p => p.Name)));

                    // Populate data rows
                    foreach (var item in dtable)
                    {
                        writer.WriteLine(string.Join(",", typeof(ADashTable).GetProperties().Select(p => p.GetValue(item))));
                    }
                }

                memoryStream.Position = 0;
                return File(memoryStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "TableData.xlsx");
            }
        }



    }
}
