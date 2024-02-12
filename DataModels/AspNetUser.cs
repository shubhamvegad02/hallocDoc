﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace hallocDoc.DataModels;

[Table("aspnetusers")]
public partial class Aspnetuser
{
    [Key]
    [StringLength(128)]
    public string Id { get; set; } = null!;

    [StringLength(256)]
    public string UserName { get; set; } = null!;

    [Column(TypeName = "character varying")]
    public string? PasswordHash { get; set; }

    [StringLength(256)]
    public string? Email { get; set; }

    [Column(TypeName = "character varying")]
    public string? PhoneNumber { get; set; }

    [Column("IP")]
    [StringLength(20)]
    public string? Ip { get; set; }

    [Column(TypeName = "timestamp without time zone")]
    public DateTime? CreatedDate { get; set; }

    [Column(TypeName = "timestamp without time zone")]
    public DateTime? EndDate { get; set; }

    [InverseProperty("CreatedByNavigation")]
    public virtual ICollection<Business> BusinessCreatedByNavigations { get; set; } = new List<Business>();

    [InverseProperty("ModifiedByNavigation")]
    public virtual ICollection<Business> BusinessModifiedByNavigations { get; set; } = new List<Business>();

    [InverseProperty("AspNetUser")]
    public virtual ICollection<Physician> PhysicianAspNetUsers { get; set; } = new List<Physician>();

    [InverseProperty("CreatedByNavigation")]
    public virtual ICollection<Physician> PhysicianCreatedByNavigations { get; set; } = new List<Physician>();

    [InverseProperty("ModifiedByNavigation")]
    public virtual ICollection<Physician> PhysicianModifiedByNavigations { get; set; } = new List<Physician>();

    [InverseProperty("AspNetUser")]
    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
