using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GoogleRuta.Dtos;

public class ProjectAddDto
{
    public int Id { get; set; }
    public string Name { get; set; }

    public double Latitude { get; set; }
    public double Length { get; set; }
    public string Message { get; set; }
    public bool Success { get; set; }

}