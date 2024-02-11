using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using hallocDoc.DataModels;
using Microsoft.AspNetCore.Mvc;

namespace hallocDoc.ViewDataModels
{
    public class patientReq
    {
        [BindProperty]
        public string FirstName { get; set; } = "";


        [BindProperty]
        public string? LastName { get; set; }

        public DateTime CreatedDate { get; set; }

        [EmailAddress]
        public string Email { get; set; } = null!;
        [Phone]
        public string? Mobile { get; set; }

        public string? Street { get; set; }

        public string? City { get; set; }

        public string? State { get; set; }

        public string? ZipCode { get; set; }

    }
}
