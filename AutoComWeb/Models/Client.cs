using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace AutoComWeb.Models;

[Table("clients")]
[Index("UserId", Name = "clients_user_id_key", IsUnique = true)]
public partial class Client
{
    [Key]
    [Column("client_id")]
    public int ClientId { get; set; }

    [Column("user_id")]
    public int? UserId { get; set; }

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

    [Column("email")]
    [StringLength(200)]
    public string? Email { get; set; }

    [ForeignKey("UserId")]
    [InverseProperty("Client")]
    public virtual User? User { get; set; }

    [InverseProperty("Client")]
    public virtual ICollection<Vehicle> Vehicles { get; set; } = new List<Vehicle>();

    [InverseProperty("Client")]
    public virtual ICollection<WorkOrder> WorkOrders { get; set; } = new List<WorkOrder>();
}
