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
using OfficeOpenXml;

namespace hallocDoc.Controllers
{
    [CustomAuthorize("Admin")]
    public class ADashboardController : Controller
    {
        private readonly IADashboard _iadash;
        private readonly IPDashboard _pDashboard;
        private readonly IHostingEnvironment _hostEnvironment;
        private readonly IJwtService _jwtService;
        private readonly IFormRequest _iformReq;

        public ADashboardController(IADashboard dashboard, IPDashboard pDashboard, IHostingEnvironment hostingEnvironment, IJwtService jwtService, IFormRequest formRequest)
        {
            _iadash = dashboard;
            _pDashboard = pDashboard;
            _hostEnvironment = hostingEnvironment;
            _jwtService = jwtService;
            _iformReq = formRequest;
        }


        public IActionResult SendLinkPost(ADashTable adt)
        {
            string subject = "Greetings From HalloDoc";
            string message = "For submit your request follow this link" + "http://localhost:5011/";
            string email = adt.email;
            string[] s = new string[0];
            var check = _iadash.sendMail(email, subject, message, s);
            TempData["SuccessMessage"] = "Email Sent Successfully..";
            return RedirectToAction("Dmain", "ADashboard");
        }

        public IActionResult MyProfilePost(int Aid, AdminProfile ap)
        {
            bool check = _iadash.MyProfilePost(Aid, ap);
            if (check)
            {
                TempData["SuccessMessage"] = "Data Updated Successfully..";
            }
            else
            {
                TempData["ErrorMessage"] = "Data is not updated";
            }
            return RedirectToAction("MyProfile", "ADashboard", new { Aid = Aid });
        }

        public IActionResult ProfilePasswordSubmit(int Aid, AdminProfile ap)
        {
            bool check = _iadash.ProfilePasswordSubmit(Aid, ap);
            if (check)
            {
                TempData["SuccessMessage"] = "Password Updated Successfully..";
            }
            return RedirectToAction("MyProfile", "ADashboard", new { Aid = Aid });
        }

        public async Task<IActionResult> MyProfile(int? Aid)
        {
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
            try
            {
            string email = _iadash.EmailFromRid(rid);
            string subject = "HalloDoc Attachement";
            string message = "Please Check Attached Files for your Request From HalloDoc..";
            List<string> files = new List<string>();
            var check = _iadash.sendMail(email, subject, message, filenames);
            TempData["SuccessMessage"] = "Mail Sent Successfully..";
            return RedirectToAction("ViewUpload", "ADashboard", new { rid = rid });
            }
            catch(Exception ex)
            {
                return RedirectToAction("first", "Home");
            }
        }


        [HttpPost]
        public async Task<IActionResult> Encounter(int rid, EncounterData ed, string? s)
        {
            int status = await _iadash.EncounterPost(rid, ed);
            TempData["SuccessMessage"] = "Data Updated Successfully..";
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
            TempData["SuccessMessage"] = "Case Closed..";
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
            TempData["SuccessMessage"] = "Aggrement sent..";
            return RedirectToAction("Dmain", "ADashboard", new { id = status });
        }

