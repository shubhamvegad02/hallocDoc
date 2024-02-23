using halloDocEntities.ViewDataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace halloDocLogic.Interfaces
{
    public interface IPDashboard
    {
        Task<bool> uploadtoid(History h, int reqid);

        public List<History> HistoryData(string aspid);

        public string UserNameFromId(string aspid);

        public List<History> ViewDocData(string aspid, int reqid);

        public profile ProfileData(string aspid, profile pr);

        public bool ProfileSubmit(string aspid, profile pr);

        public Stream downloadAll(string aspid);
    }
}
