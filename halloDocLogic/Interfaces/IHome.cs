using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using halloDocEntities.ViewDataModels;

namespace halloDocLogic.Interfaces
{
    public interface IHome
    {
        patientLogin login(patientLogin pl);

        public string encry(string pass);

        public string decry(string pass);

        public bool sendmail(string email, string link);

        Task<bool> setPassWithToken(string email, string token, string password);

        Task<bool> setPass(string email, string password);

        public bool checkuser(string email);

        public string generateToken(string email);

        public string getEmailFromId(string aspid);
    }
}
