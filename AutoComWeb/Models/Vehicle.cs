using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace AutoComWeb.Models;

[Table("vehicles")]
public partial class Vehicle
{
    [Key]
    [Column("vehicle_id")]
    public int VehicleId { get; set; }

    [Column("client_id")]
    public int? ClientId { get; set; }

    [Column("brand")]
    [StringLength(50)]
    public string Brand { get; set; } = null!;

    [Column("model")]
    [StringLength(50)]
    public string Model { get; set; } = null!;

    [Column("year")]
    public int? Year { get; set; }

    [Column("vin")]
    [StringLength(17)]
    public string? Vin { get; set; }

    [Column("number_plate")]
    [StringLength(10)]
    public string? NumberPlate { get; set; }

    [ForeignKey("ClientId")]
    [InverseProperty("Vehicles")]
    public virtual Client? Client { get; set; }

    [InverseProperty("Vehicle")]
    public virtual ICollection<WorkOrder> WorkOrders { get; set; } = new List<WorkOrder>();
}
