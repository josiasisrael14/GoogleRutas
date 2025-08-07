using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GoogleRuta.Models;

public class DrawnData
{
    public int? Id { get; set; }
    public string? Name { get; set; }
    public string Type { get; set; }
    public List<List<double>>? Coordinates { get; set; }
    public List<PlacedElement>? PlacedElements { get; set; }
}