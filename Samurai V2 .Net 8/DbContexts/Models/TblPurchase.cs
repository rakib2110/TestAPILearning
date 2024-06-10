using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Samurai_V2_.Net_8.DbContexts.Models;

[Table("tblPurchase")]
public partial class TblPurchase
{
    [Key]
    public int PurchaseId { get; set; }

    public int SupplierId { get; set; }

    public DateOnly PurchaseDate { get; set; }

    public bool IsActive { get; set; }
}
