using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace halloDocEntities.ViewDataModels
{
    public class EncounterData
    {

        public int rid { get; set; }


        [Required]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Name can only contain letters and spaces.")]
        public string FirstName { get; set; } = null!;

        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Name can only contain letters and spaces.")]
        public string? LastName { get; set; }

        [EmailAddress(ErrorMessage = "Not a valid Email Address")]
        [Required(ErrorMessage = "Email is Required")]
        [StringLength(50)]
        public string Email { get; set; } = null!;

        [Phone]
        [RegularExpression(@"^[6789]\d{9}$", ErrorMessage = "Not a valid phone number")]
        [StringLength(10)]
        [Required(ErrorMessage = "Mobile Number is Required")]
        public string Mobile { get; set; } = null!;
        public string? Address { get; set; }
        public DateTime? CreatedDate { get; set; }


        public DateTime? Dob { get; set; }


        public string? IllnessHistory { get; set; }


        public string? MedicalHistory { get; set; }


        public string? Medication { get; set; }


        public string? Allergies { get; set; }


        public string? Temp { get; set; }


        public string? Hr { get; set; }


        public string? Rr { get; set; }


        public string? O2 { get; set; }


        public string? Bp { get; set; }


        public string? Pain { get; set; }



        public string? Heent { get; set; }


        public string? Cv { get; set; }



        public string? Chest { get; set; }


        public string? Abd { get; set; }


        public string? Extr { get; set; }


        public string? Skin { get; set; }



        public string? Neuro { get; set; }



        public string? Other { get; set; }

        public string? Diagnosis { get; set; }

        public string? TreatmentPlan { get; set; }
        public string? MedicationDespensed { get; set; }

        public string? Procedure { get; set; }

        public string? Followup { get; set; }

        public short? Status { get; set; }

        public bool? IsFinalize { get; set; }
    }
}
