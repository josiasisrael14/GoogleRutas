using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace GoogleRuta.Models;

public class Nodo
{
    [Key]
    //el id llave primaria 
    public int Id { get; set; }

    //typo que puede ser cable o spliter
    public string? Type { get; set; }
    public string? TypeSplitter { get; set; }
    //coordenadas x
    public double CoordinateX { get; set; }
    //coordenadas y
    public double CoordinateY { get; set; }

    public double Rotation { get; set; }
    public int? Size { get; set; }

     [Column(TypeName = "TEXT")]
     public string? StrandColorsJson { get; set; }
     
    public int DrawingId { get; set; }
    [ForeignKey("DrawingId")]
    public virtual Drawing Drawing { get; set; }

}