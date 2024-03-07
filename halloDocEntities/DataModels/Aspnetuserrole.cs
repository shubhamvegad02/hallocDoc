using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace halloDocEntities.DataModels;

[Table("aspnetuserroles")]
public partial class Aspnetuserrole
{
    [Key]
    public int Id { get; set; }

    [StringLength(128)]
    public string UserId { get; set; } = null!;

    [StringLength(128)]
    public string RoleId { get; set; } = null!;

    [ForeignKey("RoleId")]
    [InverseProperty("Aspnetuserroles")]
    public virtual Aspnetrole Role { get; set; } = null!;

    [ForeignKey("UserId")]
    [InverseProperty("Aspnetuserroles")]
    public virtual Aspnetuser User { get; set; } = null!;
}
