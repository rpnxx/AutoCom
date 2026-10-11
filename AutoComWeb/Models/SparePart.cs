using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace AutoComWeb.Models;

[Table("spare_parts")]
public partial class SparePart
{
    [Key]
    [Column("spare_parts_id")]
    public int SparePartsId { get; set; }

    [Column("name")]
    [StringLength(100)]
    public string Name { get; set; } = null!;

    [Column("article")]
    [StringLength(20)]
    public string Article { get; set; } = null!;

    [Column("price")]
    [Precision(18, 2)]
    public decimal Price { get; set; }

    [Column("quantity")]
    public int Quantity { get; set; }

    [InverseProperty("SpareParts")]
    public virtual ICollection<WorkOrderPart> WorkOrderParts { get; set; } = new List<WorkOrderPart>();
}
