using hallocDoc.DataContext;
using hallocDoc.DataModels;
using hallocDoc.ViewDataModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Nest;
using Npgsql.EntityFrameworkCore.PostgreSQL.Infrastructure.Internal;
using System.IO;


namespace hallocDoc.Controllers
{
    public class ReqFormController : Controller
    {
        public Boolean V = false;

        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        

        public ReqFormController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }


        public IActionResult Business()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Business(businessReq br)
        {
            if (ModelState.IsValid && br != null)
            {
                var request = new Request();

                request.RequestTypeId = 1;
                /*request.UserId = user.UserId;*/
                request.FirstName = br.BFirstName;
                request.LastName = br.BLastName;
                request.CreatedDate = DateTime.Now;
                request.PhoneNumber = br.BMobile;
                request.Email = br.BEmail;
                request.CaseNumber = br.CaseNumber;
                request.RelationName = "Business Person";

                await _context.Requests.AddAsync(request);
                await _context.SaveChangesAsync();

                var rc = new Requestclient();

                rc.RequestId = request.RequestId;
                rc.FirstName = br.FirstName;
                rc.LastName = br.LastName;
                rc.PhoneNumber = br.Mobile;
                rc.Notes = br.Notes;
                rc.Street = br.Street;
                rc.City = br.City;
                rc.State = br.State;
                rc.ZipCode = br.ZipCode;
                rc.Email = br.Email;
                rc.Address = br.Address;
                _context.Requestclients.Add(rc);
                _context.SaveChanges();

                var dbdata = await _context.Businesses.FirstOrDefaultAsync(m => m.Name == br.BusinessName);
                var bid = 0;
                if (dbdata == null)
                {
                    var b = new Business();

                    b.Name = br.BusinessName;
                    b.Address1 = br.Address;
                    b.City = br.City;
                    b.PhoneNumber = br.BMobile;
                    _context.Businesses.Add(b);
                    _context.SaveChanges();
                    bid = b.BusinessId;
                }

                else
                {
                    bid = dbdata.BusinessId;
                }


                var rb = new Requestbusiness();
                rb.BusinessId = bid;
                rb.RequestId = request.RequestId;
                _context.Requestbusinesses.Add(rb);
                _context.SaveChanges();





                return RedirectToAction("first", "Home");
            }
            return View(br);
        }

        public IActionResult Concierge()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Concierge(conciergeReq cr)
        {
            if (ModelState.IsValid && cr != null)
            {
                var request = new Request();

                request.RequestTypeId = 1;
                request.FirstName = cr.CFN;
                request.LastName = cr.CLN;
                request.CreatedDate = DateTime.Now;
                request.PhoneNumber = cr.CMobile;
                request.Email = cr.CMail;
                request.CreatedDate = DateTime.Now;
                request.RelationName = "Concierge";

                await _context.Requests.AddAsync(request);
                await _context.SaveChangesAsync();

                var rc = new Requestclient();

                rc.RequestId = request.RequestId;
                rc.FirstName = cr.FirstName;
                rc.LastName = cr.LastName;
                rc.PhoneNumber = cr.Mobile;
                rc.Notes = cr.Notes;
                rc.Street = cr.Street;
                rc.City = cr.City;
                rc.State = cr.State;
                rc.ZipCode = cr.ZipCode;
                rc.Email = cr.Email;
                rc.Address = cr.Address;
                _context.Requestclients.Add(rc);
                _context.SaveChanges();

                var c = new Concierge();
                c.ConciergeName = cr.CFN;
                c.Address = cr.property;
                c.State = cr.State;
                c.City = cr.City;
                c.Street = cr.Street;
                c.ZipCode = cr.ZipCode;
                c.CreatedDate = cr.CreatedDate;
                /*await _context.Concierge.AddAsync(c);*/
                await _context.SaveChangesAsync();



                return RedirectToAction("first", "Home");
            }
            return View(cr);
        }


