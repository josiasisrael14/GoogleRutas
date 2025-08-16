using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GoogleRuta.Models;

public class PlacedElement
{

    public int? Id { get; set; }
    public int? DrawingId { get; set; }
    public double Lat { get; set; }
    public double Lng { get; set; }
    public int? ElementTypeId { get; set; }
}