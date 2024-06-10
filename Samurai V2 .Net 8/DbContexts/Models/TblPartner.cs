using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Samurai_V2_.Net_8.DbContexts.Models;

[Table("tblPartner")]
public partial class TblPartner
{
    [Key]
    public int PartnerId { get; set; }

    [StringLength(250)]
    public string PartnerName { get; set; } = null!;

    public int PartnerTypeId { get; set; }

    public bool IsActive { get; set; }

    [ForeignKey("PartnerTypeId")]
    [InverseProperty("TblPartners")]
    public virtual TblPartnerType PartnerType { get; set; } = null!;
}
