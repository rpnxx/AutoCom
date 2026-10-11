using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace AutoComWeb.Models;

[Table("users")]
[Index("Phone", Name = "users_phone_key", IsUnique = true)]
public partial class User
{
    [Key]
    [Column("user_id")]
    public int UserId { get; set; }

    [Column("phone")]
    [StringLength(20)]
    public string Phone { get; set; } = null!;

    [Column("passwordhash")]
    [StringLength(255)]
    public string Passwordhash { get; set; } = null!;

    [Column("role")]
    [StringLength(20)]
    public string Role { get; set; } = null!;

    [InverseProperty("User")]
    public virtual Client? Client { get; set; }
}
