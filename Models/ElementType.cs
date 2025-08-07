using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GoogleRuta.Models;

public class ElementType
{

    [Key]// se usa en el usin System.ComponentModel.DataAnnotations  
    public int Id { get; set; }
    public string Name { get; set; }
    public string? IconoUrl { get; set; }
    public virtual ICollection<ElementProject> ElementProject { get; set; } = new List<ElementProject>();
     //public virtual ICollection<ColorThreadProject> ColorThreadProjects { get; set; } = new List<ColorThreadProject>();
}