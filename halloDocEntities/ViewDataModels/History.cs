using halloDocEntities.DataModels;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace halloDocEntities.ViewDataModels
{
    public class History
    {
        public int?  guid { get; set; }

        public DateTime date { get; set; }

        public int?  status { get; set; }

        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Name can only contain letters and spaces.")]
        public string? name { get; set; }

        public string? uploder { get; set; }

        public IFormFile myfile { get; set; }

        public string? popupNotes { get; set; }

        public int?  fileId { get; set; }



    }
}
