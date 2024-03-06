using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace halloDocEntities.DataModels;

[Table("aspnetroles")]
public partial class Aspnetrole
{
    [Key]
    [StringLength(128)]
    public string AspNetRoleId { get; set; } = null!;

    [StringLength(256)]
    public string Name { get; set; } = null!;

    [ForeignKey("RoleId")]
    [InverseProperty("Roles")]
    public virtual ICollection<Aspnetuser> Users { get; set; } = new List<Aspnetuser>();
}
