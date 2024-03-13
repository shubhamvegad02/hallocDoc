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

namespace hallocDoc.Controllers
{
    [CustomAuthorize("Admin")]
    public class ADashboardController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IADashboard _iadash;
        private readonly IPDashboard _pDashboard;
        private readonly IHostingEnvironment _hostEnvironment;
        public ADashboardController(ApplicationDbContext dbContext, IADashboard dashboard, IPDashboard pDashboard, IHostingEnvironment hostingEnvironment)
        {
            _context = dbContext;
            _iadash = dashboard;
            _pDashboard = pDashboard;
            _hostEnvironment = hostingEnvironment;
        }





        public async Task<IActionResult> SendFilesInMail(int rid, [FromBody] string[] filenames)
        {

            //string email = _iadash.EmailFromRid(rid);
            string email = "vegadshubham2002@gmail.com";
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
            
            return RedirectToAction("Dmain", "ADashboard", new {id = status});
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
            var dbreq = _context.Requests.FirstOrDefault(m => m.RequestId == rid);
            if (dbreq != null)
            {
                dbreq.PhysicianId = adt.phyId;
                _context.Requests.Update(dbreq);
                _context.SaveChanges();
            }
            var dbreqnotes = await _context.Requestnotes.FirstOrDefaultAsync(m => m.RequestId == rid);
            if (dbreqnotes != null)
            {
                dbreqnotes.AdminNotes = adt.notes;
                dbreqnotes.ModifiedBy = "Admin";
                dbreqnotes.ModifiedDate = DateTime.Now;
                _context.Requestnotes.Update(dbreqnotes);
                _context.SaveChanges();
            }
            var rl = new Requeststatuslog();
            rl.RequestId = rid;
            rl.Status = 2;
            rl.AdminId = 1;
            rl.TransToPhysicianId = adt.phyId;
            rl.Notes = adt.notes;
            rl.CreatedDate = DateTime.Now;
            _context.Requeststatuslogs.Add(rl);
            _context.SaveChanges();

            return RedirectToAction("Dmain", "ADashboard", new { id = 2 });
        }
        public async Task<IActionResult> Logout()
        {
            Response.Cookies.Delete("jwt");
            return RedirectToAction("Index", "Home");
        }
        public async Task<IActionResult> GetOrderData(int businessId)
        {
            var business = await _context.Healthprofessionals
                              .Where(p => p.VendorId == businessId)
                              .Select(p => new
                              {
                                  Contact = p.PhoneNumber,
                                  Email = p.Email,
                                  Fax = p.FaxNumber,

                              })
                              .FirstOrDefaultAsync();

            return Json(business);
        }

        public async Task<IActionResult> Order(int rid)
        {
            TempData["rid"] = rid;
            var dbReq = await _context.Requests.FirstOrDefaultAsync(m => m.RequestId == rid);
            TempData["status"] = dbReq.Status;
            ViewBag.status = dbReq.Status;
            var dbProType = await _context.Healthprofessionaltypes.ToListAsync();
            var dbpro = await _context.Healthprofessionals.ToListAsync();


            var sendOrder = new SendOrder
            {
                professionList = dbProType,
                businessList = dbpro
            };
            /*{
                professionList = dbProType,
                businessList = dbpro
            };*/

            return View(sendOrder);
        }

        [HttpPost]
        public async Task<IActionResult> Order(SendOrder so)
        {
            var rid = TempData["rid"] as int?;
            var status = TempData["status"] as int?;
            var dbprofessional = await _context.Healthprofessionals.FirstOrDefaultAsync(m => m.VendorName == so.business);
            int? vendor = dbprofessional?.VendorId;

            if (so != null)
            {
                var od = new Orderdetail();
                od.VendorId = int.Parse(so.business);
                od.RequestId = rid;
                od.FaxNumber = so.fax;
                od.BusinessContact = so.Contact;
                od.Email = so.email;
                od.Prescription = so.orderDetail;
                od.NoOfRefill = so.refill;
                od.CreatedBy = "Admin";
                od.CreatedDate = DateTime.Today;
                await _context.Orderdetails.AddAsync(od);
                _context.SaveChanges();
            }


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
            var dbreq = from r in _context.Requests
                        join rf in _context.Requestwisefiles on r.RequestId equals rf.RequestId
                        where rf.RequestId == rid
                        where rf.IsDeleted == false || rf.IsDeleted == null
                        select new { r, rf };

            var dbReqClient = await _context.Requestclients.FirstOrDefaultAsync(m => m.RequestId == rid);
            ViewBag.PatientName = string.Concat(dbReqClient.FirstName, " ", dbReqClient.LastName);
            string con = "";
            ViewBag.reqid = rid;
            List<ViewUploadedDoc> data = new List<ViewUploadedDoc>();
            foreach (var item in dbreq)
            {
                var vu = new ViewUploadedDoc();


                con = item?.r?.ConfirmationNumber;
                vu.uploadDate = item.r.CreatedDate;
                vu.fileName = item.rf.FileName;
                vu.fileId = item.rf.RequestWiseFileId;
                vu.rid = rid;
                


                data.Add(vu);
            }
            ViewBag.confirmation = con;
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

            List<Region> rlist = new List<Region>();
            var dbregion = _context.Regions;
            foreach (var item in dbregion)
            {
                Region r = new Region();
                r.RegionId = item.RegionId;
                r.Name = item.Name;
                rlist.Add(r);
            }
            ViewBag.rlist = rlist;

            List<Physician> plist = new List<Physician>();
            var dbphysician = _context.Physicians;
            foreach (var item in dbphysician)
            {
                Physician p = new Physician();
                p.FirstName = item.FirstName;
                p.RegionId = item.RegionId;
                p.PhysicianId = item.PhysicianId;
                plist.Add(p);
            }
            ViewBag.plist = plist;

            /*var dbdata = from r in _context.Requests
                         join rc in _context.Requestclients on r.RequestId equals rc.RequestId
                         where r.Status == 1
                         select new { r, rc };*/
            ViewBag.heading = TempData["heading"];
            /*var dbdata = await (from r in _context.Requests
                                join rc in _context.Requestclients on r.RequestId equals rc.RequestId
                                where r.Status == n
                                select new { r, rc }).ToListAsync();


            List<ADashTable> dtable = new List<ADashTable>();
            foreach (var item in dbdata)
            {
                var dt = new ADashTable();
                dt.guid = Guid.NewGuid().ToString();
                dt.name = string.Concat(item.rc.FirstName, " ", item.rc.LastName);
                dt.email = item.rc.Email;
                *//*dt.dob = new DateTime(item.rc.IntYear.Value, int.Parse(item.rc.StrMonth), item.rc.IntDate.Value);*//*
                dt.dob = item.r.CreatedDate.Date;
                dt.requstor = item.r?.RelationName;
                dt.reqDate = item.r.CreatedDate.Date;
                dt.mobile = item.r.PhoneNumber;
                dt.address = string.Concat(item.rc.Street, " ", item.rc.City, " ", item.rc.State);
                dt.notes = "";
                dt.region = item.rc.RegionId.ToString();
                var providerName = await _context.Physicians?.FirstOrDefaultAsync(m => m.PhysicianId == item.r.PhysicianId);
                if (providerName != null)
                {
                    dt.provider = providerName.FirstName;
                }
                dt.relation = item.r.RelationName;

                dtable.Add(dt);
            }*/
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