        [HttpPost]
        public IActionResult ClearCase(int id)
        {
            int status = _iadash.ClearCase(id);
            TempData["SuccessMessage"] = "Case Cleared";
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
            TempData["SuccessMessage"] = "Case Transfered Successfully..";
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

            TempData["SuccessMessage"] = "Order placed Successfully..";
            ModelState.AddModelError("ordersuccess", "Order placed Successfully..");
            return RedirectToAction("Dmain", "ADashboard", new { id = status });
        }
        public async Task<IActionResult> uploadbtn(History h, int reqid)
        {
            if (await _pDashboard.uploadtoid(h, reqid))
            {
                TempData["SuccessMessage"] = "File Uploaded Successfully..";
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
            TempData["SuccessMessage"] = "File deleted Successfully..";
            return RedirectToAction("ViewUpload", "ADashboard", new { rid = x });

        }
        public async Task<IActionResult> DeleteFile(int fileId)
        {
            string fileName = _iadash.fileNameFromId(fileId);
            int x = await _iadash.DeleteFile(fileName);
            ModelState.AddModelError("deleted", "File Deleted Successfully..");
            TempData["SuccessMessage"] = "File deleted Successfully..";
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
            if (status1 != null)
            {
                TempData["SuccessMessage"] = "Data Updated Successfully..";
            }
            return RedirectToAction("Dmain", "ADashboard", new { id = status1 });
        }
        public async Task<IActionResult> CancelCase(int rid, ADashTable dt)
        {
            int status1 = await _iadash.CancelCase(rid, dt);
            if (status1 != null)
            {
                TempData["SuccessMessage"] = "Data Updated Successfully..";
            }

            return RedirectToAction("Dmain", "ADashboard", new { id = status1 });
        }
        public async Task<IActionResult> AssignCase(int rid, ADashTable dt)
        {
            return RedirectToAction("Dmain");
        }
        [HttpGet]
        public async Task<IActionResult> ViewNote(int rid, AViewNoteCase? vnc)
        {
            TempData["id"] = rid;
            var result = _iadash.VNData(rid, vnc);
            return View(result);
        }
        [HttpPost]
        public async Task<IActionResult> ViewNote(AViewNoteCase vnc)
        {
            int? rid = TempData["id"] as int?;
            var check = await _iadash.VNDatapost(vnc);
            if (check)
            {
                TempData["SuccessMessage"] = "Data Updated Successfully..";
            }
            else
            {
                TempData["ErrorMessage"] = "Data is not updated";
            }
            return RedirectToAction("ViewNote", "ADashboard", new {rid = rid});
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
                TempData["SuccessMessage"] = "Data Updated Successfully..";
            }
            return View(vnc);
        }

        public IActionResult AdminSendReq()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> AdminSendReq(patientReq pr)
        {

            if (ModelState.IsValid)
            {
                var filename = _iformReq.uploadfile(pr.myfile);

                var result = _iformReq.Patient(filename, pr);
                if (await result == "first")
                {
                    TempData["SuccessMessage"] = "Request Generated Successfully..";

                    return RedirectToAction("Dmain", "ADashboard");
                }
                else if (await result == "")
                {
                    return View(pr);
                }

            }
            TempData["SuccessMessage"] = "Request Generated Successfully..";
            return RedirectToAction("Dmain", "ADashboard");
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

        public IActionResult ExportAllRequests()
        {

            List<Request> requests = _iadash.ReqData();

            if (requests == null || requests.Count == 0)
            {
                return NotFound("No requests found to export.");
            }
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Requests");

                worksheet.Cells[1, 1].Value = "RequestId";
                worksheet.Cells[1, 2].Value = "UserId";
                worksheet.Cells[1, 3].Value = "FirstName";
                worksheet.Cells[1, 4].Value = "LastName";
                worksheet.Cells[1, 5].Value = "PhoneNumber";
                worksheet.Cells[1, 6].Value = "Email";
                worksheet.Cells[1, 7].Value = "Status";
                worksheet.Cells[1, 8].Value = "PhysicianId";
                worksheet.Cells[1, 9].Value = "ConfirmationNumber";
                worksheet.Cells[1, 10].Value = "CreatedDate";
                worksheet.Cells[1, 11].Value = "RelationName";

                int row = 2;
                foreach (var request in requests)
                {
                    worksheet.Cells[row, 1].Value = request.RequestId;
                    worksheet.Cells[row, 2].Value = request.UserId;
                    worksheet.Cells[row, 3].Value = request.FirstName;
                    worksheet.Cells[row, 4].Value = request.LastName;
                    worksheet.Cells[row, 5].Value = request.PhoneNumber;
                    worksheet.Cells[row, 6].Value = request.Email;
                    worksheet.Cells[row, 7].Value = request.Status;
                    worksheet.Cells[row, 8].Value = request.PhysicianId;
                    worksheet.Cells[row, 9].Value = request.ConfirmationNumber;
                    worksheet.Cells[row, 10].Value = request.CreatedDate;
                    worksheet.Cells[row, 11].Value = request.RelationName;
                    row++;
                }
                worksheet.Cells[worksheet.Dimension.Start.Row, 1].AutoFitColumns();
                byte[] excelData = package.GetAsByteArray();

                return File(excelData, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "requests.xlsx");
            }
        }



    }
}
