using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace AutoComWeb.Models;

[Table("work_order")]
public partial class WorkOrder
{
    [Key]
    [Column("work_order_id")]
    public int WorkOrderId { get; set; }

    [Column("client_id")]
    public int? ClientId { get; set; }

    [Column("vehicle_id")]
    public int? VehicleId { get; set; }

    [Column("mechanic_id")]
    public int? MechanicId { get; set; }

    [Column("date_created", TypeName = "timestamp without time zone")]
    public DateTime DateCreated { get; set; }

    [Column("appointment_date", TypeName = "timestamp without time zone")]
    public DateTime AppointmentDate { get; set; }

    [Column("status")]
    [StringLength(30)]
    public string Status { get; set; } = null!;

    [Column("payment")]
    [StringLength(10)]
    public string Payment { get; set; } = null!;

    [Column("odometr")]
    public int Odometr { get; set; }

    [Column("total_amount")]
    [Precision(18, 2)]
    public decimal TotalAmount { get; set; }

    [Column("notes")]
    public string? Notes { get; set; }

    [ForeignKey("ClientId")]
    [InverseProperty("WorkOrders")]
    public virtual Client? Client { get; set; }

    [ForeignKey("MechanicId")]
    [InverseProperty("WorkOrders")]
    public virtual Mechanic? Mechanic { get; set; }

    [ForeignKey("VehicleId")]
    [InverseProperty("WorkOrders")]
    public virtual Vehicle? Vehicle { get; set; }

    [InverseProperty("WorkOrder")]
    public virtual ICollection<WorkOrderPart> WorkOrderParts { get; set; } = new List<WorkOrderPart>();

    [InverseProperty("WorkOrder")]
    public virtual ICollection<WorkOrderService> WorkOrderServices { get; set; } = new List<WorkOrderService>();
}
