using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Samurai_V2_.Net_8.DbContexts.Models;

[Table("tblSales")]
public partial class TblSale
{
    [Key]
    public int SalesId { get; set; }

    public int CustomerId { get; set; }

    public DateTime SalesDate { get; set; }

    public bool IsActive { get; set; }
}
