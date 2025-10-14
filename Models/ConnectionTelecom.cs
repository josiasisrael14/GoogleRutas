using System.ComponentModel.DataAnnotations;
using System.Data;

namespace GoogleRuta.Models;

public class ConnectionTelecom
{
    [Key]
    public int Id { get; set; }
    public string? Description { get; set; }
    public string? Datetime { get; set; }
    //relacion con ODL
    public int OdlId { get; set; }
    public Odl Odl { get; set; } = null!;
    public int PortOdl { get; set; }

    //relacion con ELFA
    public int ElfaId { get; set; }
    public Elfa Elfa { get; set; } = null!;
    public int PortElfaInput { get; set; }
    public int PortElfaOutput { get; set; }

    //relacion con ODF
    public int OdfId { get; set; }
    public Odf Odf { get; set; } = null!;
    public int PortOdf { get; set; }




}
