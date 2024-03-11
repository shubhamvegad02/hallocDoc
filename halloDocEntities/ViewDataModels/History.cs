using halloDocEntities.DataModels;
using Microsoft.AspNetCore.Http;

namespace halloDocEntities.ViewDataModels
{
    public class History
    {
        public int guid { get; set; }

        public DateTime date { get; set; }

        public int status { get; set; }

        public string name { get; set; }

        public string uploder { get; set; }

        public IFormFile myfile { get; set; }

        public string popupNotes { get; set; }



    }
}
