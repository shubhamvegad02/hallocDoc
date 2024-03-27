using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace halloDocEntities.DataModels;

[Table("menu")]
public partial class Menu
{
    [Key]
    public int MenuId { get; set; }

    [StringLength(50)]
    public string Name { get; set; } = null!;

    public int? SortOrder { get; set; }

    [StringLength(50)]
    public string? AccountType { get; set; }

    [InverseProperty("Menu")]
    public virtual ICollection<Rolemenu> Rolemenus { get; set; } = new List<Rolemenu>();
}
