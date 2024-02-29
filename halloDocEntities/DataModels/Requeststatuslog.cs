using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace halloDocEntities.DataModels;

[Table("requeststatuslog")]
public partial class Requeststatuslog
{
    [Key]
    public int RequestStatusLogId { get; set; }

    public int RequestId { get; set; }

    public short Status { get; set; }

    public int? PhysicianId { get; set; }

    public int? AdminId { get; set; }

    public int? TransToPhysicianId { get; set; }

    [StringLength(500)]
    public string? Notes { get; set; }

    [Column(TypeName = "timestamp without time zone")]
    public DateTime? CreatedDate { get; set; }

    [Column("IP")]
    [StringLength(20)]
    public string? Ip { get; set; }

    [Column(TypeName = "bit(1)")]
    public BitArray? TransToAdmin { get; set; }

    [ForeignKey("AdminId")]
    [InverseProperty("Requeststatuslogs")]
    public virtual Admin? Admin { get; set; }

    [ForeignKey("PhysicianId")]
    [InverseProperty("RequeststatuslogPhysicians")]
    public virtual Physician? Physician { get; set; }

    [ForeignKey("RequestId")]
    [InverseProperty("Requeststatuslogs")]
    public virtual Request Request { get; set; } = null!;

    [ForeignKey("TransToPhysicianId")]
    [InverseProperty("RequeststatuslogTransToPhysicians")]
    public virtual Physician? TransToPhysician { get; set; }
}
