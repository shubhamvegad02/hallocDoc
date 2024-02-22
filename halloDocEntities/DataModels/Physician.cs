﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace halloDocEntities.DataModels;

[Table("physician")]
public partial class Physician
{
    [Key]
    public int PhysicianId { get; set; }

    [StringLength(128)]
    public string? AspNetUserId { get; set; }

    [StringLength(100)]
    public string FirstName { get; set; } = null!;

    [StringLength(100)]
    public string? LastName { get; set; }

    [StringLength(50)]
    public string Email { get; set; } = null!;

    [StringLength(20)]
    public string? Mobile { get; set; }

    [StringLength(500)]
    public string? MedicalLicense { get; set; }

    [StringLength(100)]
    public string? Photo { get; set; }

    [StringLength(500)]
    public string? AdminNotes { get; set; }

    [Column(TypeName = "bit(1)")]
    public BitArray? IsAgreementDoc { get; set; }

    [Column(TypeName = "bit(1)")]
    public BitArray? IsBackgroundDoc { get; set; }

    [Column(TypeName = "bit(1)")]
    public BitArray? IsTrainingDoc { get; set; }

    [Column(TypeName = "bit(1)")]
    public BitArray? IsNonDisclosureDoc { get; set; }

    [StringLength(500)]
    public string? Address1 { get; set; }

    [StringLength(500)]
    public string? Address2 { get; set; }

    [StringLength(100)]
    public string? City { get; set; }

    public int? RegionId { get; set; }

    [StringLength(10)]
    public string? Zip { get; set; }

    [StringLength(20)]
    public string? AltPhone { get; set; }

    [StringLength(128)]
    public string? CreatedBy { get; set; }

    [Column(TypeName = "timestamp without time zone")]
    public DateTime CreatedDate { get; set; }

    [StringLength(128)]
    public string? ModifiedBy { get; set; }

    [Column(TypeName = "timestamp without time zone")]
    public DateTime? ModifiedDate { get; set; }

    public short? Status { get; set; }

    [StringLength(100)]
    public string? BusinessName { get; set; }

    [StringLength(200)]
    public string? BusinessWebsite { get; set; }

    [Column(TypeName = "bit(1)")]
    public BitArray? IsDeleted { get; set; }

    public int? RoleId { get; set; }

    [Column("NPINumber")]
    [StringLength(500)]
    public string? Npinumber { get; set; }

    [Column(TypeName = "bit(1)")]
    public BitArray? IsLicenseDoc { get; set; }

    [StringLength(100)]
    public string? Signature { get; set; }

    [Column(TypeName = "bit(1)")]
    public BitArray? IsCredentialDoc { get; set; }

    [Column(TypeName = "bit(1)")]
    public BitArray? IsTokenGenerate { get; set; }

    [StringLength(50)]
    public string? SyncEmailAddress { get; set; }

    [ForeignKey("AspNetUserId")]
    [InverseProperty("PhysicianAspNetUsers")]
    public virtual Aspnetuser? AspNetUser { get; set; }

    [ForeignKey("CreatedBy")]
    [InverseProperty("PhysicianCreatedByNavigations")]
    public virtual Aspnetuser? CreatedByNavigation { get; set; }

    [ForeignKey("ModifiedBy")]
    [InverseProperty("PhysicianModifiedByNavigations")]
    public virtual Aspnetuser? ModifiedByNavigation { get; set; }

    [InverseProperty("Physician")]
    public virtual ICollection<Request> Requests { get; set; } = new List<Request>();

    [InverseProperty("Physician")]
    public virtual ICollection<Requestwisefile> Requestwisefiles { get; set; } = new List<Requestwisefile>();
}
