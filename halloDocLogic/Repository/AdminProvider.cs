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

namespace halloDocLogic.Repository
{
    public class AdminProvider : IAdminProvider
    {
        private readonly ApplicationDbContext _context;
        public AdminProvider(ApplicationDbContext context)
        {
            _context = context;
        }


        
        public async Task<EditPhysicianData> EditPhysician(int physicianId)
        {
            var dbdata = from asp in _context.Aspnetusers
                         join physician in _context.Physicians on asp.Id equals physician.AspNetUserId
                         join userRole in _context.Aspnetuserroles on asp.Id equals userRole.UserId
                         join role in _context.Aspnetroles on userRole.RoleId equals role.AspNetRoleId
                         join region in _context.Regions on physician.RegionId equals region.RegionId
                         join physicianRegion in _context.Physicianregions on physician.PhysicianId equals physicianRegion.PhysicianId

                         select new
                         {
                             aspData = asp,
                             physicianData = physician,
                             RoleName = role.Name,
                             RegionName = region.Name,
                             physicianregion = physicianRegion,
                         };


            var dbdata1 = from asp in _context.Aspnetusers
                         join physician in _context.Physicians on asp.Id equals physician.AspNetUserId
                         join role in _context.Roles on physician.RoleId equals role.RoleId  // Join with Roles directly
                         join region in _context.Regions on physician.RegionId equals region.RegionId
                         join physicianRegion in _context.Physicianregions on physician.PhysicianId equals physicianRegion.PhysicianId

                         select new
                         {
                             aspData = asp,
                             physicianData = physician,
                             RoleName = role.Name,
                             RegionName = region.Name,
                             physicianregion = physicianRegion,
                         };

            EditPhysicianData ep = new EditPhysicianData();
            return ep;
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
