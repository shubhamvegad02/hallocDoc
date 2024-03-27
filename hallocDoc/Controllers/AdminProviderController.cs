using halloDocEntities.ViewDataModels;
using halloDocLogic.Interfaces;
using halloDocLogic.Repository;
using Microsoft.AspNetCore.Mvc;

namespace hallocDoc.Controllers
{
    [CustomAuthorize("Admin")]
    public class AdminProviderController : Controller
    {
        private readonly IAdminProvider _adminProvider;
        private readonly IADashboard _aDashboard;
        public AdminProviderController(IAdminProvider adminProvider, IADashboard aDashboard)
        {
            _adminProvider = adminProvider;
            _aDashboard = aDashboard;
        }


        public IActionResult CreateProviderPost(EditPhysicianData edt)
        {
            bool check = _adminProvider.CreateProviderPost(edt);
            if (check)
            {
                TempData["SuccessMessage"] = "Provider Created Successfully..";
            }
            else
            {
                TempData["ErrorMessage"] = "Data is not updated";
            }
            return RedirectToAction("Provider");
        }
        public IActionResult CreteProvider()
        {
            EditPhysicianData edt = _adminProvider.CreteProvider();
            return View(edt);
        }

        public IActionResult DeleteProvider(int PhysicianId, EditPhysicianData epd)
        {
            bool check = _adminProvider.DeleteProvider(PhysicianId);
            if (check)
            {
                TempData["SuccessMessage"] = "Data Updated Successfully..";
            }
            else
            {
                TempData["ErrorMessage"] = "Data is not updated";
            }
            return RedirectToAction("Provider", "AdminProvider");
        }

        public IActionResult EditProviderForm4(int PhysicianId, EditPhysicianData epd)
        {
            bool check = _adminProvider.EditProviderForm4(PhysicianId, epd);
            if (check)
            {
                TempData["SuccessMessage"] = "Data Updated Successfully..";
            }
            else
            {
                TempData["ErrorMessage"] = "Data is not updated";
            }
            return RedirectToAction("EditProvider", "AdminProvider", new { physicianId = PhysicianId });
        }

        public IActionResult EditProviderForm3(int PhysicianId, EditPhysicianData epd)
        {
            bool check = _adminProvider.EditProviderForm3(PhysicianId, epd);
            if (check)
            {
                TempData["SuccessMessage"] = "Data Updated Successfully..";
            }
            else
            {
                TempData["ErrorMessage"] = "Data is not updated";
            }
            return RedirectToAction("EditProvider", "AdminProvider", new { physicianId = PhysicianId });
        }

        public IActionResult EditProviderForm2(int PhysicianId, EditPhysicianData epd)
        {
            bool check = _adminProvider.EditProviderForm2(PhysicianId, epd);
            if (check)
            {
                TempData["SuccessMessage"] = "Data Updated Successfully..";
            }
            else
            {
                TempData["ErrorMessage"] = "Data is not updated";
            }
            return RedirectToAction("EditProvider", "AdminProvider", new { physicianId = PhysicianId });
        }

        public IActionResult EditProviderForm1(int PhysicianId, EditPhysicianData epd)
        {
            bool check = _adminProvider.EditProviderForm1(PhysicianId, epd);
            if (check)
            {
                TempData["SuccessMessage"] = "Data Updated Successfully..";
            }
            else
            {
                TempData["ErrorMessage"] = "Data is not updated";
            }
            return RedirectToAction("EditProvider", "AdminProvider", new {physicianId = PhysicianId});
        }

        public async Task<IActionResult> EditProvider(int physicianId)
        {
            EditPhysicianData epd = await _adminProvider.EditPhysician(physicianId);
            return View(epd);
        }

        public IActionResult MailToProvider(AProvider ap, string flexRadioDefault)
        {
            string message = ap?.popupMsg ?? "";
            string number = "8155973839";
            if (flexRadioDefault == "Email" || flexRadioDefault == "Both")
            {
                string email = ap?.email;
                string subject = "Message From HalloDoc Admin";
                string[] s = new string[0];
                var check = _aDashboard.sendMail(email, subject, message, s);
                if (check)
                {
                    TempData["SuccessMessage"] = "Message sent Successfully..";
                }
                else
                {
                    TempData["ErrorMessage"] = "Message not sent..";
                }
            }
            if (flexRadioDefault == "SMS" || flexRadioDefault == "Both")
            {
                _adminProvider.sendSMS(number, message);
            }
            return RedirectToAction("Provider", "AdminProvider");
        }

        public ActionResult Provider()
        {
            ViewBag.APData = _adminProvider.Provider();
            return View();
        }

    }
}
