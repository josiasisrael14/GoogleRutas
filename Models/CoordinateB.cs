using System.ComponentModel.DataAnnotations;

namespace GoogleRuta.Models;

public class CoordinateB
{
    [Key]
    public int Id { get; set; }

    public double Lat { get; set; }
    public double Lng { get; set; }

    // Clave foránea al proyecto al que pertenece
    public int ProjectId { get; set; }
    public virtual Project Project { get; set; }

    // public virtual ICollection<ElementProject> ElementProjects { get; set; } = new List<ElementProject>();
}