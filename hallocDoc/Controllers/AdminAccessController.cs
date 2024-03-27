using halloDocEntities.DataModels;
using halloDocEntities.ViewDataModels;
using halloDocLogic.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Elfie.Diagnostics;

namespace hallocDoc.Controllers
{
    public class AdminAccessController : Controller
    {
        private readonly IAdminAccess _adminaccess;
        public AdminAccessController(IAdminAccess adminAccess)
        {
            _adminaccess = adminAccess;
        }


        public IActionResult CreateAdminPost(EditPhysicianData edt)
        {
            bool check = _adminaccess.CreateAdminPost(edt);
            if (check)
            {
                TempData["SuccessMessage"] = "Role Updated Successfully..";
            }
            return RedirectToAction("UserAccess");
        }
        public IActionResult CreateAdmin()
        {
            EditPhysicianData edt = _adminaccess.CreateAdmin();
            return View(edt);
        }

        public IActionResult UserAccess()
        {
            return View();
        }

        public IActionResult EditRolePost(CreateRoleData crd)
        {
            bool check = _adminaccess.EditRolePost(crd);
            if (check)
            {
                TempData["SuccessMessage"] = "Role Updated Successfully..";
            }
            return RedirectToAction("AccountAccess");
        }

        public IActionResult CreateRolePost(CreateRoleData crd)
        {
            bool check = _adminaccess.CreateRolePost(crd);
            if (check)
            {
                TempData["SuccessMessage"] = "Role Added Successfully..";
            }
            return RedirectToAction("CreateRole");
        }

        public IActionResult CreateRole(int? roleId)
        {
            if (roleId != null)
            {
                CreateRoleData crd1 = _adminaccess.EditRole(roleId);
                ViewBag.RoleName = crd1.roleName;
                ViewBag.AccountType = crd1.accountType;
                ViewBag.SelectedMenuItem = crd1.selectedMenuItem;
                ViewBag.RoleId = roleId;
            }
            CreateRoleData crd = _adminaccess.CreateRole();
            if (roleId == null)
            {
                ViewBag.RoleName = "fresh";
                ViewBag.SelectedMenuItem = null;
                ViewBag.AccountType = null;
                ViewBag.RoleId = null;
            }
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
