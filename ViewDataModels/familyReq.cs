using System.ComponentModel.DataAnnotations;

namespace hallocDoc.ViewDataModels
{
    public class familyReq
    {
        [StringLength(100)]
        public string? FFirstName { get; set; }

        [StringLength(100)]
        public string? FLastName { get; set; }

        [StringLength(23)]
        public string? FPhoneNumber { get; set; }

        [StringLength(50)]
        public string? FEmail { get; set; }

        [StringLength(100)]
        public string? RelationName { get; set; }

        [StringLength(500)]
        [Required]
        public string? Notes { get; set; }

        [Required]
        public string FirstName { get; set; } = "";


        [Required]
        public string? LastName { get; set; }

        public DateTime CreatedDate { get; set; }

        [EmailAddress]
        [Required]
        public string Email { get; set; } = null!;

        [Phone]
        [Required]
        public string? Mobile { get; set; }
        /*
                [Column(TypeName = "character varying")]
                [Required]
                public string? PasswordHash { get; set; }*/

        public string? Street { get; set; }

        public string? City { get; set; }

        public string? State { get; set; }

        public string? ZipCode { get; set; }

        [StringLength(500)]
        public string? Address { get; set; }
    }
}
