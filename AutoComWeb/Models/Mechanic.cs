using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace AutoComWeb.Models;

[Table("mechanics")]
public partial class Mechanic
{
    [Key]
    [Column("mechanic_id")]
    public int MechanicId { get; set; }

    [Column("first_name")]
    [StringLength(50)]
    public string FirstName { get; set; } = null!;

    [Column("last_name")]
    [StringLength(50)]
    public string LastName { get; set; } = null!;

    [Column("patronymic")]
    [StringLength(50)]
    public string? Patronymic { get; set; }

    [Column("phone")]
    [StringLength(20)]
    public string Phone { get; set; } = null!;

    [Column("specialization")]
    [StringLength(50)]
    public string Specialization { get; set; } = null!;

    [InverseProperty("Mechanic")]
    public virtual ICollection<WorkOrder> WorkOrders { get; set; } = new List<WorkOrder>();
}
