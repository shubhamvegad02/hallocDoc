﻿using halloDocEntities.DataContext;
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

namespace hallocDoc.Controllers
{
    public class ADashboardController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IADashboard _iadash;
        public ADashboardController(ApplicationDbContext dbContext, IADashboard dashboard)
        {
            _context = dbContext;
            _iadash = dashboard;
        }

        public async Task<IActionResult> CancelCase(int rid, ADashTable dt)
        {
            /*int status = dt.status;*/


            /*var dbreq = _context.Requests.FirstOrDefault(m => m.RequestId == rid);
            int status1 = dbreq?.Status ?? 0;
            if (dbreq != null)
            {
                dbreq.Status = 5;
                dbreq.CaseTag = dt.caseTag.ToString();
                _context.Requests.Update(dbreq);
                _context.SaveChanges();
            }
            var dbrnote = await _context.Requestnotes.FirstOrDefaultAsync(m => m.RequestId == rid);
            if (dbrnote != null)
            {
                dbrnote.AdminNotes = dt.notes;
                dbrnote.CreatedBy = "Admin";
                dbrnote.CreatedDate = DateTime.Today;
                _context.Requestnotes.Update(dbrnote);
                _context.SaveChanges();
            }
            else
            {
                Requestnote rn = new Requestnote();
                rn.RequestId = rid;
                rn.AdminNotes = dt.notes;
                rn.CreatedBy = "Admin";
                rn.CreatedDate = DateTime.Today;
                _context.Requestnotes.Add(rn);
                _context.SaveChanges();
            }


            Requeststatuslog rsl = new Requeststatuslog();
            rsl.RequestId = rid;
            *//*rsl.AdminId = 1;*//*
            rsl.Status = 5;
            rsl.Notes = dt.notes;
            _context.Requeststatuslogs.Add(rsl);
            _context.SaveChanges();*/

            int status1 = await _iadash.CancelCase(rid, dt);

            return RedirectToAction("Dmain", "ADashboard", new { id = status1 });
        }
        public async Task<IActionResult> AssignCase(int rid, ADashTable dt)
        {
            /*var dbreq = _context.Requests.FirstOrDefault(m => m.RequestId == rid);
            if (dbreq != null)
            {
                dbreq.PhysicianId = dt.phyId;
                dbreq.Status = 2;
                _context.Requests.Update(dbreq);
                _context.SaveChanges();
            }
            var dbrnote = await _context.Requestnotes.FirstOrDefaultAsync(m => m.RequestId == rid);
            if (dbrnote != null)
            {
                dbrnote.AdminNotes = dt.notes;
                dbrnote.CreatedBy = "Admin";
                dbrnote.CreatedDate = DateTime.Today;
                _context.Requestnotes.Update(dbrnote);
                _context.SaveChanges();
            }
            else
            {
                Requestnote rn = new Requestnote();
                rn.RequestId = rid;
                rn.AdminNotes = dt.notes;
                rn.CreatedBy = "Admin";
                rn.CreatedDate = DateTime.Today;
                _context.Requestnotes.Add(rn);
                _context.SaveChanges();
            }
            var reqsnote = await _context.Requeststatuslogs.FirstOrDefaultAsync(m => m.RequestId == rid);

            Requeststatuslog rsl = new Requeststatuslog();
            rsl.RequestId = rid;
            *//*rsl.AdminId = 1;*//*
            rsl.Status = 2;
            rsl.TransToPhysicianId = dt.phyId;
            _context.Requeststatuslogs.Add(rsl);
            _context.SaveChanges();*/

            return RedirectToAction("Dmain");
            /*return JavaScript("alert('No more images');");*/
        }
        public async Task<IActionResult> ViewNote(int rid)
        {
            var vnc = _iadash.VNData(rid);


            return View(vnc);
        }

        public async Task<IActionResult> ViewCase(int rid)
        {
            var vnc = _iadash.VCData(rid);
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
