using Microsoft.AspNetCore.Mvc;

namespace hallocDoc.Controllers
{
    public class pDashboardController : Controller
    {
        public IActionResult History()
        {
            return View();
        }
        public IActionResult viewDoc()
        {
            return View();
        }
    }
}
