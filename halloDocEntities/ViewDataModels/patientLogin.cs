﻿using halloDocEntities.DataModels;
using System.ComponentModel.DataAnnotations;

namespace halloDocEntities.ViewDataModels
{
    public class patientLogin
    {
        [EmailAddress]
        [Required]
        public string Email { get; set; } = null!;

        [Required]
        public string Password { get; set; } = null!;

        public string? Aspid { get; set; }

        public string? Status { get; set; }
    }
}