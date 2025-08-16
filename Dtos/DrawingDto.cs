using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GoogleRuta.Dtos;

public class DrawingDto
{
    public List<NodoDto> Nodos { get; set; }
    public List<ConnectionDto> Connections { get; set; }
}

// DTO para los nodos
public class NodoDto
{
    public string ClienteId { get; set; } // El ID temporal del cliente (ej: "cliente_abc123")
    public string Type { get; set; }
    public double CoordinateX { get; set; }
    public double CoordinateY { get; set; }
    public double Rotation { get; set; }
    public int? Size { get; set; }
     public string? StrandColorsJson { get; set; }
}

// DTO para las conexiones
public class ConnectionDto
{
    public string Color { get; set; }
    public int Thickness { get; set; }
    public string OrigenNodoClienteId { get; set; }
    public string DestinationNodoClienteId { get; set; }
    public string JsonIntermediatePoints { get; set; }
    public int OrigenPuntoIndex { get; set; }
    public int DestinoPuntoIndex { get; set; }
}