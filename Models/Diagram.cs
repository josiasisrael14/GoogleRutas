using System.ComponentModel.DataAnnotations.Schema;


namespace GoogleRuta.Models;
    public class Diagram
    {
        public int Id { get; set; }
        public string Name { get; set; }

        // Guarda el estado completo del lienzo de Konva como un string JSON
        [Column(TypeName = "TEXT")]
        public string JsonContent { get; set; }
    }
