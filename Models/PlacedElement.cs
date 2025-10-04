using System.Text.Json.Serialization;

namespace GoogleRuta.Models;

public class PlacedElement
{

    //[JsonPropertyName("databaseId")]
    public int? Id { get; set; }
    public int? DrawingId { get; set; }
    public double Lat { get; set; }
    public double Lng { get; set; }
    public int? ElementTypeId { get; set; }
    public int? DatabaseId { get; set; }
}