using halloDocEntities.DataModels;
using halloDocEntities.ViewDataModels;
using halloDocLogic.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace hallocDoc.Controllers
{
    public class AdminAccessController : Controller
    {
        private readonly IAdminAccess _adminaccess;
        public AdminAccessController(IAdminAccess adminAccess)
        {
            _adminaccess = adminAccess;
        }



        public IActionResult CreateRolePost(CreateRoleData crd)
        {
            bool check = _adminaccess.CreateRolePost(crd);
            return RedirectToAction("AccountAccess");
        }

        public IActionResult CreateRole()
        {
            CreateRoleData crd = _adminaccess.CreateRole();
            return View(crd);
        }
        public IActionResult DeleteRole(int roleId)
        {
            bool check = _adminaccess.DeleteRole(roleId);
            if (check)
            {
                TempData["SuccessMessage"] = "Role Deleted Successfully..";
            }
            return RedirectToAction("AccountAccess");
        }
        public IActionResult AccountAccess()
        {
            List<Role> roleList = _adminaccess.AccountAccess();
            ViewData["roleList"] = roleList;
            return View(roleList);
        }
    }
}
