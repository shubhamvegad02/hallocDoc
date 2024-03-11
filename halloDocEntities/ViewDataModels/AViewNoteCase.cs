using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace halloDocEntities.ViewDataModels
{
    public class AViewNoteCase
    {
        public string? relation { get; set; }
        public int? status { get; set; }

        public string? guid { get; set; }

        public string? symptoms { get; set; }

        public string? fname { get; set; }

        public string? lname { get; set; }

        public DateTime dob { get; set; }

        public string? mobile { get; set; }

        public string? email { get; set; }

        public string? region { get; set; }

        public string? address { get; set; }

        public string? room { get; set; }

        public string? Tnotes { get; set; }

        public string? Pnotes { get; set; }

        public string? Anotes { get; set; }

        public string? TempAnotes { get; set; }

        public int rid { get; set; }

        public string confNumber { get; set; }
    }
}
