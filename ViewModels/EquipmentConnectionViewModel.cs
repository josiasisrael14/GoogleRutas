using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GoogleRuta.Models;
using GoogleRuta.Dtos;

namespace GoogleRuta.ViewModels;

public class EquipmentConnectionViewModel
{
    // Propiedad para la lista de conexiones existentes (para el DataTable)
    // Usamos 'object' porque en el controlador lo creas como un tipo anónimo.
    public IEnumerable<ConnectionEquipmentDto> Connections { get; set; }

    // Propiedad para la lista de todos los routers disponibles (para el <select>)
    public IEnumerable<Router> AllRouters { get; set; }

    // Propiedad para la lista de todos los switches disponibles (para el <select>)
    public IEnumerable<Switchs> AllSwitches { get; set; }
}
