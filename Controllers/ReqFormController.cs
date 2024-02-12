using hallocDoc.DataContext;
using hallocDoc.DataModels;
using hallocDoc.ViewDataModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Npgsql.EntityFrameworkCore.PostgreSQL.Infrastructure.Internal;


namespace hallocDoc.Controllers
{
    public class ReqFormController : Controller
    {

        private readonly ApplicationDbContext _context;

        public ReqFormController(ApplicationDbContext context)
        {
            _context = context;
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
        public IActionResult family()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> family(familyReq fr)
        {
            if (ModelState.IsValid && fr != null)
            {
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

                _context.Requests.Add(request);
                _context.SaveChanges();

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
            if (ModelState.IsValid && pr != null)
            {
                /*var dbdata = await _context.Aspnetusers.FirstOrDefaultAsync(m => m.Email == pr.Email);
                if (dbdata != null)
                {
                    const int V = 1;
                    ViewBag.hidebackbtn = V;

                    if (dbdata.PasswordHash == pr.PasswordHash)
                    {
                        var nrequest = new Request();
                        var nuser = _context.Users.FirstOrDefault(m => m.AspNetUserId == dbdata.Id);
                        nrequest.RequestTypeId = 1;
                        nrequest.UserId = nuser.UserId;
                        nrequest.FirstName = pr.FirstName;
                        nrequest.LastName = pr.LastName;
                        nrequest.CreatedDate = DateTime.Now;
                        nrequest.PhoneNumber = pr.Mobile;
                        nrequest.Email = pr.Email;
                        nrequest.PhoneNumber = pr.Mobile;
                        nrequest.CreatedDate = DateTime.Now;

                        _context.Requests.Add(nrequest);
                        _context.SaveChanges();

                        var nrc = new Requestclient();

                        nrc.RequestId = nrequest.RequestId;
                        nrc.FirstName = pr.FirstName;
                        nrc.LastName = pr.LastName;
                        nrc.PhoneNumber = pr.Mobile;
                        nrc.Notes = pr.Notes;
                        nrc.Street = pr.Street;
                        nrc.City = pr.City;
                        nrc.State = pr.State;
                        nrc.ZipCode = pr.ZipCode;
                        nrc.Email = pr.Email;
                        _context.Requestclients.Add(nrc);
                        _context.SaveChanges();

                        return RedirectToAction("first", "Home");
                    }
                    else
                    {
                        ModelState.AddModelError("wrong", "wrong password");
                        return View(pr);

                    }
                }*/



                var aspnetuser = new Aspnetuser();

                aspnetuser.Id = Guid.NewGuid().ToString();
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

                

                return RedirectToAction("first", "Home");

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
