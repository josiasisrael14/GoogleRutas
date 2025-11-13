using GoogleRuta.Models;

namespace GoogleRuta.Views.ViewModel
{
    public class TelecomViewModel
    {
        public IEnumerable<Odl> Odls { get; set; }
        public IEnumerable<Odf> Odfs { get; set; }
        public IEnumerable<Elfa> Elfas { get; set; }

        public IEnumerable<ConnectionTelecom> ExistingConnections { get; set; }

    }
}