using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace halloDocEntities.DataModels;

[Table("admin")]
public partial class Admin
{
    [Key]
    public int AdminId { get; set; }

    [StringLength(128)]
    public string AspNetUserId { get; set; } = null!;

    [StringLength(100)]
    public string FirstName { get; set; } = null!;

    [StringLength(100)]
    public string? LastName { get; set; }

    [StringLength(50)]
    public string Email { get; set; } = null!;

    [StringLength(20)]
    public string? Mobile { get; set; }

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
    public string CreatedBy { get; set; } = null!;

    [Column(TypeName = "timestamp without time zone")]
    public DateTime CreatedDate { get; set; }

    [StringLength(128)]
    public string? ModifiedBy { get; set; }

    [Column(TypeName = "timestamp without time zone")]
    public DateTime? ModifiedDate { get; set; }

    public short? Status { get; set; }

    public bool? IsDeleted { get; set; }

    public int? RoleId { get; set; }

    [ForeignKey("AspNetUserId")]
    [InverseProperty("AdminAspNetUsers")]
    public virtual Aspnetuser AspNetUser { get; set; } = null!;

    [ForeignKey("CreatedBy")]
    [InverseProperty("AdminCreatedByNavigations")]
    public virtual Aspnetuser CreatedByNavigation { get; set; } = null!;

    [ForeignKey("ModifiedBy")]
    [InverseProperty("AdminModifiedByNavigations")]
    public virtual Aspnetuser? ModifiedByNavigation { get; set; }

    [InverseProperty("Admin")]
    public virtual ICollection<Requestwisefile> Requestwisefiles { get; set; } = new List<Requestwisefile>();
}
