using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GoogleRuta.Models;

namespace GoogleRuta.ViewModels
{
    public class ProjectViewModel
    {
        //public int ElementProjectId { get; set; }
        public int Id { get; set; }
        public string Name { get; set; }
        public List<CoordinateDto> Coordinates { get; set; }
         public List<PlacedElement>? PlacedElementse{ get; set; }
    }
}