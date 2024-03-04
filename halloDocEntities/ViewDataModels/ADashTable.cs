using halloDocEntities.DataModels;
using System.ComponentModel.DataAnnotations;

namespace halloDocEntities.ViewDataModels
{
    public class ADashTable
    {
        public int rid { get; set; }
        public string guid { get; set; }
        public string name { get; set; }

        public string email { get; set; }

        public DateTime dob { get; set; }

        public string requstor { get; set; }

        public DateTime reqDate { get; set; }

        public string mobile { get; set; }

        public string address { get; set; }

        public string notes { get; set; }

        public string provider { get; set; }

        public string relation { get; set; }

        public string region { get; set; }

        public int regionId { get; set; }

        public int phyId { get; set; }

        public int status { get; set; }

        public int caseTag { get; set; }

        public string PhysicianName { get; set; }


    }
}
