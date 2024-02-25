using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace halloDocEntities.DataModels;

[Table("region")]
public partial class Region
{
    [Key]
    public int RegionId { get; set; }

    [StringLength(50)]
    public string Name { get; set; } = null!;

    [StringLength(50)]
    public string? Abbreviation { get; set; }

    [InverseProperty("Region")]
    public virtual ICollection<Business> Businesses { get; set; } = new List<Business>();

    [InverseProperty("Region")]
    public virtual ICollection<Requestclient> Requestclients { get; set; } = new List<Requestclient>();
}
