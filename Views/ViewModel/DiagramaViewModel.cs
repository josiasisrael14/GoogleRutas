using GoogleRuta.Models;
using GoogleRuta.Dtos;

namespace GoogleRuta.Views.ViewModel
{
    public class DiagramaViewModel
    {
        public IEnumerable<Router> Routers { get; set; }
        public IEnumerable<Switchs> Switches { get; set; }
        public IEnumerable<Odl> Odls { get; set; }
        public IEnumerable<Odf> Odfs { get; set; }
        public IEnumerable<Elfa> Elfas { get; set; }
        public string DiagramJson { get; set; }
        public IEnumerable<ConnectionEquipmentDto> Connections { get; set; }
         public IEnumerable<ConnectionTelecom> TelecomConnections { get; set; }
    }
}