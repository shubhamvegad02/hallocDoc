using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace halloDocEntities.DataModels;

[Table("physicianregion")]
public partial class Physicianregion
{
    [Key]
    public int PhysicianRegionId { get; set; }

    public int PhysicianId { get; set; }

    public int RegionId { get; set; }

    [ForeignKey("PhysicianId")]
    [InverseProperty("Physicianregions")]
    public virtual Physician Physician { get; set; } = null!;

    [ForeignKey("RegionId")]
    [InverseProperty("Physicianregions")]
    public virtual Region Region { get; set; } = null!;
}
