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
        public string Type { get; set; }
        public List<CoordinateDto> Coordinates { get; set; }
        public string LineColor { get; set; }
        public List<PolylineSegmentDto> Segments { get; set; }
        public List<PlacedElement>? PlacedElementse { get; set; }
    }

    public class PolylineSegmentDto
    {
        public string SegmentId { get; set; }
        public string Color { get; set; }
        public List<CoordinateDto> Coordinates { get; set; }
    }
}