using halloDocLogic.Interfaces;
using halloDocEntities.DataModels;
using halloDocEntities.ViewDataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using halloDocEntities.DataContext;
using Microsoft.Extensions.Hosting;
using System.Data.Common;
using Microsoft.EntityFrameworkCore;
using Nest;
using MailKit.Security;
using MimeKit;

namespace halloDocLogic.Repository
{
    public class ADashboard : IADashboard
    {
        private readonly ApplicationDbContext _context;

        public ADashboard(ApplicationDbContext context)
        {
            _context = context;
        }



        public string fileNameFromId(int fileId)
        {
            var dbreqfile = _context.Requestwisefiles.FirstOrDefault(m => m.RequestWiseFileId == fileId);
            string fileName = dbreqfile?.FileName;
            return fileName;
        }
        public async Task<int> DeleteFile(string file)
        {
            var dbreqfile = await _context.Requestwisefiles.FirstOrDefaultAsync(m => m.FileName == file);
            int rid = dbreqfile.RequestId;
            var fileToDelete = await _context.Requestwisefiles.FirstOrDefaultAsync(f => f.FileName == file);
            if (fileToDelete != null)
            {
                fileToDelete.IsDeleted = true;
            }
            await _context.SaveChangesAsync();
            return rid;
        }
        public async Task<int> BlockCase(int rid, ADashTable dt)
        {
            var dbreq = _context.Requests.FirstOrDefault(m => m.RequestId == rid);
            int status1 = dbreq?.Status ?? 0;
            if (dbreq != null)
            {
                dbreq.Status = 7;

                _context.Requests.Update(dbreq);
                _context.SaveChanges();
            }

            Blockrequest br = new Blockrequest();
            br.PhoneNumber = dt.mobile;
            br.Email = dt.email;
            br.Reason = dt.notes;
            br.RequestId = rid;
            br.CreatedDate = DateTime.Today;
            _context.Blockrequests.Add(br);
            _context.SaveChanges();

            Requeststatuslog rsl = new Requeststatuslog();
            rsl.RequestId = rid;
            /*rsl.AdminId = 1;*/
            rsl.Status = 7;
            rsl.Notes = dt.notes;
            rsl.CreatedDate = DateTime.Now.Date;
            _context.Requeststatuslogs.Add(rsl);
            _context.SaveChanges();

            return status1;
        }


        public async Task AssignCase(int rid, ADashTable dt)
        {
            var dbreq = _context.Requests.FirstOrDefault(m => m.RequestId == rid);
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
            /*rsl.AdminId = 1;*/
            rsl.Status = 2;
            rsl.TransToPhysicianId = dt.phyId;
            _context.Requeststatuslogs.Add(rsl);
            _context.SaveChanges();
        }
        public async Task<int> CancelCase(int rid, ADashTable dt)
        {
            var dbreq = _context.Requests.FirstOrDefault(m => m.RequestId == rid);
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
            /*rsl.AdminId = 1;*/
            rsl.Status = 5;
            rsl.Notes = dt.notes;
            _context.Requeststatuslogs.Add(rsl);
            _context.SaveChanges();

            return status1;
        }
        public AViewNoteCase VNData(int rid, AViewNoteCase vnc)
        {
            /*var dbdata = from r in _context.Requests
                         join rn in _context.Requestnotes on r.RequestId equals rn.RequestId
                         where r.RequestId == rid
                         select new { r, rn };*/
            var dbnotedata = from rl in _context.Requeststatuslogs
                             join rn in _context.Requestnotes on rl.RequestId equals rn.RequestId
                             where rn.RequestId == rid
                             select new { rl, rn };
            var dbreq = _context.Requests.FirstOrDefault(m => m.RequestId == rid);
            if (dbreq != null)
            {
                vnc.status = dbreq.Status;
            }
            else
            {
                vnc.status = 1;
            }

            foreach (var item in dbnotedata)
            {
                vnc.Tnotes = item.rl.Notes;
                vnc.Pnotes = item.rn.PhysicianNotes;
                vnc.Anotes = item.rn.AdminNotes;
                vnc.rid = rid;
            }
            return vnc;
        }
        public async Task<bool> VNDatapost(AViewNoteCase vnc)
        {
            int rid = vnc.rid;
            vnc.Anotes = vnc.TempAnotes;
            var dbreqnotes = await _context.Requestnotes.FirstOrDefaultAsync(m => m.RequestId == rid);
            dbreqnotes.AdminNotes = vnc?.Anotes;
            _context.Requestnotes.Update(dbreqnotes);
            _context.SaveChanges();
            vnc.TempAnotes = null;

            return true;
        }
        public AViewNoteCase VCData(int rid)
        {

            var dbdata = from r in _context.Requests
                         join rc in _context.Requestclients on r.RequestId equals rc.RequestId
                         where r.RequestId == rid
                         select new { r, rc };

            AViewNoteCase vnc = new AViewNoteCase();
            vnc.rid = rid;
            foreach (var item in dbdata)
            {

                vnc.relation = item.r.RelationName;
                vnc.status = item.r.Status;
                vnc.guid = Guid.NewGuid().ToString();
                vnc.fname = item.rc?.FirstName;
                vnc.lname = item?.rc?.LastName;
                vnc.email = item?.rc?.Email;
                vnc.mobile = item?.rc.PhoneNumber;
                vnc.symptoms = item?.rc?.Notes;
                vnc.status = item?.r.Status;
                /*var dates = string.Concat(item?.rc?.IntDate.ToString() + item.rc.StrMonth?.Substring(0, 3) + item.rc.IntYear);
                DateTime fdate = DateTime.Parse(dates);
                vnc.dob = fdate;*/
                /*var region =  _context.Regions.FirstOrDefault(r => r.RegionId == item.rc.RegionId)?.Name;*/
                /*vnc.region = item.rc.State;
                vnc.Tnotes = item.rn.AdministrativeNotes;
                vnc.Pnotes = item.rn.PhysicianNotes;
                vnc.Anotes = item.rn.AdminNotes;*/
            }
            return (vnc);
        }
        public async Task<bool> VCDataPost(AViewNoteCase vnc)
        {
            int rid = vnc.rid;
            var dbreqClient = await _context.Requestclients.FirstOrDefaultAsync(m => m.RequestId == rid);
            dbreqClient.PhoneNumber = vnc.mobile;
            dbreqClient.Email = vnc.email;
            _context.Requestclients.Update(dbreqClient);
            _context.SaveChanges();

            return true;
        }


