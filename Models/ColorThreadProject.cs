using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;

namespace GoogleRuta.Models;

public class ColorThreadProject
{
    [Key]
    public int Id { get; set; }
    public int? ProjectId { get; set; }
    [ForeignKey("ProjectId")]
    public virtual Project? Project { get; set; }

    public int? ColorTracesId { get; set; }
    [ForeignKey("ColorTracesId")]
    public virtual ColorTraces? ColorTraces { get; set; }

    // public int? ElementTypeId { get; set; }
    // [ForeignKey("ElementTypeId")]
    // public virtual ElementType? ElementType { get; set; }
}