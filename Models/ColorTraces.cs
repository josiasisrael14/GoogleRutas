using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;

namespace GoogleRuta.Models;

public class ColorTraces
{
    [Key]
    public int Id { get; set; }
    public string Name { get; set; }
    public string? Color { get; set; }
    public string? Description { get; set; }
    public virtual ICollection<ColorThreadProject> ColorThreadProjects{ get; set; } = new List<ColorThreadProject>();
}