using halloDocEntities.ViewDataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;

namespace halloDocLogic.Interfaces
{
    public interface IFormRequest
    {
        Task<bool> business (businessReq br);

        Task<bool> concierge (conciergeReq cr);

        Task<bool> family(string filename, familyReq fr);

        Task<string> Patient(string filename, patientReq pr);

        public string uploadfile(IFormFile myfile);

        public bool sendmailtoNew(string email, string link);

    }
}
