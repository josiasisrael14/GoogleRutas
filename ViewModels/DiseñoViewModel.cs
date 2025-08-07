
using GoogleRuta.Models;

namespace GoogleRuta.ViewModels;

public class DiseñoViewModel
{
    public double? InitialLat { get; set; } // Ahora es una propiedad del modelo
    public double? InitialLng { get; set; } // Ahora es una propiedad del modelo
    public List<ElementType> ElementTypes { get; set; } = new List<ElementType>();
    public List<ColorTraces> ColorTraces{ get; set; } = new List<ColorTraces>();
    
}