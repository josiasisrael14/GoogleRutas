using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace GoogleRuta.Models;

public class ElementProject
{
    [Key]
    public int Id { get; set; }

    public int? ElementTypeId { get; set; }
    [ForeignKey("ElementTypeId")]//se usa en el usin system.ComponentModel.DataAnnotations.Schema
    public virtual ElementType? ElementType { get; set; }
    public string? Description { get; set; }
    public int ProjectId { get; set; }
    [ForeignKey("ProjectId")]
    public virtual Project Project { get; set; }

    public double Lat { get; set; }
    public double Lng { get; set; }

    public int? DrawingId { get; set; }
    [ForeignKey("DrawingId")]
    public virtual Drawing Drawing { get; set; }
    // public int CoordinateBId { get; set; }
    // [ForeignKey("CoordinateBId")]
    // public virtual CoordinateB CoordinateB { get; set; }

}