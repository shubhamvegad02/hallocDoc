using halloDocEntities.DataModels;
using halloDocEntities.ViewDataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace halloDocLogic.Interfaces
{
    public interface IAdminAccess
    {
        List<Role> AccountAccess();

        bool DeleteRole(int roleId);

        public CreateRoleData CreateRole();

        bool CreateRolePost(CreateRoleData crd);

        CreateRoleData EditRole(int? roleId);

        bool EditRolePost(CreateRoleData crd);

        EditPhysicianData CreateAdmin();

        bool CreateAdminPost(EditPhysicianData edt);

        List<UserAccessData> UserAccess();
    }
}
