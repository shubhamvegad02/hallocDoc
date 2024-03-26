using halloDocEntities.DataContext;
using halloDocEntities.DataModels;
using halloDocEntities.ViewDataModels;
using halloDocLogic.Interfaces;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace halloDocLogic.Repository
{
    public class AdminAccess : IAdminAccess
    {
        private readonly ApplicationDbContext _context;
        private readonly IHostEnvironment _hostEnvironment;
        private readonly IJwtService _jwtService;

        public AdminAccess(ApplicationDbContext context, IHostEnvironment environment, IJwtService jwtService)
        {
            _context = context;
            _hostEnvironment = environment;
            _jwtService = jwtService;
        }



        public bool CreateRolePost(CreateRoleData crd)
        {
            if (crd != null)
            {
                var role = new Role();
                role.Name = crd.roleName;
                role.AccountType = short.Parse(crd.accountType);
                role.CreatedBy = "admin";
                role.CreatedDate = DateTime.Now;
                _context.Roles.Update(role);
                _context.SaveChanges();

                var dbrole = _context.Roles.FirstOrDefault(m => m.Name == crd.roleName);
                if (dbrole != null)
                {
                    int roleId = dbrole.RoleId;
                    foreach (var item in crd?.menuItem)
                    {
                        var rolemenu = new Rolemenu();
                        rolemenu.RoleId = roleId;
                        /*rolemenu.Menu = item;*/
                    }
                }

                return true;
            }
            return false;
        }

        public CreateRoleData CreateRole()
        {
            CreateRoleData crd = new CreateRoleData();

            var dbmenu = _context.Menus;
            var dbaspnetrole = _context.Aspnetroles;

            crd.menuList = dbmenu.ToList();
            crd.accountTypeList = dbaspnetrole.ToList();

            return crd;
        }
        public bool DeleteRole(int roleId)
        {
            var dbrole = _context.Roles.FirstOrDefault(m => m.RoleId == roleId);
            if (dbrole != null)
            {
                dbrole.IsDeleted = true;
                _context.Roles.Update(dbrole);
                _context.SaveChanges();
            }
            return true;
        }
        public List<Role> AccountAccess()
        {
            List<Role> roleList = new List<Role>();
            var Dbrole = _context.Roles.Where(m => m.IsDeleted != true);
            foreach (var role in Dbrole)
            {
                roleList.Add(role);
            }
            return roleList;
        }

    }
}
