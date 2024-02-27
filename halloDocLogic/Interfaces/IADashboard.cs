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
    }
}
