using halloDocEntities.DataContext;
using halloDocEntities.DataModels;
using halloDocEntities.ViewDataModels;
using halloDocLogic.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Twilio.Clients;
using Twilio;
using Twilio.Types;
using Twilio.Rest.Api.V2010.Account;
using Org.BouncyCastle.Ocsp;

namespace halloDocLogic.Repository
{
    public class AdminProvider : IAdminProvider
    {
        private readonly ApplicationDbContext _context;
        private readonly IJwtService _jwtService;
        private readonly IFormRequest _formRequest;
        public AdminProvider(ApplicationDbContext context, IJwtService jwtService, IFormRequest formRequest)
        {
            _context = context;
            _jwtService = jwtService;
            _formRequest = formRequest;
        }


        public bool CreateProviderPost(EditPhysicianData edt)
        {
            Aspnetuser asp = new Aspnetuser();
            asp.Id = Guid.NewGuid().ToString();
            asp.UserName = edt.username;
            asp.Email = edt.email;
            asp.PasswordHash = _jwtService.encry(edt.password);
            asp.PhoneNumber = edt.mobile;
            asp.CreatedDate = DateTime.Now;
            _context.Aspnetusers.Add(asp);
            _context.SaveChanges();

            string photofile = "";
            if (edt?.photo != null)
            {
                photofile = _formRequest.uploadfile(edt?.photo);
            }

            Physician ph = new Physician();
            ph.AspNetUserId = asp.Id;
            ph.FirstName = edt.firstname;
            ph.LastName = edt.lastname;
            ph.Email = edt.email;
            ph.Mobile = edt.mobile;
            ph.CreatedDate = DateTime.Now;
            ph.Photo = photofile;
            ph.AdminNotes = edt.adminNotes;
            ph.Address1 = edt.address1;
            ph.Address2 = edt.address2;
            ph.City = edt.city;
            ph.RegionId = int.Parse(edt?.state);
            ph.Zip = edt.zipcode;
            ph.AltPhone = edt.billingMobile;
            /*ph.CreatedBy = "admin";*/
            ph.BusinessName = edt.businessname;
            ph.BusinessWebsite = edt.businesssite;
            ph.RoleId = int.Parse(edt.role);
            ph.MedicalLicense = edt.medicallicence;
            ph.Npinumber = edt.npinumber;
            _context.Physicians.Add(ph);
            _context.SaveChanges();

            var dbphysician = _context.Physicians.FirstOrDefault(m => m.AspNetUserId == ph.AspNetUserId);
            if (dbphysician != null)
            {
                int physicianId = dbphysician.PhysicianId;
                foreach (var row in edt.ProviderStateList)
                {
                    Physicianregion pr = new Physicianregion();
                    pr.PhysicianId = physicianId;
                    pr.RegionId = int.Parse(row);
                    _context.Physicianregions.Add(pr);
                }
                    _context.SaveChanges();
            }

            return true;
        }

        public EditPhysicianData CreteProvider()
        {
            EditPhysicianData epd = new EditPhysicianData();
            var dbrole = _context.Roles.Where(m => m.AccountType == 3);
            List<Role> roleList = new List<Role>();
            foreach (var i in dbrole)
            {
                roleList.Add(i);
            }

            var dbregion = _context.Regions;
            List<halloDocEntities.DataModels.Region> regions = new List<halloDocEntities.DataModels.Region>();
            foreach (var i in dbregion)
            {
                regions.Add(i);
            }
            epd.roleList = roleList;
            epd.stateList = regions;
            return epd;
        }
        public bool DeleteProvider(int physicianId)
        {
            var dbphysician = _context.Physicians.FirstOrDefault(m => m.PhysicianId == physicianId);
            if (dbphysician != null)
            {
                dbphysician.Isdeleted = true;
                _context.Physicians.Update(dbphysician);
                _context.SaveChanges();
            }

            return true;
        }

        public bool EditProviderForm4(int physicianId, EditPhysicianData edt)
        {
            string photofile = "";
            string signfile = "";
            if (edt?.photo != null)
            {
                photofile = _formRequest.uploadfile(edt?.photo);
            }
            if (edt?.sign != null)
            {
                signfile = _formRequest.uploadfile(edt?.sign);
            }
            var dbphysician = _context.Physicians.FirstOrDefault(m => m.PhysicianId == physicianId);
            if (dbphysician != null)
            {
                dbphysician.BusinessName = edt.businessname ?? dbphysician.BusinessName;
                dbphysician.BusinessWebsite = edt?.businesssite ?? dbphysician.BusinessWebsite;
                dbphysician.AdminNotes = edt?.adminNotes ?? dbphysician.AdminNotes;
                dbphysician.Mobile = edt?.mobile ?? dbphysician.Mobile;
                dbphysician.Zip = edt?.zipcode ?? dbphysician.Zip;
                if (photofile != null)
                {
                    dbphysician.Photo = photofile;
                }
                if (signfile != null)
                {
                    dbphysician.Signature = signfile;
                }

                _context.Physicians.Update(dbphysician);
                _context.SaveChanges();
            }
            return true;
        }

