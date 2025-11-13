using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GoogleRuta.Models;

public class Odf
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public int TotalPorts { get; set; }
    public int GroupCount { get; set; }
    public int PortsPerGroup { get; set; }

    public ICollection<ConnectionTelecom> ConnectionTelecoms { get; set; } = new List<ConnectionTelecom>();


}