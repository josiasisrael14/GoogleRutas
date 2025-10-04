using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GoogleRuta.Dtos
{
    public class ImagePayloadDto
    {
        public int ElementProjectId { get; set; }
        public string Base64Image { get; set; }
    }
}