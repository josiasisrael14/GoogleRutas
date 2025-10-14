using System.ComponentModel.DataAnnotations;

namespace GoogleRuta.Models;

public class Elfa
{
    [Key]
    public int Id { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public int TotalPorts { get; set; }
    public int GroupCount { get; set; }
    public int PortsPerGroup { get; set; }
    public bool IsDuplex { get; set; } = true;
    public ICollection<ConnectionTelecom> ConnectionTelecoms { get; set; } = new List<ConnectionTelecom>();


}
