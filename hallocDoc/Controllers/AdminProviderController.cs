using halloDocLogic.Interfaces;
using halloDocLogic.Repository;
using Microsoft.AspNetCore.Mvc;

namespace hallocDoc.Controllers
{
    [CustomAuthorize("Admin")]
    public class AdminProviderController : Controller
    {
        private readonly IAdminProvider _adminProvider;
        public AdminProviderController(IAdminProvider adminProvider) { 
            _adminProvider = adminProvider;
        }



        public ActionResult Provider()
        {
            ViewBag.APData = _adminProvider.Provider();
            return View();
        }
    }
}
