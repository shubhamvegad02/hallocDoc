using halloDocEntities.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace halloDocEntities.ViewDataModels
{
    public class CreateRoleData
    {
        public string? roleName { get; set; }

        public string? accountType { get; set; }

        public List<string>? menuItem { get; set; }

        public List<int>? selectedMenuItem { get; set; }

        public List<Aspnetrole>? accountTypeList { get; set; }

        public List<Menu>? menuList { get; set; }

        public int? roleId { get; set; }
    }
}
