using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GoogleRuta.ViewModels
{
    public class ElementViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? IconoUrl { get; set; }
        public IFormFile IconoFile { get; set; }
    }
}