        public IActionResult family()
        {

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> family(familyReq fr)
        {
            if (ModelState.IsValid && fr != null)
            {
                string filename = null;

                if (fr.myfile != null)
                {
                    string folder = "uplodedItems/";
                    var key = Guid.NewGuid().ToString();
                    folder += key + "_" + fr.myfile.FileName;
                    filename = key + "_" + fr.myfile.FileName;
                    string serverFolder = Path.Combine(_webHostEnvironment.WebRootPath, folder);

                    await fr.myfile.CopyToAsync(new FileStream(serverFolder, FileMode.Create));

                }

                var request = new Request();

                request.RequestTypeId = 1;
                /*request.UserId = user.UserId;*/
                request.FirstName = fr.FFirstName;
                request.LastName = fr.FLastName;
                request.CreatedDate = DateTime.Now;
                request.PhoneNumber = fr.FPhoneNumber;
                request.Email = fr.FEmail;
                request.CreatedDate = DateTime.Now;
                request.RelationName = fr.RelationName;

                await _context.Requests.AddAsync(request);
                await _context.SaveChangesAsync();

                var rc = new Requestclient();

                rc.RequestId = request.RequestId;
                rc.FirstName = fr.FirstName;
                rc.LastName = fr.LastName;
                rc.PhoneNumber = fr.Mobile;
                rc.Notes = fr.Notes;
                rc.Street = fr.Street;
                rc.City = fr.City;
                rc.State = fr.State;
                rc.ZipCode = fr.ZipCode;
                rc.Email = fr.Email;
                rc.Address = fr.Address;
                _context.Requestclients.Add(rc);
                _context.SaveChanges();

                var rf = new Requestwisefile();
                rf.RequestId = request.RequestId;
                rf.CreatedDate = DateTime.Now;
                rf.FileName = filename;
                _context.Requestwisefiles.Add(rf);
                _context.SaveChanges();



                return RedirectToAction("first", "Home");
            }

            return View(fr);
        }

        public IActionResult Patient()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Patient(patientReq pr)
        {
            string filename = null;
            if (ModelState.IsValid && pr != null)
            {
                if(pr.myfile != null)
                {
                    string folder = "uplodedItems/";
                    var key = Guid.NewGuid().ToString();
                    folder += key + "_" + pr.myfile.FileName;
                    filename = key + "_" + pr.myfile.FileName;
                    string serverFolder = Path.Combine(_webHostEnvironment.WebRootPath, folder);
                    

                    /*await pr.myfile.CopyToAsync(new FileStream(serverFolder, FileMode.Create));*/

                    using (var fileStream = new FileStream(serverFolder, FileMode.Create))
                    {
                        fileStream.Position = 0;
                        await pr.myfile.CopyToAsync(fileStream);
                        fileStream.Flush();

                    }
                }
            

            var dbasp = await _context.Aspnetusers.FirstOrDefaultAsync(m => m.Email == pr.Email);
                var aspid = dbasp?.Id;
                var dbuser = await _context.Users.FirstOrDefaultAsync(m => m.AspNetUserId == aspid);
                if (dbasp != null)
                {
                    var request2 = new Request();
                    request2.RequestTypeId = 1;
                    request2.UserId = dbuser.UserId;
                    request2.FirstName = pr.FirstName;
                    request2.LastName = pr.LastName;
                    request2.CreatedDate = DateTime.Now;
                    request2.PhoneNumber = pr.Mobile;
                    request2.Email = pr.Email;
                    request2.Status = 2;

                    _context.Requests.Add(request2);
                    _context.SaveChanges();

                    var rc2 = new Requestclient();

                    rc2.RequestId = request2.RequestId;
                    rc2.FirstName = request2.FirstName;
                    rc2.LastName = request2.LastName;
                    rc2.PhoneNumber = request2.PhoneNumber;
                    rc2.Notes = pr.Notes;
                    rc2.Street = pr.Street;
                    rc2.City = pr.City;
                    rc2.State = pr.State;
                    rc2.ZipCode = pr.ZipCode;
                    rc2.Email = pr.Email;
                    _context.Requestclients.Add(rc2);
                    _context.SaveChanges();

                    var rf = new Requestwisefile();
                    rf.RequestId = request2.RequestId;
                    rf.CreatedDate = DateTime.Now;
                    rf.FileName = filename;
                    _context.Requestwisefiles.Add(rf);
                    _context.SaveChanges();

                    return RedirectToAction("first", "Home");
                }
                else
                {



                    var aspnetuser = new Aspnetuser();
                    aspnetuser.Id = Guid.NewGuid().ToString();
                    /*aspnetuser.PasswordHash = password.encry(pr.password);*/
                    aspnetuser.Email = pr.Email;
                    aspnetuser.CreatedDate = DateTime.Now;
                    aspnetuser.UserName = pr.Email;
                    aspnetuser.PhoneNumber = pr.Mobile;

                    await _context.Aspnetusers.AddAsync(aspnetuser);
                    _context.SaveChanges();

                    var user = new User();

                    user.AspNetUserId = aspnetuser.Id;
                    user.FirstName = pr.FirstName;
                    user.LastName = pr.LastName;
                    user.Mobile = pr.Mobile;
                    user.Email = pr.Email;
                    user.Street = pr.Street;
                    user.City = pr.City;
                    user.State = pr.State;
                    user.ZipCode = pr.ZipCode;
                    user.CreatedBy = pr.FirstName;
                    user.Mobile = pr.Mobile;
                    user.CreatedDate = DateTime.Now;


                    _context.Users.Add(user);
                    _context.SaveChanges();

                    var request = new Request();

                    request.RequestTypeId = 1;
                    request.UserId = user.UserId;
                    request.FirstName = pr.FirstName;
                    request.LastName = pr.LastName;
                    request.CreatedDate = DateTime.Now;
                    request.PhoneNumber = pr.Mobile;
                    request.Email = pr.Email;
                    request.PhoneNumber = pr.Mobile;
                    request.CreatedDate = DateTime.Now;
                    request.Status = 2;

                    _context.Requests.Add(request);
                    _context.SaveChanges();

                    var rc = new Requestclient();

                    rc.RequestId = request.RequestId;
                    rc.FirstName = request.FirstName;
                    rc.LastName = request.LastName;
                    rc.PhoneNumber = request.PhoneNumber;
                    rc.Notes = pr.Notes;
                    rc.Street = pr.Street;
                    rc.City = pr.City;
                    rc.State = pr.State;
                    rc.ZipCode = pr.ZipCode;
                    rc.Email = pr.Email;
                    _context.Requestclients.Add(rc);
                    _context.SaveChanges();

                    var rf = new Requestwisefile();
                    rf.RequestId = request.RequestId;
                    rf.CreatedDate = DateTime.Now;
                    rf.FileName = filename;
                    _context.Requestwisefiles.Add(rf);
                    _context.SaveChanges();



                    return RedirectToAction("createPatient", "Home", new {aspid=aspnetuser.Id});
                }
            }
            /*return RedirectToAction("Privacy", "Home");*/
            return View(pr);

        }


        public IActionResult SubmitReq()
        {

            const bool V = true;
            ViewBag.hidebackbtn = V;
            return View();
        }
    }
}
