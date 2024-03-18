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

namespace halloDocLogic.Repository
{
    public class AdminProvider : IAdminProvider
    {
        private readonly ApplicationDbContext _context;
        public AdminProvider(ApplicationDbContext context) { 
            _context = context;
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
            foreach(var region in dbregion)
            {
                regionList.Add(region.Name);
            }

            List<AProvider> ap = new List<AProvider>();
            foreach (var item in dbprovider)
            {
                AProvider aProvider = new AProvider();
                aProvider.name = string.Concat(item.Physician.FirstName," ", item.Physician.LastName);
                aProvider.onCall = "unAvailable";
                aProvider.status = item.Physician.Status == 1 ? "Active" : "Pending";
                aProvider.role = item.RoleName.Name;
                aProvider.region = item.RegionName;
                aProvider.regionList = regionList;
                ap.Add(aProvider);
            }
            return ap;
        }


    }
}
