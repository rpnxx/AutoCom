using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace AutoComWeb.Models;

[PrimaryKey("WorkOrderId", "ServicesId")]
[Table("work_order_services")]
public partial class WorkOrderService
{
    [Key]
    [Column("work_order_id")]
    public int WorkOrderId { get; set; }

    [Key]
    [Column("services_id")]
    public int ServicesId { get; set; }

    [Column("fixed_price")]
    [Precision(18, 2)]
    public decimal FixedPrice { get; set; }

    [ForeignKey("ServicesId")]
    [InverseProperty("WorkOrderServices")]
    public virtual Service Services { get; set; } = null!;

    [ForeignKey("WorkOrderId")]
    [InverseProperty("WorkOrderServices")]
    public virtual WorkOrder WorkOrder { get; set; } = null!;
}
