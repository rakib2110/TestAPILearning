using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Samurai_V2_.Net_8.DbContexts.Models;

[Table("tblItem")]
public partial class TblItem
{
    [Key]
    public int ItemId { get; set; }

    [StringLength(255)]
    public string ItemName { get; set; } = null!;

    public int StockQuantity { get; set; }

    public string ImageUrl { get; set; }

    public bool? IsActive { get; set; }
}
