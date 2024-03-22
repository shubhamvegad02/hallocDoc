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
using Elasticsearch.Net;
/*using Nest;*/

namespace halloDocLogic.Repository
{
    public class ADashboard : IADashboard
    {
        private readonly ApplicationDbContext _context;
        private readonly IHostEnvironment _hostEnvironment;
        private readonly IJwtService _jwtService;

        public ADashboard(ApplicationDbContext context, IHostEnvironment environment, IJwtService jwtService)
        {
            _context = context;
            _hostEnvironment = environment;
            _jwtService = jwtService;
        }



        public List<Request> ReqData()
        {
            List<Request> reqData = new List<Request>();
            reqData = _context.Requests.ToList();
            return reqData;
        }
        public async Task<bool> TransferCase(int rid, ADashTable adt)
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

            return true;
        }

        public async Task<object> GetOrderData(int businessId)
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
            return business;
        }
        public async Task<int> statusFromRid(int rid)
        {
            var dbReq = await _context.Requests.FirstOrDefaultAsync(m => m.RequestId == rid);
            if (dbReq != null)
            {
                return dbReq?.Status ?? 1;
            }
            return 0;
        }

        public async Task<SendOrder> order()
        {
            var dbProType = await _context.Healthprofessionaltypes.ToListAsync();
            var dbpro = await _context.Healthprofessionals.ToListAsync();


            var s = new SendOrder
            {
                professionList = dbProType,
                businessList = dbpro
            };
            return s;
        }

        public bool OrderPost(int rid, SendOrder so)
        {
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
                _context.Orderdetails.AddAsync(od);
                _context.SaveChanges();
                return true;
            }
            return false;
        }

        public async Task<List<ViewUploadedDoc>> ViewUpload(int rid)
        {
            var dbreq = from r in _context.Requests
                        join rf in _context.Requestwisefiles on r.RequestId equals rf.RequestId
                        where rf.RequestId == rid
                        where rf.IsDeleted == false || rf.IsDeleted == null
                        select new { r, rf };

            var dbReqClient = await _context.Requestclients.FirstOrDefaultAsync(m => m.RequestId == rid);
            string patientName = string.Concat(dbReqClient.FirstName, " ", dbReqClient.LastName);
            string con = "";
            List<ViewUploadedDoc> data = new List<ViewUploadedDoc>();
            foreach (var item in dbreq)
            {
                var vu = new ViewUploadedDoc();

                vu.confirmation = item?.r?.ConfirmationNumber;
                vu.uploadDate = item.r.CreatedDate;
                vu.fileName = item.rf.FileName;
                vu.fileId = item.rf.RequestWiseFileId;
                vu.rid = rid;
                vu.patientName = patientName;

                data.Add(vu);
            }
            return data;
        }

        public bool MyProfilePost(int aid, AdminProfile ap)
        {
            string aspid = "";
            var dbadmin = _context.Admins.FirstOrDefault(m => m.AdminId == aid);
            if (dbadmin != null)
            {
                aspid = dbadmin.AspNetUserId;
                dbadmin.FirstName = ap?.firstname ?? dbadmin.FirstName;
                dbadmin.LastName = ap?.lastname ?? dbadmin.LastName;
                dbadmin.Email = ap?.email ?? dbadmin.Email;
                dbadmin.Mobile = ap?.mobile ?? dbadmin.Mobile;
                dbadmin.Address1 = ap?.address1 ?? dbadmin.Address1;
                dbadmin.Address2 = ap?.address2 ?? dbadmin.Address2;
                dbadmin.City = ap?.city ?? dbadmin.City;
                dbadmin.Zip = ap?.zipcode ?? dbadmin.Zip;
                if (ap?.state != null)
                {
                    dbadmin.RegionId = int.Parse(ap?.state);
                }
                _context.Admins.Update(dbadmin);
                _context.SaveChanges();
            }

            var dbasp = _context.Aspnetusers.FirstOrDefault(m => m.Id == aspid);
            if (dbasp != null)
            {
                dbasp.UserName = ap.email ?? dbasp.UserName;
                dbasp.Email = ap.email ?? dbasp.Email;
                dbasp.PhoneNumber = ap.mobile ?? dbasp.PhoneNumber;
                _context.Aspnetusers.Update(dbasp);
                _context.SaveChanges();
            }
            if (ap?.AdminStateList?.Count > 0)
            {
                var rowsToRemove = _context.AdminRegions.Where(m => m.AdminId == aid);
                _context.AdminRegions.RemoveRange(rowsToRemove);
                _context.SaveChanges();

                foreach (var row in ap.AdminStateList)
                {
                    AdminRegion ar = new AdminRegion();
                    ar.AdminId = aid;
                    /*ar.RegionId = row;*/
                    ar.RegionId = int.Parse(row);
                    _context.AdminRegions.Add(ar);
                    _context.SaveChanges();
                }
            }

            return true;
        }

        public bool ProfilePasswordSubmit(int aid, AdminProfile ap)
        {
            var dbadmin = _context.Admins.FirstOrDefault(m => m.AdminId == aid);
            if (dbadmin != null)
            {
                string aspid = dbadmin.AspNetUserId;
                if (aspid != null && ap.password != null)
                {
                    var dbasp = _context.Aspnetusers.FirstOrDefault(m => m.Id == aspid);
                    dbasp.PasswordHash = _jwtService.encry(ap?.password);
                    _context.Aspnetusers.Update(dbasp);
                    _context.SaveChanges();
                }
            }
            return true;
        }


        public async Task<AdminProfile> MyProfile(int aid)
        {
            var dbadminRegion = from ar in _context.AdminRegions
                                join r in _context.Regions on ar.RegionId equals r.RegionId
                                where ar.AdminId == aid
                                select r.Name;
            List<string> adminRegion = new List<string>();
            foreach (var region in dbadminRegion)
            {
                adminRegion.Add(region);
            }


            var dbregion = _context.Regions;
            List<string> list = new List<string>();
            List<Region> regions = new List<Region>();
            foreach (var i in dbregion)
            {
                regions.Add(i);
                list.Add(i?.Name);
            }


            var dbdata = from user in _context.Aspnetusers
                         join admin in _context.Admins on user.Id equals admin.AspNetUserId
                         join userRole in _context.Aspnetuserroles on user.Id equals userRole.UserId
                         join role in _context.Aspnetroles on userRole.RoleId equals role.AspNetRoleId
                         join region in _context.Regions on admin.RegionId equals region.RegionId

                         select new
                         {
                             aspData = user,
                             AdminData = admin,
                             RoleName = role.Name,
                             RegionName = region.Name
                         };
            foreach (var item in dbdata)
            {
                AdminProfile ap = new AdminProfile();
                ap.username = item?.aspData?.Email;
                ap.status = item?.AdminData?.Status?.ToString();
                ap.role = item?.RoleName;
                ap.firstname = item?.AdminData?.FirstName;
                ap.lastname = item?.AdminData?.LastName;
                ap.email = item?.AdminData?.Email;
                ap.confirmMail = item?.AdminData?.Email;
                ap.mobile = item?.AdminData?.Mobile;
                ap.address1 = item?.AdminData?.Address1;
                ap.address2 = item?.AdminData?.Address2;
                ap.city = item?.AdminData?.City;
                ap.zipcode = item?.AdminData?.Zip;
                ap.billingMobile = item?.AdminData?.AltPhone ?? item?.AdminData.Mobile;
                ap.stateList = list;
                ap.AdminStateList = adminRegion;
                ap.state = item?.RegionName;
                ap.regionList = regions;

                return ap;
            }
            return null;

        }
        public int AdminidFromAspid(string aspid)
        {
            var dbadmin = _context.Admins.FirstOrDefault(m => m.AspNetUserId == aspid);
            if (dbadmin != null)
            {
                int Aid = dbadmin.AdminId;
                return Aid;
            }
            return 0;
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

            if (filenames != null)
            {

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
            }

            smtpClient.Send(mailMessage);
            return true;
        }

        public string encry(string pass)
        {
            if (pass == null)
            {
                return null;
            }
            else
            {
                byte[] storePass = ASCIIEncoding.ASCII.GetBytes(pass);
                string encryptedPass = Convert.ToBase64String(storePass);
                return encryptedPass;
            }

        }
        public int sendAgreement(int rid)
        {
            string email = "vegadshubham2002@gmail.com";
            int status = 1;
            var dbreq = _context.Requests.FirstOrDefault(m => m.RequestId == rid);
            if (dbreq != null)
            {
                /*email = dbreq?.Email;*/
                status = dbreq?.Status ?? 1;
            }

            string newRid = encry(rid.ToString());
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
        public AViewNoteCase VNData(int rid, AViewNoteCase? vnc)
        {
            try
            {
                var dbnotedata = from rn in _context.Requestnotes.DefaultIfEmpty()
                                 join rl in _context.Requeststatuslogs on rn.RequestId equals rl.RequestId into temp
                                 where rn.RequestId == rid
                                 select new { rl = temp.FirstOrDefault(), rn };

                /*var dbreq = _context.Requests.FirstOrDefault(m => m.RequestId == 5000);
                vnc.status = dbreq.Status;*/

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
                    vnc.Tnotes = item?.rl?.Notes;
                    vnc.Pnotes = item?.rn?.PhysicianNotes;
                    vnc.Anotes = item?.rn?.AdminNotes;
                    vnc.rid = rid;
                }
                return vnc;

            }
            catch (Exception ex)
            {
                return new AViewNoteCase();
            }
        }
        public async Task<bool> VNDatapost(AViewNoteCase vnc)
        {
            int rid = vnc.rid;
            var dbreqnotes = await _context.Requestnotes.FirstOrDefaultAsync(m => m.RequestId == rid);
            if (dbreqnotes != null)
            {
                dbreqnotes.AdminNotes = vnc.TempAnotes;
                _context.Requestnotes.Update(dbreqnotes);
                _context.SaveChanges();
            }
            else
            {
                Requestnote rn = new Requestnote();
                rn.AdminNotes = vnc.TempAnotes;
                rn.RequestId = rid;
                rn.CreatedBy = "admin";
                rn.CreatedDate = DateTime.Now;
                _context.Requestnotes.Update(rn);
                _context.SaveChanges();
            }
            vnc.TempAnotes = "";

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

        public List<Physician> getPhysiciandata()
        {
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
            return plist;
        }


        public List<Region> getRegiondata()
        {
            List<Region> rlist = new List<Region>();
            var dbregion = _context.Regions;
            foreach (var item in dbregion)
            {
                Region r = new Region();
                r.RegionId = item.RegionId;
                r.Name = item.Name;
                rlist.Add(r);
            }
            return rlist;
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
