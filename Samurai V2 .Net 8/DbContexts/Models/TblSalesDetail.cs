using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Samurai_V2_.Net_8.DbContexts.Models;

[Table("tblSalesDetails")]
public partial class TblSalesDetail
{
    [Key]
    public int DetailsId { get; set; }

    public int SalesId { get; set; }

    public int ItemId { get; set; }

    public int ItemQuantity { get; set; }

    [Column(TypeName = "decimal(18, 3)")]
    public decimal UnitPrice { get; set; }

    public bool IsActive { get; set; }
}
