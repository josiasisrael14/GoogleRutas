using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GoogleRuta.Dtos;

public class ConnectionEquipmentDto
{
    public int Id { get; set; }
    public int SwitchId { get; set; }
    public string SwitchName { get; set; }
    public int PortNumber { get; set; }
    public int GroupNumber { get; set; }
    public string RouterName { get; set; }
    public int? RouterId { get; set; }
}
