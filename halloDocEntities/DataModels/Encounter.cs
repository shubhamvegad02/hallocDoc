using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace halloDocEntities.DataModels;

[Table("encounter")]
public partial class Encounter
{
    [Key]
    public int EncounterId { get; set; }

    public int RequestId { get; set; }

    [StringLength(100)]
    public string FirstName { get; set; } = null!;

    [StringLength(100)]
    public string? LastName { get; set; }

    [StringLength(50)]
    public string Email { get; set; } = null!;

    [StringLength(20)]
    public string Mobile { get; set; } = null!;

    [StringLength(500)]
    public string? Address { get; set; }

    [Column(TypeName = "timestamp without time zone")]
    public DateTime? CreatedDate { get; set; }

    [Column("DOB", TypeName = "timestamp without time zone")]
    public DateTime? Dob { get; set; }

    [Column("illnessHistory")]
    [StringLength(500)]
    public string? IllnessHistory { get; set; }

    [Column("medicalHistory")]
    [StringLength(500)]
    public string? MedicalHistory { get; set; }

    [Column("medication")]
    [StringLength(500)]
    public string? Medication { get; set; }

    [Column("allergies")]
    [StringLength(500)]
    public string? Allergies { get; set; }

    [Column("temp")]
    [StringLength(50)]
    public string? Temp { get; set; }

    [Column("hr")]
    [StringLength(50)]
    public string? Hr { get; set; }

    [Column("rr")]
    [StringLength(50)]
    public string? Rr { get; set; }

    [Column("o2")]
    [StringLength(50)]
    public string? O2 { get; set; }

    [Column("bp")]
    [StringLength(50)]
    public string? Bp { get; set; }

    [Column("pain")]
    [StringLength(50)]
    public string? Pain { get; set; }

    [Column("heent")]
    [StringLength(100)]
    public string? Heent { get; set; }

    [Column("cv")]
    [StringLength(100)]
    public string? Cv { get; set; }

    [Column("chest")]
    [StringLength(100)]
    public string? Chest { get; set; }

    [Column("abd")]
    [StringLength(100)]
    public string? Abd { get; set; }

    [Column("extr")]
    [StringLength(100)]
    public string? Extr { get; set; }

    [Column("skin")]
    [StringLength(100)]
    public string? Skin { get; set; }

    [Column("neuro")]
    [StringLength(100)]
    public string? Neuro { get; set; }

    [Column("other")]
    [StringLength(100)]
    public string? Other { get; set; }

    [Column("diagnosis")]
    [StringLength(100)]
    public string? Diagnosis { get; set; }

    [Column("treatmentPlan")]
    [StringLength(100)]
    public string? TreatmentPlan { get; set; }

    [StringLength(100)]
    public string? MedicationDespensed { get; set; }

    [Column("procedure")]
    [StringLength(100)]
    public string? Procedure { get; set; }

    [Column("followup")]
    [StringLength(100)]
    public string? Followup { get; set; }

    public short? Status { get; set; }

    public bool? IsFinalize { get; set; }

    [ForeignKey("RequestId")]
    [InverseProperty("Encounters")]
    public virtual Request Request { get; set; } = null!;
}
