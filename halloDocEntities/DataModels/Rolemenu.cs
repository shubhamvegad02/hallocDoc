using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace halloDocEntities.DataModels;

[Table("rolemenu")]
public partial class Rolemenu
{
    [Key]
    public int RoleMenuId { get; set; }

    public int RoleId { get; set; }

    public int MenuId { get; set; }

    [ForeignKey("MenuId")]
    [InverseProperty("Rolemenus")]
    public virtual Menu Menu { get; set; } = null!;

    [ForeignKey("RoleId")]
    [InverseProperty("Rolemenus")]
    public virtual Role Role { get; set; } = null!;
}
