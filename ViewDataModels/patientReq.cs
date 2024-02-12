﻿using System;
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

    }
}
