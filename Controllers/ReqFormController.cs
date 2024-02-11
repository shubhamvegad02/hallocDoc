using hallocDoc.DataContext;
using hallocDoc.DataModels;
using hallocDoc.ViewDataModels;
using Microsoft.AspNetCore.Mvc;
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
        public IActionResult Concierge()
        {
            return View();
        }
        public IActionResult family()
        {
            return View();
        }
        
        public IActionResult Patient()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> patientReq(patientReq pr)
        {
            if (ModelState.IsValid && pr != null)
            {
                var aspnetuser = new Aspnetuser();

                aspnetuser.Id = Guid.NewGuid().ToString();
                aspnetuser.Email = pr.Email;
                aspnetuser.CreatedDate = DateTime.Now;
                aspnetuser.UserName = pr.Email;

                 _context.Aspnetusers.Add(aspnetuser);
                 _context.SaveChanges();


                var user = new User();


                /*user.UserId = Guid.NewGuid().ToString();*/
                
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

                _context.Users.Add(user);
                _context.SaveChanges();

                var request = new Request();

                request.RequestTypeId = 1;
                request.FirstName = pr.FirstName;   
                request.LastName = pr.LastName;
                request.CreatedDate = DateTime.Now;
                request.PhoneNumber = pr.Mobile;
                request.Email = pr.Email;
                request.CreatedDate = DateTime.Now;

                _context.Requests.Add(request);
                _context.SaveChanges();

                return RedirectToAction("first", "Home");

            }
            return RedirectToAction("Privacy", "Home");

        }

        /*[HttpPost]
        public IActionResult Patient(patientReq patient)
        {
            string FirstName = patient.FirstName;
            string? LastName = patient.LastName;
            DateTime CreatedDate = DateTime.Now;
            string Email = patient.Email;
            string? Mobile = patient.Mobile;
            string? Street = patient.Street;
            string? City = patient.City;
            string? State = patient.State;
            string? zipcode = patient.ZipCode;
            return View(patient);
        }*/
        public IActionResult SubmitReq()
        {

            const bool V = true;
            ViewBag.hidebackbtn = V;
            return View();
        }
    }
}
