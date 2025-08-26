using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GoogleRuta.Dtos;

public class SaveDrawingPayloadDto
{

    public int? ElementProjectId { get; set; }
    public int ProjectId { get; set; }
    public double Lat { get; set; }
    public double Lng { get; set; }
    public int ElementTypeId { get; set; }

    // Datos del dibujo

    public int? DrawingId { get; set; }
    public string SvgContent { get; set; }
    public string JsonContent { get; set; }

    // Esta propiedad contendrá los datos del dibujo (nodos y conexiones)
    public DrawingDto DrawingData { get; set; }

}