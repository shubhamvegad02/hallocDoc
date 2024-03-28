﻿using halloDocEntities.DataContext;
using halloDocEntities.DataModels;
using halloDocEntities.ViewDataModels;
using halloDocLogic.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Nest;
using System;
using System.Collections.Generic;
using System.Data;
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


        public List<UserAccessData> UserAccess()
        {
            List<UserAccessData> list = new List<UserAccessData>();

            var dbdataadmin = from admin in _context.Admins
                              join asp in _context.Aspnetuserroles on admin.AspNetUserId equals asp.UserId into roles
                              from asp in roles.DefaultIfEmpty()
                              select new
                              {
                                  admin,
                                  asp
                              };

            int adminopenReq = _context.Requests.Where(m => m.Status == 1).Count();

            foreach (var item in dbdataadmin)
            {
                UserAccessData uad = new UserAccessData();
                uad.accountType = int.Parse(item.asp.RoleId);
                uad.aspId = item.asp.UserId;
                uad.name = string.Concat(item.admin.FirstName + " " + item.admin.LastName);
                uad.mobile = item.admin.Mobile;
                uad.status = item.admin.Status;
                uad.openReq = adminopenReq;
                list.Add(uad);
            }

            var dbdataphysician = from phy in _context.Physicians
                                  join asp in _context.Aspnetuserroles on phy.AspNetUserId equals asp.UserId into roles
                                  from asp in roles.DefaultIfEmpty()
                                  select new
                                  {
                                      phy,
                                      asp,
                                      openReqCount = (from request in _context.Requests
                                                      where request.PhysicianId == phy.PhysicianId
                                                      select request).Count()
                                  };

            foreach (var item in dbdataphysician)
            {
                UserAccessData uad = new UserAccessData();
                uad.accountType = int.Parse(item.asp.RoleId);
                uad.aspId = item.asp.UserId;
                uad.name = string.Concat(item.phy.FirstName + " " + item.phy.LastName);
                uad.mobile = item.phy.Mobile;
                uad.status = item.phy.Status.ToString();
                uad.openReq = item.openReqCount;
                list.Add(uad);
            }

            return list;
        }

        public bool CreateAdminPost(EditPhysicianData edt)
        {
            if (edt != null)
            {
                Aspnetuser asp = new Aspnetuser();
                asp.Id = Guid.NewGuid().ToString();
                asp.UserName = edt.email;
                asp.Email = edt.email;
                asp.PasswordHash = _jwtService.encry(edt.password);
                asp.PhoneNumber = edt.mobile;
                asp.CreatedDate = DateTime.Now;
                _context.Aspnetusers.Add(asp);
                _context.SaveChanges();

                var dbasp = _context.Aspnetusers.FirstOrDefault(m => m.Email == asp.Email);
                string aspid = "";
                if (dbasp != null)
                {
                    aspid = dbasp.Id;
                    Admin admin = new Admin();
                    admin.AspNetUserId = aspid;
                    admin.FirstName = edt.firstname;
                    admin.LastName = edt.lastname;
                    admin.Mobile = edt.mobile;
                    admin.Email = edt.email;
                    admin.Address1 = edt.address1;
                    admin.Address2 = edt.address2;
                    admin.City = edt.city;
                    admin.RegionId = int.Parse(edt?.state);
                    admin.Zip = edt.zipcode;
                    admin.AltPhone = edt.billingMobile;
                    admin.CreatedBy = aspid;
                    admin.CreatedDate = DateTime.Now;
                    admin.Status = "active";
                    _context.Admins.Add(admin);
                    _context.SaveChanges();
                }

                Aspnetuserrole aspnetuserrole = new Aspnetuserrole();
                aspnetuserrole.UserId = aspid;
                aspnetuserrole.RoleId = "1";
                _context.Aspnetuserroles.Add(aspnetuserrole);
                _context.SaveChanges();

                var dbadmin = _context.Admins.FirstOrDefault(m => m.AspNetUserId == aspid);
                if (dbadmin != null)
                {
                    int adminId = dbadmin.AdminId;
                    foreach (var row in edt.ProviderStateList)
                    {
                        AdminRegion ar = new AdminRegion();
                        ar.AdminId = adminId;
                        ar.RegionId = int.Parse(row);
                        _context.AdminRegions.Add(ar);
                    }
                    _context.SaveChanges();
                }


                return true;
            }
            return false;
        }

        public EditPhysicianData CreateAdmin()
        {
            EditPhysicianData epd = new EditPhysicianData();
            var dbrole = _context.Roles.Where(m => m.AccountType == 1);
            List<Role> roleList = new List<Role>();
            foreach (var i in dbrole)
            {
                roleList.Add(i);
            }

            var dbregion = _context.Regions;
            List<Region> regions = new List<Region>();
            foreach (var i in dbregion)
            {
                regions.Add(i);
            }
            epd.roleList = roleList;
            epd.stateList = regions;
            return epd;
        }

        public bool EditRolePost(CreateRoleData crd)
        {
            if (crd != null)
            {
                int roleId = 0;
                var dbrole = _context.Roles.FirstOrDefault(m => m.RoleId == crd.roleId);
                if (dbrole != null)
                {
                    roleId = dbrole.RoleId;
                    dbrole.Name = crd?.roleName;
                    dbrole.AccountType = short.Parse(crd?.accountType);
                    _context.Roles.Update(dbrole);
                    _context.SaveChanges();
                }
                var dbrolemenu = _context.Rolemenus.Where(m => m.RoleId == roleId);
                if (dbrolemenu != null)
                {
                    _context.Rolemenus.RemoveRange(dbrolemenu);
                }
                foreach (var item in crd?.menuItem)
                {
                    var rolemenu = new Rolemenu();
                    rolemenu.RoleId = roleId;
                    rolemenu.MenuId = int.Parse(item);
                    _context.Rolemenus.Add(rolemenu);
                }
                _context.SaveChanges();
                return true;
            }
            return false;
        }

        public CreateRoleData EditRole(int? roleId)
        {
            CreateRoleData crd = new CreateRoleData();
            var dbrole = _context.Roles.FirstOrDefault(m => m.RoleId == roleId);
            if (dbrole != null)
            {
                crd.roleName = dbrole.Name;
                crd.accountType = dbrole.AccountType.ToString();
            }
            var dbrolemenu = _context.Rolemenus.Where(m => m.RoleId == roleId);
            if (dbrolemenu != null)
            {
                List<int> ints = new List<int>();
                foreach (var item in dbrolemenu)
                {
                    ints.Add(item.MenuId);
                }
                crd.selectedMenuItem = ints;
            }
            return crd;
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
                        rolemenu.MenuId = int.Parse(item);
                        _context.Rolemenus.Add(rolemenu);
                    }
                    _context.SaveChanges();
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