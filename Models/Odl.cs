using System.ComponentModel.DataAnnotations;

namespace GoogleRuta.Models;

public class Odl
{

    [Key]

    public int Id { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public int TotalPorts { get; set; }
    public int ColumnCount { get; set; }
    public int PortsPerColumn { get; set; }
    public ICollection<ConnectionTelecom> ConnectionTelecoms { get; set; } = new List<ConnectionTelecom>();

}