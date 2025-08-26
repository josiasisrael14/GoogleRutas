using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace GoogleRuta.Models;

public class Drawing
{
    [Key]
    //Id primar key
    public int Id { get; set; }


    // Columna para guardar la estructura editable de Konva
    [Column(TypeName = "TEXT")]
    public string? JsonContent { get; set; }

    // Columna para guardar la representación visual como SVG
    [Column(TypeName = "TEXT")]
    public string? SvgContent { get; set; }
    // tiene coleccion de nodos 
    public virtual ICollection<Nodo> Nodos { get; set; }
    //tiene coleccion de conectores 
    public virtual ICollection<Connection> Connections { get; set; }
}