using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace GoogleRuta.Models;

public class Connection
{
    [Key]

    public int Id { get; set; }

    // **¡Este es el campo que usaremos para el color de la línea!**
    public string Color { get; set; } // Por ejemplo, "#FF0000" para rojo, "#000000" para negro

    public string? Type { get; set; }
    public int? Thickness { get; set; }

    public int OrigenPuntoIndex { get; set; }
    public int DestinoPuntoIndex { get; set; }

    public int DrawingId { get; set; }
    [ForeignKey("DrawingId")]
    public virtual Drawing Drawing { get; set; }

    // Relación con el nodo de origen
    public int OrigenNodoId { get; set; }
    [ForeignKey("OriginNodoId")]
    public virtual Nodo OriginNodo { get; set; }

    // Relación con el nodo de destino
    public int DestinationNodoId { get; set; }
    [ForeignKey("DestinationNodoId")]
    public virtual Nodo DestinationNodo { get; set; }

    // ... otros campos como HiloNum, etc.
    [Column(TypeName = "TEXT")]
    public string JsonIntermediatePoints { get; set; }
}