        public bool EditProviderForm3(int physicianId, EditPhysicianData edt)
        {
            var dbphysician = _context.Physicians.FirstOrDefault(m => m.PhysicianId == physicianId);
            if (dbphysician != null)
            {
                dbphysician.Address1 = edt.address1 ?? dbphysician.Address1;
                dbphysician.Address2 = edt?.address2 ?? dbphysician.Address2;
                dbphysician.City = edt?.city ?? dbphysician.City;
                dbphysician.Mobile = edt?.mobile ?? dbphysician.Mobile;
                dbphysician.Zip = edt?.zipcode ?? dbphysician.Zip;
                if (edt.state != null)
                {
                    dbphysician.RegionId = int.Parse(edt?.state);
                }
                _context.Physicians.Update(dbphysician);
                _context.SaveChanges();
            }
            return true;
        }

        public bool EditProviderForm2(int physicianId, EditPhysicianData edt)
        {
            string aspid = "";
            var dbphysician = _context.Physicians.FirstOrDefault(m => m.PhysicianId == physicianId);
            if (dbphysician != null)
            {
                aspid = dbphysician?.AspNetUserId;
                dbphysician.Email = edt.email ?? dbphysician.Email;
                dbphysician.FirstName = edt.firstname ?? dbphysician.FirstName;
                dbphysician.LastName = edt?.lastname ?? dbphysician.LastName;
                dbphysician.Mobile = edt?.mobile ?? dbphysician.Mobile;
                dbphysician.MedicalLicense = edt?.medicallicence ?? dbphysician.MedicalLicense;
                dbphysician.SyncEmailAddress = edt?.semail ?? dbphysician.SyncEmailAddress;
                dbphysician.Npinumber = edt?.npinumber ?? dbphysician.Npinumber;
                _context.Physicians.Update(dbphysician);
                _context.SaveChanges();
            }
            var dbasp = _context.Aspnetusers.FirstOrDefault(m => m.Id == aspid);
            if (dbasp != null)
            {
                dbasp.Email = edt.email ?? dbasp.Email;
                dbasp.UserName = edt.email ?? dbasp.UserName;
                _context.Aspnetusers.Update(dbasp);
                _context.SaveChanges();
            }

            if (edt?.ProviderStateList != null)
            {
                var dbphyregion = _context.Physicianregions.Where(m => m.PhysicianId == physicianId);
                _context.Physicianregions.RemoveRange(dbphyregion);
                _context.SaveChanges();

                foreach (var row in edt.ProviderStateList)
                {
                    Physicianregion pr = new Physicianregion();
                    pr.PhysicianId = physicianId;
                    pr.RegionId = int.Parse(row);
                    _context.Physicianregions.Add(pr);
                    _context.SaveChanges();
                }
            }
            return true;
        }


        public bool EditProviderForm1(int physicianId, EditPhysicianData edt)
        {
            string aspid = "";
            var dbphysician = _context.Physicians.FirstOrDefault(m => m.PhysicianId == physicianId);
            if (dbphysician != null)
            {
                aspid = dbphysician?.AspNetUserId;
                dbphysician.Email = edt.username ?? dbphysician.Email;
                dbphysician.Status = (short?)edt.status ?? dbphysician.Status;
                dbphysician.RoleId = int.Parse(edt?.role);
                _context.Physicians.Update(dbphysician);
                _context.SaveChanges();
            }
            var dbasp = _context.Aspnetusers.FirstOrDefault(m => m.Id == aspid);
            if (dbasp != null)
            {
                dbasp.PasswordHash = _jwtService.encry(edt.password) ?? dbasp.PasswordHash;
                dbasp.Email = edt.username ?? dbasp.Email;
                dbasp.UserName = edt.username ?? dbasp.UserName;
                _context.Aspnetusers.Update(dbasp);
                _context.SaveChanges();
            }
            return true;
        }

