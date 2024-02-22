﻿using halloDocEntities.DataContext;
using halloDocEntities.DataModels;
using halloDocEntities.ViewDataModels;
using halloDocLogic.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace halloDocLogic.Repository
{
    public class FormRequest : IFormRequest
    {
        private readonly ApplicationDbContext _context;
        private readonly IHostEnvironment _hostEnvironment;

        public FormRequest(ApplicationDbContext context, IHostEnvironment environment)
        {
            _context = context;
            _hostEnvironment = environment;
        }

        public string uploadfile(IFormFile myfile)
        {
            string filename = null;

            if (myfile != null)
            {
                string folder = "wwwroot/uplodedItems/";
                var key = Guid.NewGuid().ToString();
                folder += key + "_" + myfile.FileName;
                filename = key + "_" + myfile.FileName;
                string serverFolder = Path.Combine(_hostEnvironment.ContentRootPath, folder);

                myfile.CopyToAsync(new FileStream(serverFolder, FileMode.Create));
                return filename;
            }
            return "";
        }

        public async Task<bool> business(businessReq br)
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
            return true;
        }

        public async Task<bool> concierge(conciergeReq cr)
        {
            var request = new Request();

            request.RequestTypeId = 1;
            request.FirstName = cr.CFN;
            request.LastName = cr.CLN;
            request.CreatedDate = DateTime.Now;
            request.PhoneNumber = cr.CMobile;
            request.Email = cr.CMail;
            request.CreatedDate = DateTime.Now;
            request.RelationName = "Concierge";

            await _context.Requests.AddAsync(request);
            await _context.SaveChangesAsync();

            var rc = new Requestclient();

            rc.RequestId = request.RequestId;
            rc.FirstName = cr.FirstName;
            rc.LastName = cr.LastName;
            rc.PhoneNumber = cr.Mobile;
            rc.Notes = cr.Notes;
            rc.Street = cr.Street;
            rc.City = cr.City;
            rc.State = cr.State;
            rc.ZipCode = cr.ZipCode;
            rc.Email = cr.Email;
            rc.Address = cr.Address;
            _context.Requestclients.Add(rc);
            _context.SaveChanges();

            var c = new Concierge();
            c.ConciergeName = cr.CFN;
            c.Address = cr.property;
            c.State = cr.State;
            c.City = cr.City;
            c.Street = cr.Street;
            c.ZipCode = cr.ZipCode;
            c.CreatedDate = cr.CreatedDate;
            await _context.Concierges.AddAsync(c);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> family(string filename, familyReq fr)
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

            await _context.Requests.AddAsync(request);
            await _context.SaveChangesAsync();

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

            var rf = new Requestwisefile();
            rf.RequestId = request.RequestId;
            rf.CreatedDate = DateTime.Now;
            rf.FileName = filename;
            _context.Requestwisefiles.Add(rf);
            _context.SaveChanges();
            return true;
        }

        public async Task<string> Patient(string filename, patientReq pr)
        {
            var dbasp = await _context?.Aspnetusers?.FirstOrDefaultAsync(m => m.Email == pr.Email);
            if (dbasp == null)
            {
                var aspnetuser = new Aspnetuser();
                aspnetuser.Id = Guid.NewGuid().ToString();
                /*aspnetuser.PasswordHash = password.encry(pr.password);*/
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
                request.Status = 2;

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

                var rf = new Requestwisefile();
                rf.RequestId = request.RequestId;
                rf.CreatedDate = DateTime.Now;
                rf.FileName = filename;
                _context.Requestwisefiles.Add(rf);
                _context.SaveChanges();

                var x = aspnetuser.Id.ToString();
                return x;
            }
            else
            {
                var aspid = dbasp?.Id;
                var dbuser = await _context.Users.FirstOrDefaultAsync(m => m.AspNetUserId == aspid);

                var request2 = new Request();
                request2.RequestTypeId = 1;
                request2.UserId = dbuser.UserId;
                request2.FirstName = pr.FirstName;
                request2.LastName = pr.LastName;
                request2.CreatedDate = DateTime.Now;
                request2.PhoneNumber = pr.Mobile;
                request2.Email = pr.Email;
                request2.Status = 2;

                _context.Requests.Add(request2);
                _context.SaveChanges();

                var rc2 = new Requestclient();

                rc2.RequestId = request2.RequestId;
                rc2.FirstName = request2.FirstName;
                rc2.LastName = request2.LastName;
                rc2.PhoneNumber = request2.PhoneNumber;
                rc2.Notes = pr.Notes;
                rc2.Street = pr.Street;
                rc2.City = pr.City;
                rc2.State = pr.State;
                rc2.ZipCode = pr.ZipCode;
                rc2.Email = pr.Email;
                _context.Requestclients.Add(rc2);
                _context.SaveChanges();

                var rf = new Requestwisefile();
                rf.RequestId = request2.RequestId;
                rf.CreatedDate = DateTime.Now;
                rf.FileName = filename;
                _context.Requestwisefiles.Add(rf);
                _context.SaveChanges();

                return "first";
            }
            
            
            return "";
        }
    }
}