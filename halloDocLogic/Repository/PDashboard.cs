using halloDocEntities.DataContext;
using halloDocEntities.ViewDataModels;
using halloDocLogic.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using halloDocEntities.DataModels;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.IO.Compression;
/*using Microsoft.Extensions.Hosting;*/

namespace halloDocLogic.Repository
{
    public class PDashboard : IPDashboard
    {
        private readonly ApplicationDbContext _context;
        private readonly IHostEnvironment _hostEnvironment;


        public PDashboard(ApplicationDbContext context, IHostEnvironment environment)
        {
            _context = context;
            _hostEnvironment = environment;
        }


        public bool agreement(int rid)
        {
            var dbreq = _context.Requests.FirstOrDefault(m => m.RequestId == rid);
            if (dbreq != null)
            {
                dbreq.Status = 3;
                _context.Requests.Update(dbreq);
                _context.SaveChanges();
            }
            return true;
        }

        public bool popupSubmit(string notes, int reqid)
        {
            Requeststatuslog rsl = new Requeststatuslog();
            rsl.Status = 10;
            rsl.Notes = notes;
            rsl.CreatedDate = DateTime.Now;
            rsl.RequestId = reqid;
            _context.Requeststatuslogs.Add(rsl);
            _context.SaveChanges();

            var dbreq = _context.Requests.FirstOrDefault(m => m.RequestId == reqid);    
            if (dbreq != null)
            {
                dbreq.Status = 10;
                _context.Requests.Update(dbreq);
                _context.SaveChanges();
            }
            return true;
        }
        public Task<bool> uploadtoid(History h, int reqid)
        {
            string filename1 = null;
            if (h.myfile != null)
            {
                string folder = "wwwroot/uplodedItems/";
                var key = Guid.NewGuid().ToString();
                folder += key + "_" + h.myfile.FileName;
                filename1 = key + "_" + h.myfile.FileName;
                string serverFolder = Path.Combine(_hostEnvironment.ContentRootPath, folder);

                using (var fileStream = new FileStream(serverFolder, FileMode.Create))
                {
                    fileStream.Position = 0;
                    h.myfile.CopyToAsync(fileStream);
                    fileStream.Flush();

                }
            }
            var rid = reqid;

            var rf = new Requestwisefile();
            rf.RequestId = rid;
            rf.CreatedDate = DateTime.Now;
            rf.FileName = filename1;
            _context.Requestwisefiles.Add(rf);
            _context.SaveChanges();
            return Task.FromResult(true);
        }

        public List<History> HistoryData(string aspid)
        {

            var userdb = _context.Users.FirstOrDefault(m => m.AspNetUserId == aspid);
            var userid = userdb?.UserId;


            var dbreq3 = from r in _context.Requests where r.UserId == userid select new { r };



            List<History> Calc = new List<History>();
            foreach (var item in dbreq3)
            {
                /*var count = _context.Requestwisefiles.Where(rf => rf.RequestId == item.r.RequestId).Select(rf => rf).Count();*/

                var nh = new History();
                nh.guid = item.r.RequestId;
                nh.status = (int)item.r.Status;
                nh.date = item.r.CreatedDate;
                nh.name = item.r.RequestId.ToString();

                Calc.Add(nh);

            }
            return Calc;
        }

        public string UserNameFromId(string aspid)
        {
            var userdb = _context.Users.FirstOrDefault(m => m.AspNetUserId == aspid);
            return string.Concat(userdb.FirstName, " ", userdb.LastName);

        }

        public List<History> ViewDocData(string aspid, int reqid)
        {
            /*var userdb =  _context.Users.FirstOrDefault(m => m.AspNetUserId == aspid);
            var userid = userdb?.UserId;*/

            var dbreq2 = from r in _context.Requests
                         join rf in _context.Requestwisefiles on r.RequestId equals rf.RequestId
                         where rf.RequestId == reqid
                         select new { r, rf };

            List<History> data = new List<History>();
            foreach (var item in dbreq2)
            {
                var nh = new History();
                nh.guid = item.r.RequestId;
                nh.uploder = string.Concat(item.r.FirstName, " ", item.r.LastName);
                nh.date = item.r.CreatedDate;
                nh.name = item.rf.FileName;

                data.Add(nh);

            }
            return data;
        }

        public profile ProfileData(string aspid, profile pr)
        {
            var userdb = _context.Users.FirstOrDefault(m => m.AspNetUserId == aspid);
            var userid = userdb?.UserId;


            pr.FirstName = userdb.FirstName;
            pr.LastName = userdb.LastName;
            pr.CreatedDate = userdb.CreatedDate;
            pr.Mobile = userdb.Mobile;
            pr.Email = userdb.Email;
            pr.Street = userdb.Street;
            pr.City = userdb.City;
            pr.State = userdb.State;
            pr.ZipCode = userdb.ZipCode;
            return pr;
        }

        public bool ProfileSubmit(string aspid, profile pr)
        {
            var userdb = _context.Users.FirstOrDefault(m => m.AspNetUserId == aspid);
            var userid = userdb?.UserId;

            userdb.FirstName = pr?.FirstName;
            userdb.LastName = pr?.LastName;
            userdb.CreatedDate = pr.CreatedDate;
            userdb.Mobile = pr?.Mobile;
            userdb.Email = pr?.Email;
            userdb.Street = pr?.Street;
            userdb.City = pr?.City;
            userdb.State = pr?.State;
            userdb.ZipCode = pr?.ZipCode;
            _context.Users.Update(userdb);
            _context.SaveChanges();

            return true;
        }
        public Stream downloadAll(string aspid)
        {
            var userdb = _context.Users.FirstOrDefault(m => m.AspNetUserId == aspid);
            var userid = userdb?.UserId;

            var dbreq2 = from r in _context.Requests
                         join rf in _context.Requestwisefiles on r.RequestId equals rf.RequestId
                         where r.UserId == userid
                         select new { r, rf };

            string baseFilePath = Path.Combine(_hostEnvironment.ContentRootPath, "wwwroot/uplodedItems");

            MemoryStream zipStream = new MemoryStream();
            using (var zipArchive = new ZipArchive(zipStream, ZipArchiveMode.Create, leaveOpen: true))
            {
                foreach (var document in dbreq2)
                {
                    string fullFilePath = Path.Combine(baseFilePath, document.rf.FileName);

                    string fileName = document.rf.FileName;
                    int index = fileName.LastIndexOf("/");
                    if (index != -1)
                        fileName = fileName.Substring(index + 1);
                    zipArchive.CreateEntryFromFile(fullFilePath, fileName);
                }
            } // disposal of archive will force data to be written to memory stream.
            zipStream.Position = 0; //reset memory stream position.
            return zipStream;
        }
        public Stream downloadSelected(string[] filenames)
        {
            string baseFilePath = Path.Combine(_hostEnvironment.ContentRootPath, "wwwroot/uplodedItems");

            MemoryStream zipStream = new MemoryStream();
            using (var zipArchive = new ZipArchive(zipStream, ZipArchiveMode.Create, leaveOpen: true))
            {
                foreach (var item in filenames)
                {
                    string fullFilePath = Path.Combine(baseFilePath, item);

                    string fileName = item;
                    int index = fileName.LastIndexOf("/");
                    if (index != -1)
                        fileName = fileName.Substring(index + 1);
                    zipArchive.CreateEntryFromFile(fullFilePath, fileName);
                }
            } // disposal of archive will force data to be written to memory stream.
            zipStream.Position = 0; //reset memory stream position.
            return zipStream;
        }

    }
}
