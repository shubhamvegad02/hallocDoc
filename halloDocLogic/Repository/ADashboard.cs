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
/*using Nest;*/
using MailKit.Security;
using MimeKit;
using System.Net.Mail;
using System.Net;
using Microsoft.AspNetCore.Http;
using NuGet.Common;
using System.Security.Policy;
using Humanizer;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using System.Security.Cryptography.X509Certificates;
using System.Globalization;

namespace halloDocLogic.Repository
{
    public class ADashboard : IADashboard
    {
        private readonly ApplicationDbContext _context;
        private readonly IHostEnvironment _hostEnvironment;

        public ADashboard(ApplicationDbContext context, IHostEnvironment environment)
        {
            _context = context;
            _hostEnvironment = environment;
        }


        public string EmailFromRid(int rid)
        {
            var dbreq = _context.Requests.FirstOrDefault(m => m.RequestId == rid);
            if (dbreq != null)
            {
                string email = dbreq?.Email;
                return email;
            }
            return "";
        }
        public async Task<int> EncounterPost(int rid, EncounterData ed)
        {
            var dbreq = await _context.Requests.FirstOrDefaultAsync(m => m.RequestId == rid);
            int status = 1;
            if (dbreq != null)
            {
                status = dbreq.Status ?? 1;
            }
            var dbencounter = await _context.Encounters.FirstOrDefaultAsync(m => m.RequestId == rid);
            if (dbencounter != null)
            {
                dbencounter.Email = ed.Email;
                dbencounter.Mobile = ed.Mobile;
                dbencounter.Address = ed.Address;
                dbencounter.CreatedDate = ed.CreatedDate;
                dbencounter.Dob = ed.Dob;
                dbencounter.IllnessHistory = ed.IllnessHistory;
                dbencounter.MedicalHistory = ed.MedicalHistory;
                dbencounter.Medication = ed.Medication;
                dbencounter.Allergies = ed.Allergies;
                dbencounter.Temp = ed.Temp;
                dbencounter.Hr = ed.Hr;
                dbencounter.Rr = ed.Rr;
                dbencounter.O2 = ed.O2;
                dbencounter.Bp = ed.Bp;
                dbencounter.Pain = ed.Pain;
                dbencounter.Heent = ed.Heent;
                dbencounter.Cv = ed.Cv;
                dbencounter.Chest = ed.Chest;
                dbencounter.Abd = ed.Abd;
                dbencounter.Extr = ed.Extr;
                dbencounter.Skin = ed.Skin;
                dbencounter.Neuro = ed.Neuro;
                dbencounter.Other = ed.Other;
                dbencounter.Diagnosis = ed.Diagnosis;
                dbencounter.TreatmentPlan = ed.TreatmentPlan;
                dbencounter.MedicationDespensed = ed.MedicationDespensed;
                dbencounter.Procedure = ed.Procedure;
                dbencounter.Followup = ed.Followup;
                _context.Encounters.Update(dbencounter);
                _context.SaveChanges();
            }
            return status;
        }

        public async Task<EncounterData> Encounter(int rid, EncounterData ed)
        {
            var dbencounter = await _context.Encounters.FirstOrDefaultAsync(m => m.RequestId == rid);
            if (dbencounter != null)
            {
                ed.rid = rid;
                ed.Status = dbencounter.Status;
                ed.FirstName = dbencounter.FirstName;
                ed.LastName = dbencounter.LastName;
                ed.Mobile = dbencounter?.Mobile;
                ed.Email = dbencounter?.Email;
                ed.Address = dbencounter?.Address;
                ed.CreatedDate = DateTime.Now;
                ed.Dob = dbencounter?.Dob;
                ed.CreatedDate = dbencounter.CreatedDate;
                ed.Dob = dbencounter.Dob;
                ed.IllnessHistory = dbencounter.IllnessHistory;
                ed.MedicalHistory = dbencounter.MedicalHistory;
                ed.Medication = dbencounter.Medication;
                ed.Allergies = dbencounter.Allergies;
                ed.Temp = dbencounter.Temp;
                ed.Hr = dbencounter.Hr;
                ed.Rr = dbencounter.Rr;
                ed.O2 = dbencounter.O2;
                ed.Bp = dbencounter.Bp;
                ed.Pain = dbencounter.Pain;
                ed.Heent = dbencounter.Heent;
                ed.Cv = dbencounter.Cv;
                ed.Chest = dbencounter.Chest;
                ed.Abd = dbencounter.Abd;
                ed.Extr = dbencounter.Extr;
                ed.Skin = dbencounter.Skin;
                ed.Neuro = dbencounter.Neuro;
                ed.Other = dbencounter.Other;
                ed.Diagnosis = dbencounter.Diagnosis;
                ed.TreatmentPlan = dbencounter.TreatmentPlan;
                ed.MedicationDespensed = dbencounter.MedicationDespensed;
                ed.Procedure = dbencounter.Procedure;
                ed.Followup = dbencounter.Followup;
                ed.Status = dbencounter.Status;
                ed.IsFinalize = dbencounter.IsFinalize;
                /*if (dbencounter.IntDate.HasValue && dbencounter.IntYear.HasValue && dbencounter.StrMonth != null)
                {
                    DateTime monthDateTime = DateTime.ParseExact(dbencounter.StrMonth, "MMMM", CultureInfo.InvariantCulture);
                    int month = monthDateTime.Month;
                    DateOnly date = new DateOnly((int)dbencounter.IntYear, month, dbencounter.IntDate.Value);
                    e.Dob = date.ToString("yyyy-MM-dd");
                }*/

            }
            return ed;
        }

