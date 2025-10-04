using System.ComponentModel.DataAnnotations;

namespace GoogleRuta.Models;

public class Router
{
    [Key]
    public int Id { get; set; }
    public string Name { get; set; }
    public string? IPAddress { get; set; }
    public string? Description { get; set; }
}