using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace AutoComWeb.Models;

[PrimaryKey("WorkOrderId", "SparePartsId")]
[Table("work_order_parts")]
public partial class WorkOrderPart
{
    [Key]
    [Column("work_order_id")]
    public int WorkOrderId { get; set; }

    [Key]
    [Column("spare_parts_id")]
    public int SparePartsId { get; set; }

    [Column("quantity")]
    public int Quantity { get; set; }

    [Column("fixed_price")]
    [Precision(18, 2)]
    public decimal FixedPrice { get; set; }

    [ForeignKey("SparePartsId")]
    [InverseProperty("WorkOrderParts")]
    public virtual SparePart SpareParts { get; set; } = null!;

    [ForeignKey("WorkOrderId")]
    [InverseProperty("WorkOrderParts")]
    public virtual WorkOrder WorkOrder { get; set; } = null!;
}
