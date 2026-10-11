using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace AutoComWeb.Models;

[Table("services")]
public partial class Service
{
    [Key]
    [Column("services_id")]
    public int ServicesId { get; set; }

    [Column("name")]
    [StringLength(100)]
    public string Name { get; set; } = null!;

    [Column("price")]
    [Precision(18, 2)]
    public decimal Price { get; set; }

    [Column("standart_hours")]
    [Precision(5, 2)]
    public decimal StandartHours { get; set; }

    [InverseProperty("Services")]
    public virtual ICollection<WorkOrderService> WorkOrderServices { get; set; } = new List<WorkOrderService>();
}
