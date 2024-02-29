using halloDocEntities.ViewDataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace halloDocLogic.Interfaces
{
    public interface IADashboard
    {
        Task<List<ADashTable>> ADashTableData(int n);

        public ARequestCount ReqCount();

        public AViewNoteCase VCData(int rid);

        public AViewNoteCase VNData(int rid);

        Task<int> CancelCase(int rid, ADashTable dt);

        Task AssignCase(int rid, ADashTable dt);
    }
}
