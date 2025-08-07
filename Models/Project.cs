using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GoogleRuta.Models;

public class Project
{
    [Key]
    public int Id { get; set; }
    public string Name { get; set; }
    public virtual ICollection<CoordinateB> CoordinateBs { get; set; } = new List<CoordinateB>();
    public virtual ICollection<ElementProject> ElementProjects { get; set; } = new List<ElementProject>();
    public virtual ICollection<ColorThreadProject> ColorThreadProjects { get; set; } = new List<ColorThreadProject>();

}
//project tiene muchas coordinatBs (puntos trazados)
//project tiene muchos elementProjects(cajas,postes)
//project tiene muchas colorThreadProjects(colores)


//Relacion de Project y CoordinateBs relacion de uno a muchos
//un proyecto tiene muchos puntos de coordenadas. Pero cada punto de coordenada pertenece a un solo proyecto

//rwlaicon de uno a muchos projecyt y elementProject
//un proyetco tiene muchos elementos colocados en ele . pero cada elemento pertenece  a un solo proyecto.

//relacion uno a muchos project y colorThreadProject
//un proyecto tiene muchas reglas de color pero cada regla de color pertenece a un solo proyecto