        public async Task<List<ADashTable>> ADashTableData(int n)

        {
            var dbdata1 = from r in _context.Requests
                          join rc in _context.Requestclients on r.RequestId equals rc.RequestId
                          where r.Status == n
                          select new { r, rc };

            var dbdata = from r in _context.Requests
                         join rc in _context.Requestclients on r.RequestId equals rc.RequestId
                         join p in _context.Physicians on r.PhysicianId equals p.PhysicianId into gj
                         from p in gj.DefaultIfEmpty() // Left outer join
                         where r.Status == n
                         select new { r, rc, p };

            List<ADashTable> dtable = new List<ADashTable>();
            foreach (var item in dbdata)
            {
                var dt = new ADashTable();

                dt.status = n;
                dt.guid = Guid.NewGuid().ToString();
                dt.rid = item.r.RequestId;
                dt.name = string.Concat(item.rc.FirstName, " ", item.rc.LastName);
                dt.email = item.rc.Email;
                /*dt.dob = new DateTime(item.rc.IntYear.Value, int.Parse(item.rc.StrMonth), item.rc.IntDate.Value);*/
                dt.dob = item.r.CreatedDate.Date;
                dt.requstor = item.r?.RelationName;
                dt.reqDate = item.r.CreatedDate.Date;
                dt.mobile = item.r.PhoneNumber;
                dt.address = string.Concat(item.rc.Street, " ", item.rc.City, " ", item.rc.State);
                dt.notes = "";
                if (item.p != null)
                {
                    dt.PhysicianName = string.Concat(item.p.FirstName, " ", item.p.LastName);
                }
                else
                {
                    dt.PhysicianName = "Physician";
                }
                dt.region = item.rc.RegionId.ToString();
                dt.relation = item.r.RelationName;

                dtable.Add(dt);


            }
            return dtable;
        }
        public ARequestCount ReqCount()
        {
            ARequestCount reqc = new ARequestCount();
            reqc.newc = _context.Requests.Count(m => m.Status == 1);
            reqc.pendingc = _context.Requests.Count(m => m.Status == 2);
            reqc.activec = _context.Requests.Count(m => m.Status == 3);
            reqc.concludec = _context.Requests.Count(m => m.Status == 4);
            reqc.closec = _context.Requests.Count(m => m.Status == 5);
            reqc.unpaidc = _context.Requests.Count(m => m.Status == 6);

            return reqc;
        }

        
    }
}
