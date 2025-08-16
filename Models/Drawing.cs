using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;

namespace GoogleRuta.Models;

public class Drawing
{
    [Key]
    //Id primar key
    public int Id { get; set; }
    // tiene coleccion de nodos 
    public virtual ICollection<Nodo> Nodos { get; set; }
    //tiene coleccion de conectores 
    public virtual ICollection<Connection> Connections { get; set; }
}