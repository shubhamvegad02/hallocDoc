using halloDocEntities.DataContext;
using halloDocEntities.DataModels;
using halloDocEntities.ViewDataModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

/*using Nest;*/
using Npgsql.EntityFrameworkCore.PostgreSQL.Infrastructure.Internal;
using System.IO;
using halloDocLogic.Interfaces;


namespace hallocDoc.Controllers
{
    public class ReqFormController : Controller
    {
        public Boolean V = false;

        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IFormRequest _req;


        public ReqFormController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment, IFormRequest req)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
            _req = req;
        }


        public IActionResult Business()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Business(businessReq br)
        {
            if (ModelState.IsValid/* && br != null*/)
            {
                var result = _req.business(br);
                if (await result)
                {
                    return RedirectToAction("first", "Home");
                }
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
            if (ModelState.IsValid)
            {
                var result = await _req.concierge(cr);

                if (result)
                {
                    return RedirectToAction("first", "Home");
                }

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
            if (ModelState.IsValid)
            {
                
                var filename = _req.uploadfile(fr.myfile);
                var result = await _req.family(filename, fr);

                if (result)
                {
                    return RedirectToAction("first", "Home");
                }
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
            
            if (ModelState.IsValid)
            {
                var filename = _req.uploadfile(pr.myfile);

                var result = _req.Patient(filename, pr);
                if (await result == "first")
                {
                    return RedirectToAction("first", "Home");
                }
                else if (await result == "")
                {
                    return View(pr);
                }
                else
                {
                    var x = result.Result;
                    return RedirectToAction("createPatient", "Home", new { aspid = x });
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
