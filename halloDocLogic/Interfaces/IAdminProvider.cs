using halloDocEntities.ViewDataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace halloDocLogic.Interfaces
{
    public interface IAdminProvider
    {
        List<AProvider> Provider();

        Task sendSMS(string number, string msg);

        Task<EditPhysicianData> EditPhysician(int physicianId);

        bool EditProviderForm1(int physicianId, EditPhysicianData edt);

        bool EditProviderForm2(int physicianId, EditPhysicianData edt);

        bool EditProviderForm3(int physicianId, EditPhysicianData edt);

        bool EditProviderForm4(int physicianId, EditPhysicianData edt);

        bool DeleteProvider(int physicianId);

        EditPhysicianData CreteProvider();

        bool CreateProviderPost(EditPhysicianData edt);
    }
}
