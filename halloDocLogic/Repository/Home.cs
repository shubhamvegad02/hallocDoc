using halloDocEntities.DataContext;
using halloDocLogic.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using halloDocEntities.ViewDataModels;
using halloDocEntities.DataModels;
using Microsoft.EntityFrameworkCore;
using System.Net.Http;
using System.Net.Mail;
using System.Net;
using System.Security.Policy;
using Microsoft.AspNetCore.Http;


namespace halloDocLogic.Repository
{
    public class Home : IHome
    {
        private readonly ApplicationDbContext _context;

        public Home(ApplicationDbContext context)
        {
            _context = context;
        }

        public patientLogin login(patientLogin pl)
        {
            patientLogin patientLogin = new patientLogin();
            var email = pl.Email;
            var dbasp = _context.Aspnetusers.FirstOrDefault(m => m.Email == email);
            var b = 2;
            if (dbasp == null)
            {
                patientLogin.Status = "notfound";
                return patientLogin;

            }
            else
            {
                if (decry(dbasp.PasswordHash) == pl.Password)
                {
                    var user =  _context.Aspnetusers.FirstOrDefault(m => m.Email == pl.Email);
                    patientLogin.Aspid = (user.Id);
                    patientLogin.Status = "success";
                    return patientLogin;
                }
                patientLogin.Status = "fail";
                return patientLogin;

            }


        }

        public string encry(string pass)
        {
            if (pass == null)
            {
                return null;
            }
            else
            {
                byte[] storePass = ASCIIEncoding.ASCII.GetBytes(pass);
                string encryptedPass = Convert.ToBase64String(storePass);
                return encryptedPass;
            }

        }

        public string decry(string pass)
        {
            if (pass == null)
            {
                return null;
            }
            else
            {
                byte[] encryptedPass = Convert.FromBase64String(pass);
                string decryptedPass = ASCIIEncoding.ASCII.GetString(encryptedPass);
                return decryptedPass;
            }
        }

        public bool sendmail(string email, string link)
        {
            var smtpClient = new SmtpClient("smtp.office365.com")
            {
                Port = 587,
                Credentials = new NetworkCredential("tatva.dotnet.shubhamvegad@outlook.com", "Vegad@12"),
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress("tatva.dotnet.shubhamvegad@outlook.com"),
                Subject = "Reset Your Password for halloDoc",

                Body = "<div> Hello " + email + "</div><p>We received a request to reset your password for your account on halloDoc. If you initiated this request, please click the link below to choose a new password:</p><p>" + link + "</p>",
                IsBodyHtml = true,
            };
            mailMessage.To.Add(email);

            smtpClient.Send(mailMessage);
            return true;
        }

        public async Task<bool> setPassWithToken(string email, string token, string password)
        {
            var cemail = email;
            var ctoken = token;
            var dbasp = await _context.Aspnetusers.FirstOrDefaultAsync(m => m.Email == cemail);
            if (dbasp != null)
            {

                if (String.Equals(dbasp.Token, ctoken.ToString()))
                {
                    dbasp.PasswordHash = encry(password);
                    _context.Aspnetusers.Update(dbasp);
                    _context.SaveChanges();
                    return true;
                }
            }
            return false;
        }


        public async Task<bool> setPass(string email, string password)
        {
            var cemail = email;
            var dbasp = await _context.Aspnetusers.FirstOrDefaultAsync(m => m.Email == cemail);
            if (dbasp == null)
            {
                return false;
            }
            if (dbasp != null)
            {
                dbasp.PasswordHash = encry(password);
                _context.Aspnetusers.Update(dbasp);
                _context.SaveChanges();
                return true;

            }
            return false;
        }

        public bool checkuser(string email)
        {
            var dbasp =  _context.Aspnetusers.FirstOrDefault(m => m.Email == email);
            if(dbasp != null)
            {
                return true;
            }
            return false;
        }

        public string generateToken(string email)
        {
            var dbasp = _context.Aspnetusers.FirstOrDefault(m => m.Email == email);
            var token = Guid.NewGuid().ToString();
            dbasp.Token = token;
            _context.Aspnetusers.Update(dbasp);
            _context.SaveChanges();
            return token;
        }
        public string getEmailFromId(string aspid)
        {
            
            var dbasp =  _context.Aspnetusers.FirstOrDefault(m => m.Id == aspid);
            if (dbasp != null)
            {
                return dbasp.Email;
            }

            return null;
        }




    }
}