        public async Task<int> closeCasefinal(int rid)
        {
            int status = 1;
            var dbreq = await _context.Requests.FirstOrDefaultAsync(m => m.RequestId == rid);
            if (dbreq != null)
            {
                status = dbreq.Status ?? 1;
                dbreq.Status = 5;
                _context.Requests.Update(dbreq);
                _context.SaveChanges();
            }
            Requeststatuslog rsl = new Requeststatuslog();
            rsl.RequestId = rid;
            rsl.Status = 6;
            rsl.AdminId = 1;
            rsl.Notes = "closed by admin";
            rsl.CreatedDate = DateTime.Now;
            _context.Requeststatuslogs.Add(rsl);
            _context.SaveChanges();

            return status;
        }

        public async Task<int> closeCasePost(int rid, AViewNoteCase vnc)
        {
            var dbreq = await _context.Requests.FirstOrDefaultAsync(m => m.RequestId == rid);
            int status = dbreq?.Status ?? 1;
            var dbreqClient = await _context.Requestclients.FirstOrDefaultAsync(m => m.RequestId == rid);
            if (dbreqClient != null)
            {
                dbreqClient.PhoneNumber = vnc.mobile;
                dbreqClient.Email = vnc.email;
                _context.Requestclients.Update(dbreqClient);
                _context.SaveChanges();
            }
            return status;
        }
        public async Task<List<ViewUploadedDoc>> closeCase(int rid)
        {
            var dbreq = from r in _context.Requests
                        join rf in _context.Requestwisefiles on r.RequestId equals rf.RequestId
                        where rf.RequestId == rid
                        where rf.IsDeleted == false || rf.IsDeleted == null
                        select new { r, rf };

            var dbReqClient = await _context.Requestclients.FirstOrDefaultAsync(m => m.RequestId == rid);
            List<ViewUploadedDoc> data = new List<ViewUploadedDoc>();
            foreach (var item in dbreq)
            {
                var vu = new ViewUploadedDoc();
                vu.uploadDate = item.r.CreatedDate;
                vu.fileName = item.rf.FileName;
                vu.fileId = item.rf.RequestWiseFileId;
                vu.rid = rid;
                data.Add(vu);
            }
            return data;
        }
        public bool sendMail(string email, string? subject, string? message, string[] filenames = null)
        {
            var smtpClient = new SmtpClient("smtp.office365.com")
            {
                Port = 587,
                Credentials = new NetworkCredential("tatva.dotnet.shubhamvegad@outlook.com", "Vegad@12"),
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress("tatva.dotnet.shubhamvegad@outlook.com"),
                Subject = subject,

                Body = message,
                IsBodyHtml = true,
            };
            mailMessage.To.Add(email);

            foreach (var filename in filenames)
            {
                string baseFilePath = Path.Combine(_hostEnvironment.ContentRootPath, "wwwroot/uplodedItems");
                string fullFilePath = Path.Combine(baseFilePath, filename);
                if (System.IO.File.Exists(fullFilePath))
                {
                    mailMessage.Attachments.Add(new Attachment(fullFilePath));
                }
                else
                {
                    // Handle non-existent files (optional)
                    Console.WriteLine($"File not found: {fullFilePath}"); // Log or display an error message
                }
            }

            smtpClient.Send(mailMessage);
            return true;
        }
        public int sendAgreement(int rid)
        {
            string email = "";
            int status = 1;
            var dbreq = _context.Requests.FirstOrDefault(m => m.RequestId == rid);
            if (dbreq != null)
            {
                email = dbreq?.Email;
                status = dbreq?.Status ?? 1;
            }
            string newRid = rid.ToString();
            string subject = "halloDoc Agreement Update";
            string message = "Hii " + email + ", Review & agree to our updated Terms for continued process: " + "http://localhost:5011/pDashboard/agreement?Rid=" + newRid;
            string[] s = new string[1];
            var check = sendMail(email, subject, message);
            return status;
        }

        public int ClearCase(int rid)
        {
            var dbreq = _context.Requests.FirstOrDefault(m => m.RequestId == rid);
            int? status = dbreq?.Status;
            if (dbreq != null)
            {
                dbreq.Status = 10;
                _context.Update(dbreq);
                _context.SaveChanges();
            }
            Requeststatuslog rsl = new Requeststatuslog();
            rsl.RequestId = rid;
            rsl.Status = 10;
            rsl.AdminId = 1;
            rsl.CreatedDate = DateTime.Now;
            _context.Requeststatuslogs.Add(rsl);
            _context.SaveChanges();

            return status ?? 1;
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
                vnc.confNumber = item?.r?.ConfirmationNumber ?? "";
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
                dt.mobile = item.rc.PhoneNumber;
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