        public async Task<EditPhysicianData> EditPhysician(int physicianId)
        {
            var dbrole = _context.Roles;
            List<Role> roleList = new List<Role>();
            foreach (var i in dbrole)
            {
                roleList.Add(i);
            }

            var dbregion = _context.Regions;
            List<halloDocEntities.DataModels.Region> regions = new List<halloDocEntities.DataModels.Region>();
            foreach (var i in dbregion)
            {
                regions.Add(i);
            }

            var dbProviderRegionList = from pr in _context.Physicianregions
                                       join r in _context.Regions on pr.RegionId equals r.RegionId
                                       where pr.PhysicianId == physicianId
                                       select r.Name;
            List<string> providerRegion = new List<string>();
            foreach (var region in dbProviderRegionList)
            {
                providerRegion.Add(region);
            }


            var dbdata = from asp in _context.Aspnetusers
                         join physician in _context.Physicians on asp.Id equals physician.AspNetUserId
                         join region in _context.Regions on physician.RegionId equals region.RegionId
                         join role in _context.Roles on physician.RoleId equals role.RoleId
                         where physician.PhysicianId.Equals(physicianId)

                         select new
                         {
                             aspData = asp,
                             physicianData = physician,
                             RoleName = role.Name,
                             regionName = region.Name
                         };
            foreach (var item in dbdata)
            {
                EditPhysicianData epd = new EditPhysicianData();
                epd.PId = physicianId;
                epd.username = item.aspData.UserName;
                epd.status = item.physicianData.Status;
                epd.role = item.RoleName;
                epd.roleList = roleList;
                epd.firstname = item.physicianData.FirstName;
                epd.lastname = item.physicianData.LastName;
                epd.email = item.physicianData.Email;
                epd.mobile = item.physicianData.Mobile;
                epd.medicallicence = item.physicianData.MedicalLicense;
                epd.npinumber = item.physicianData.Npinumber;
                epd.semail = item.physicianData.SyncEmailAddress;
                epd.ProviderStateList = providerRegion;
                epd.stateList = regions;
                epd.address1 = item.physicianData.Address1;
                epd.address2 = item.physicianData.Address2;
                epd.city = item.physicianData.City;
                epd.state = item.regionName;
                epd.zipcode = item.physicianData.Zip;
                epd.billingMobile = item.physicianData.AltPhone;
                epd.businessname = item.physicianData.BusinessName;
                epd.businesssite = item.physicianData.BusinessWebsite;
                epd.adminNotes = item.physicianData.AdminNotes;
                epd.BC = item.physicianData.Isbackgrounddoc;
                epd.NDA = item.physicianData.Isnondisclosuredoc;
                epd.LD = item.physicianData.Islicensedoc;
                return epd;
            }
            return null;
        }
        public async Task sendSMS(string number, string msg)
        {
            string mobile = "+91" + number;
            string accountSid = "ACb2bf4b28632e754a0d01df01642de566";
            string authToken = "afeaa8e139fbe82e721c97500278cab5";

            TwilioClient.Init(accountSid, authToken);

            var message = MessageResource.Create(
                body: msg,
                from: new Twilio.Types.PhoneNumber("+15209993920"),
                to: new Twilio.Types.PhoneNumber(mobile)
            );
            Console.WriteLine(message.Sid);
        }

        public bool MailToProvider(int physicianId)
        {

            return true;
        }

        public List<AProvider> Provider()
        {
            var dbprovider = from physician in _context.Physicians
                             join region in _context.Regions on physician.RegionId equals region.RegionId into regionGroup
                             from region in regionGroup.DefaultIfEmpty()
                             join role in _context.Roles on physician.RoleId equals role.RoleId into roleGroup
                             from role in roleGroup.DefaultIfEmpty()
                             where (physician.Isdeleted != true)
                             select new
                             {
                                 Physician = physician,
                                 RegionName = region.Name,
                                 RoleName = role
                             };

            var dbregion = _context.Regions;
            List<string> regionList = new List<string>();
            foreach (var region in dbregion)
            {
                regionList.Add(region.Name);
            }

            List<AProvider> ap = new List<AProvider>();
            foreach (var item in dbprovider)
            {
                AProvider aProvider = new AProvider();
                aProvider.name = string.Concat(item.Physician.FirstName, " ", item.Physician.LastName);
                aProvider.onCall = "unAvailable";
                aProvider.status = item.Physician.Status == 1 ? "Active" : "Pending";
                aProvider.role = item.RoleName.Name;
                aProvider.region = item.RegionName;
                aProvider.physicianId = item.Physician.PhysicianId;
                aProvider.email = item.Physician.Email;
                aProvider.regionList = regionList;
                ap.Add(aProvider);
            }
            return ap;
        }


    }
}
