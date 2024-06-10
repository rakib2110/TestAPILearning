using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Samurai_V2_.Net_8.DbContexts.Models;

[Table("tblPartnerType")]
public partial class TblPartnerType
{
    [Key]
    public int PartnerTypeId { get; set; }

    [StringLength(250)]
    public string PartnerTypeName { get; set; } = null!;

    public bool IsActive { get; set; }

    [InverseProperty("PartnerType")]
    public virtual ICollection<TblPartner> TblPartners { get; set; } = new List<TblPartner>();
}
