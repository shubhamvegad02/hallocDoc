using halloDocEntities.DataContext;
using halloDocEntities.ViewDataModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Nest;
using System.Collections;
using System.Xml.Linq;

namespace hallocDoc.Controllers
{
    public class ADashboardController : Controller
    {
        private readonly ApplicationDbContext _context;
        public ADashboardController(ApplicationDbContext dbContext)
        {
            _context = dbContext;
        }
        public async Task<IActionResult> Dmain(int n, ADashTable d)
        {
            if (n == 0)
            {
                n = 1;
            }
            /*var dbdata = from r in _context.Requests
                         join rc in _context.Requestclients on r.RequestId equals rc.RequestId
                         where r.Status == 1
                         select new { r, rc };*/
            ViewBag.heading = TempData["heading"];
            var dbdata = await (from r in _context.Requests
                                join rc in _context.Requestclients on r.RequestId equals rc.RequestId
                                where r.Status == n
                                select new { r, rc }).ToListAsync();


            List<ADashTable> dtable = new List<ADashTable>();
            foreach (var item in dbdata)
            {
                var dt = new ADashTable();
                dt.name = string.Concat(item.rc.FirstName, " ", item.rc.LastName);
                dt.email = item.rc.Email;
                dt.dob = item.r.CreatedDate;
                dt.requstor = item.r?.RelationName;
                dt.reqDate = item.r.CreatedDate.Date;
                dt.mobile = item.rc.PhoneNumber;
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
            }
            ViewBag.tableData = dtable;

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
    }
}
