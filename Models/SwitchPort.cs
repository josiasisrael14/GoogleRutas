using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace GoogleRuta.Models;

public class SwitchPort
{
    [Key]
    public int Id { get; set; }
    public int SwitchsId { get; set; }
    [ForeignKey("SwitchsId")]
    public virtual Switchs Switchs { get; set; }
    public int PortNumber { get; set; }
    public int GroupNumber { get; set; }
    public int RouterId { get; set; }
    [ForeignKey("RouterId")]
    public virtual Router? Router { get; set; }
}