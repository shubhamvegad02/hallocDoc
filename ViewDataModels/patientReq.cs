using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using hallocDoc.DataModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace hallocDoc.ViewDataModels
{
    public class patientReq
    {

        /*[Required]
        *//*[StringLength(8)]*//*
        [DataType(DataType.Password)]
        public string password { get; set; }*/

        [StringLength(500)]
        [Required]
        public string? Notes { get; set; }

        [Required]
        public string FirstName { get; set; }


        
        public string? LastName { get; set; }

        public DateTime CreatedDate { get; set; }

        [EmailAddress]
        [Required]
        public string Email { get; set; } = null!;

        [Phone]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Not a valid phone number")]
        [StringLength(10)]
        [Required(ErrorMessage = "Mobile Number is Required")]
        public string? Mobile { get; set; }
/*
        [Column(TypeName = "character varying")]
        [Required]
        public string? PasswordHash { get; set; }*/

        public string? Street { get; set; }

        public string? City { get; set; }

        public string? State { get; set; }

        
        public string? ZipCode { get; set; }

        public IFormFile myfile { get; set; }

    }
}
