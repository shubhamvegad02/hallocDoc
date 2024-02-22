using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace halloDocEntities.DataModels;

[Table("requestbusiness")]
public partial class Requestbusiness
{
    [Key]
    public int RequestBusinessId { get; set; }

    public int RequestId { get; set; }

    public int BusinessId { get; set; }

    [Column("IP")]
    [StringLength(20)]
    public string? Ip { get; set; }

    [ForeignKey("BusinessId")]
    [InverseProperty("Requestbusinesses")]
    public virtual Business Business { get; set; } = null!;

    [ForeignKey("RequestId")]
    [InverseProperty("Requestbusinesses")]
    public virtual Request Request { get; set; } = null!;
}
