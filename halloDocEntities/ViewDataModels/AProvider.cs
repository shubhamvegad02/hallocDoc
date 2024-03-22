using halloDocEntities.DataModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace halloDocEntities.ViewDataModels
{
    public class AProvider
    {
        public List<string>? regionList { get; set; }


        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Name can only contain letters and spaces.")]
        public string? name { get; set; }

        public string? role { get; set; }

        public string? onCall { get; set; }

        public string? status { get; set; }

        public string? communicationType { get; set; }

        public string? popupMsg { get; set; }

        public string? region { get; set; }

        public int physicianId { get; set; }

        [EmailAddress]
        [RegularExpression(@"^([^\s@]+@[^\s@]+\.[^\s@]{1,3})$", ErrorMessage = "Domain is not real..")]
        public string? email { get; set; }
    }
}
