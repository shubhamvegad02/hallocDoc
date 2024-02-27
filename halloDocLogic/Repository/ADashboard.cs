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

namespace halloDocLogic.Repository
{
    public class ADashboard : IADashboard
    {
        private readonly ApplicationDbContext _context;

        public ADashboard(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<ADashTable>>ADashTableData(int n)
        {
            var dbdata = from r in _context.Requests
                         join rc in _context.Requestclients on r.RequestId equals rc.RequestId
                         where r.Status == n
                         select new { r, rc };


            List<ADashTable> dtable = new List<ADashTable>();
            foreach (var item in dbdata)
            {
                var dt = new ADashTable();
                dt.guid = Guid.NewGuid().ToString();
                dt.name = string.Concat(item.rc.FirstName, " ", item.rc.LastName);
                dt.email = item.rc.Email;
                /*dt.dob = new DateTime(item.rc.IntYear.Value, int.Parse(item.rc.StrMonth), item.rc.IntDate.Value);*/
                dt.dob = item.r.CreatedDate.Date;
                dt.requstor = item.r?.RelationName;
                dt.reqDate = item.r.CreatedDate.Date;
                dt.mobile = item.r.PhoneNumber;
                dt.address = string.Concat(item.rc.Street, " ", item.rc.City, " ", item.rc.State);
                dt.notes = "";
                dt.region = item.rc.RegionId.ToString();
                /*var providerName = await _context.Physicians?.FirstOrDefaultAsync(m => m.PhysicianId == item.r.PhysicianId);
                if (providerName != null)
                {
                    dt.provider = providerName.FirstName;
                }*/
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
