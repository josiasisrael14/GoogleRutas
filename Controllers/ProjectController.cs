using System.Text.Json;
using GoogleRuta.Dtos;
using GoogleRuta.Models;
using GoogleRuta.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using GoogleRuta.ViewModels;
using GoogleRuta.Data;
using SixLabors.ImageSharp;

namespace GoogleRuta.Controllers;

public class ProjectController : Controller
{
    private readonly IProjectService _service;
    private readonly IColorTracesService _serviceColor;
    private readonly ILogger<ProjectController> _logger;
    private readonly IElementService _elementService;

    //por el momento luego borrar y actualizar al servicio
    private readonly ApplicationDbContext _context;
    public ProjectController(IProjectService service, ILogger<ProjectController> logger, IElementService elementService, IColorTracesService serviceColor, ApplicationDbContext context)
    {
        _service = service;
        _logger = logger;
        _elementService = elementService;
        _serviceColor = serviceColor;
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var projects = await _service.GetAll();
        return View(projects);
    }

    [HttpGet]
    [Route("Project/GetById/{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        try
        {
            var project = await _service.GetById(id);
            return Json(project);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener el proyecto por ID");
            return BadRequest("No se pudo obtener el proyecto.");
        }
    }

    [HttpGet]
    [Route("Project/GetAll")]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var projects = await _service.GetAll();
            return Json(projects ?? new List<ProjectViewModel>());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener la lista de proyectos");
            return BadRequest("No se pudo obtener la lista de proyectos.");
        }
    }

    // [HttpPost]
    // //[Route("ProjectController/Coordinate")]//[FromBody] DrawnData data
    // public async Task<IActionResult> Coordinate([FromBody] DrawnData data)
    // {   //se modifico == pos is null
    //     if (data is null)
    //     {
    //         return BadRequest("No se recibieron datos o los datos son inválidos.");
    //     }

    //     _logger.LogInformation("Coordenadas recibidas");
    //     //var data = JsonSerializer.Deserialize<DrawnData>(jsonData);
    //     if (data.Coordinates != null && data.Coordinates.Any())
    //     {
    //         Console.WriteLine("Coordenadas recibidas");

    //         var proccessedCoordinates = new List<Object>();

    //         foreach (var coorPair in data.Coordinates)
    //         {
    //             if (coorPair.Count == 2)
    //             {
    //                 double lat = coorPair[0];
    //                 double lng = coorPair[1];

    //                 proccessedCoordinates.Add(new { lat = lat, lng = lng });

    //                 Console.WriteLine($"latitud :{lat}, longitud :{lng}");
    //             }
    //         }
    //         TempData["projectId"] = data.Id;
    //         TempData["Name"] = data.Name;
    //         TempData["ProcessedCoordinates"] = JsonSerializer.Serialize(proccessedCoordinates);
    //         TempData["SuccessMessage"] = "Coordenadas procesadas correctamente";

    //         if (data.PlacedElements != null && data.PlacedElements.Any())
    //         {
    //             Console.WriteLine("Coordenadas de los elementos recibidas");
    //             var ProcessedCoordinatesElements = new List<object>();
    //             foreach (var element in data.PlacedElements)
    //             {
    //                 ProcessedCoordinatesElements.Add(new
    //                 {
    //                     lat = element.Lat,
    //                     lng = element.Lng,
    //                     elementTypeId = element.ElementTypeId
    //                 });
    //             }
    //             TempData["PlacedElements"] = JsonSerializer.Serialize(ProcessedCoordinatesElements);
    //         }
    //     }
    //     else
    //     {
    //         Console.WriteLine("No se recibieron coordenadas.");
    //         TempData["ErrorMessage"] = "No se recibieron coordenadas válidas";
    //     }

    //     //return RedirectToAction("Index","Project");
    //     return Ok(new { redirectToUrl = Url.Action("Index", "Project") });


    // }

    [HttpPost]
    public IActionResult Coordinate([FromBody] DrawnData data)
    {
        if (data is null)
        {
            // En este flujo, simplemente redirigimos incluso si no hay datos.
            return Ok(new { redirectToUrl = Url.Action("Index", "Project") });
        }

        // Guardamos los datos recibidos en TempData para que la siguiente petición (Index) los pueda leer.
        TempData["ProjectId"] = data.Id;
        TempData["Name"] = data.Name;

        // Guardamos los segmentos si existen
        if (data.Segments != null && data.Segments.Any())
        {
            TempData["SavedSegments"] = JsonSerializer.Serialize(data.Segments);
        }

        // Guardamos las coordenadas de polígono si existen (por retrocompatibilidad)
        if (data.Coordinates != null && data.Coordinates.Any())
        {
            TempData["SavedCoordinates"] = JsonSerializer.Serialize(data.Coordinates);
        }

        // Guardamos los elementos colocados si existen
        if (data.PlacedElements != null && data.PlacedElements.Any())
        {
            TempData["PlacedElements"] = JsonSerializer.Serialize(data.PlacedElements);
        }

        // Devolvemos el JSON que le dice al frontend a dónde redirigir.
        return Ok(new { redirectToUrl = Url.Action("Index", "Project") });
    }
    public async Task<IActionResult> Diseño(double? lat, double? lng)
    {
        var elementTypes = await _elementService.GetAll();
        var colors = await _serviceColor.GetAll();
        var model = new DiseñoViewModel
        {
            InitialLat = lat,
            InitialLng = lng,
            ElementTypes = elementTypes,
            ColorTraces = colors.ToList()

        };

        //ViewData["InitialLat"] = lat;
        //ViewData["InitialLng"] = lng;

        return View("Diseño", model);
    }

    [HttpPost]
    [Route("ProjectController/Add")]

    public async Task<IActionResult> Add([FromBody] ProjectViewModel viewModel)
    {
        try
        {
            await _service.Add(viewModel);
            return Ok(new { message = "Proyecto creaddo" });
        }
        catch (Exception ex)
        {

            _logger.LogError(ex, "Ocurrio un error al crear");
            return StatusCode(500, new { messagge = "Ocurrió un error inesperado al procesar su solicitud." });

        }
    }

    [HttpPost]
    [Route("ProjectController/GoToDesign")]
    public async Task<IActionResult> GoToDesign(double? lat, double? lng)
    {

        //ViewData["InitialLat"] = lat;
        //ViewData["InitialLng"] = lng;
        var redirectUrl = Url.Action("Diseño", "Project", new { lat = lat, lng = lng });
        return Json(new { success = true, redirectUrl = redirectUrl });
        //return Json(new { redirectUrl = Url.Action("Diseño", "Project", new { lat = lat, lng = lng }) });
        //return RedirectToAction("Diseño", new { lat, lng });

    }

    [HttpDelete]
    [Route("Project/Delete/{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        if (id <= 0)
        {
            return BadRequest("El identificador del proyecto no puede estar vacio");
        }

        var searchProject = await _service.GetById(id);

        if (searchProject == null)
        {
            return NotFound("El proyecto no existe");
        }

        var result = await _service.Delete(searchProject.Id);

        if (result)
        {
            return Ok(new
            {
                message = "Eliminado Correctamente",
            });
        }
        else
        {
            return StatusCode(500, new { message = "Error interno al eliminar el proyecto." });

        }

    }

    [HttpPost]
    [Route("Project/RedirectToDiseno")]
    public IActionResult RedirectToDiseno([FromBody] CoordinateListWrapper coordenadas)
    {
        TempData["ProcessedCoordinatess"] = JsonSerializer.Serialize(coordenadas.Coordinates);
        TempData["ProjectId"] = coordenadas.ProjectId;
        TempData["Name"] = coordenadas.Name;
        TempData["ExistingSegments"] = JsonSerializer.Serialize(coordenadas.Segments ?? new List<PolylineSegmentDto>());

        if (coordenadas.PlacedElements != null)
        {
            TempData["ExistingPlacedElements"] = JsonSerializer.Serialize(coordenadas.PlacedElements);
        }
        var redirectUrl = Url.Action("Diseño", "Project");
        return Json(new { success = true, redirectUrl = redirectUrl });
    }

    // En tu ProjectController.cs

    [HttpPost]
    [Route("Project/Save")]
    public async Task<IActionResult> Save([FromBody] SaveDrawingPayloadDto payload)
    {
        // Verificación simple
        if (payload == null || string.IsNullOrEmpty(payload.JsonContent))
        {
            return BadRequest("No hay datos de dibujo para guardar.");
        }

        // Ya no necesitamos diccionarios ni transacciones complejas para esto.
        // Entity Framework manejará la transacción por nosotros en este caso simple.
        try
        {

            if (payload.DrawingId > 0)
            {
                var existId = await _context.Drawings.FindAsync(payload.DrawingId);

                if (existId != null)
                {
                    existId.JsonContent = payload.JsonContent;
                    existId.SvgContent = payload.SvgContent;
                    await _context.SaveChangesAsync();
                    return Ok(new
                    {
                        message = "Dibujo actualizado con éxito.",
                        success = true,
                    });

                }
                else
                {
                    return Ok(new
                    {
                        message = "No se encontró el dibujo para actualizar.",
                        success = false,
                    });
                }

            }
            // 1. Crear y guardar el nuevo objeto Drawing con el SVG y el JSON
            var newDrawing = new Drawing
            {
                JsonContent = payload.JsonContent,
                SvgContent = payload.SvgContent
            };
            _context.Drawings.Add(newDrawing);
            await _context.SaveChangesAsync(); // Guardamos para que newDrawing obtenga su ID

            // 2. Encontrar el ElementProject y asociarle el ID del nuevo dibujo
            var elementProject = await _context.ElementProjects.FindAsync(payload.ElementProjectId);
            if (elementProject != null)
            {
                elementProject.DrawingId = newDrawing.Id;
                await _context.SaveChangesAsync();
            }
            else
            {
                // Si no se encuentra el elemento, podríamos revertir, pero por ahora solo avisamos.
                return NotFound($"No se encontró el ElementProject con ID: {payload.ElementProjectId}");
            }

            return Ok(new
            {
                message = "Dibujo guardado con éxito.",
                drawingId = newDrawing.Id
            });
        }
        catch (Exception ex)
        {
            // Loguear el error (ex)
            return StatusCode(500, "Ocurrió un error interno al guardar el dibujo.");
        }
    }

    // [HttpPost]
    // public async Task<IActionResult> SaveImage([FromBody] ImagePayloadDto payload)
    // {
    //     //valido si la variable payload es null o si la propiedad base64Image es nula o una cadena vacia
    //     if (payload == null || string.IsNullOrEmpty(payload.Base64Image))
    //     {
    //         return BadRequest("No se proporcionó una imagen válida.");
    //     }

    //     try
    //     {
    //         // Buscar el ElementProject por su ID, porque si existe entonces actualizo la imagen
    //         var elementProject = await _context.ElementProjects.FindAsync(payload.ElementProjectId);
    //         if (elementProject == null)
    //         {
    //             return NotFound($"No se encontró el ElementProject con ID: {payload.ElementProjectId}");
    //         }

    //         // Convertir la cadena Base64 a un arreglo de bytes
    //         byte[] imageBytes = Convert.FromBase64String(payload.Base64Image);

    //         // Asignar la imagen al ElementProject
    //         // Suponiendo que tienes una propiedad Image de tipo byte[] en ElementProject
    //         elementProject.Image = imageBytes;

    //         // Guardar los cambios en la base de datos
    //         await _context.SaveChangesAsync();

    //         return Ok(new
    //         {
    //             message = "Imagen guardada correctamente.",
    //             redirectToUrl = Url.Action("Index", "Project")
    //         });
    //     }
    //     catch (FormatException)
    //     {
    //         return BadRequest("El formato de la imagen Base64 es inválido.");
    //     }
    //     catch (Exception ex)
    //     {
    //         // Loguear el error (ex)
    //         return StatusCode(500, "Ocurrió un error interno al guardar la imagen.");
    //     }
    // }

    [HttpPost]
    public async Task<IActionResult> SaveImage([FromBody] ImagePayloadDto payload)
    {
        if (payload == null || string.IsNullOrEmpty(payload.Base64Image))
        {
            return BadRequest("No se proporcionó una imagen válida.");
        }

        try
        {
            // Convertir la cadena Base64 a un arreglo de bytes (esto no cambia)
            byte[] imageBytes = Convert.FromBase64String(payload.Base64Image);

            // ✅ PASO 2: ¡AQUÍ ESTÁ LA VALIDACIÓN!
            try
            {
                // Usamos un MemoryStream para que ImageSharp pueda leer los bytes como si fuera un archivo.
                using (var stream = new MemoryStream(imageBytes))
                {
                    // Image.Identify() es muy rápido. Solo lee la cabecera del archivo para
                    // identificar el formato, sin decodificar toda la imagen.
                    // Si esto no lanza una excepción, es una imagen que ImageSharp reconoce (JPG, PNG, GIF, etc.).
                    var imageInfo = Image.Identify(stream);
                    if (imageInfo == null)
                    {
                        // Doble chequeo por si acaso, aunque Identify lanzaría excepción antes.
                        return BadRequest("El archivo proporcionado no parece ser una imagen válida.");
                    }
                }
            }
            catch (UnknownImageFormatException)
            {
                // Esta excepción se lanza si los datos no corresponden a ningún formato de imagen conocido.
                return BadRequest("El formato del archivo es desconocido o no es una imagen válida.");
            }
            catch // Captura otras posibles excepciones de ImageSharp
            {
                return BadRequest("El archivo proporcionado está corrupto o no es una imagen válida.");
            }
            // ✅ FIN DE LA VALIDACIÓN

            // Si llegamos aquí, la imagen es válida. Procedemos a buscar y guardar.
            var elementProject = await _context.ElementProjects.FindAsync(payload.ElementProjectId);
            if (elementProject == null)
            {
                return NotFound($"No se encontró el ElementProject con ID: {payload.ElementProjectId}");
            }

            elementProject.Image = imageBytes;
            await _context.SaveChangesAsync();
        

            return Ok(new
            {
                message = "Imagen guardada correctamente.",
                redirectToUrl = Url.Action("Index", "Project", new { id = elementProject.ProjectId }) // Asumiendo que quieres volver al diseño del proyecto padre
            });
        }
        catch (FormatException)
        {
            return BadRequest("El formato de la imagen Base64 es inválido.");
        }
        catch (Exception ex)
        {
            // Loguear el error (ex)
            return StatusCode(500, "Ocurrió un error interno al guardar la imagen.");
        }
    }

    [HttpGet]
    [Route("Project/GetImage/{elementProjectId}")]
    public async Task<IActionResult> GetImage(int elementProjectId)
    {
        if (elementProjectId <= 0)
        {
            return BadRequest("ID de ElementProject no válido.");
        }

        var elementProject = await _context.ElementProjects.FindAsync(elementProjectId);
        if (elementProject == null || elementProject.Image == null || elementProject.Image.Length == 0)
        {
            return NotFound("No se encontró ninguna imagen para el ElementProject especificado.");
        }
        // Devolver la imagen como un archivo
        return File(elementProject.Image, "image/png"); // Ajusta el tipo MIME según el formato de la imagen


    }


    // En tu ProjectController.cs

    [HttpGet]
    [Route("Project/GetDrawing/{drawingId}")]
    public async Task<IActionResult> GetDrawing(int drawingId)
    {
        if (drawingId <= 0)
        {
            return BadRequest("ID de dibujo no válido.");
        }

        try
        {
            var drawing = await _context.Drawings.FindAsync(drawingId);

            if (drawing == null || string.IsNullOrEmpty(drawing.JsonContent))
            {
                return NotFound("No se encontró ningún dibujo editable con ese ID.");
            }

            // Devolvemos el contenido JSON directamente.
            // El frontend lo recibirá como un string y necesitará usar JSON.parse().
            return Content(drawing.JsonContent, "application/json");
        }
        catch (Exception ex)
        {
            // Loguear el error (ex)
            return StatusCode(500, "Ocurrió un error interno al obtener los datos del dibujo.");
        }
    }


    // EN TU ProjectController.cs

    // [HttpGet]
    // [Route("Project/GetDrawing/{drawingId}")] // Una ruta clara para obtener un dibujo por su ID
    // public async Task<IActionResult> GetDrawing(int drawingId)
    // {
    //     if (drawingId <= 0)
    //     {
    //         return BadRequest("ID de dibujo no válido.");
    //     }

    //     try
    //     {
    //         // Buscamos el dibujo y usamos .Include() para cargar también sus Nodos y Connections relacionados.
    //         var drawing = await _context.Drawings
    //             .Include(d => d.Nodos)
    //             .Include(d => d.Connections)
    //             .FirstOrDefaultAsync(d => d.Id == drawingId);

    //         if (drawing == null)
    //         {
    //             return NotFound("No se encontró ningún dibujo con ese ID.");
    //         }

    //         // Creamos un DTO para enviar los datos de forma limpia. Podemos reutilizar el que ya tenemos.
    //         var drawingDto = new DrawingDto
    //         {
    //             Nodos = drawing.Nodos.Select(n => new NodoDto
    //             {
    //                 // NOTA: Enviamos el ID real del nodo por si lo necesitas. El ClienteId solo se usa para guardar.
    //                 ClienteId = "db_nodo_" + n.Id, // Creamos un ID único basado en el ID de la BD
    //                 Type = n.Type,
    //                 CoordinateX = n.CoordinateX,
    //                 CoordinateY = n.CoordinateY,
    //                 Rotation = n.Rotation,
    //                 Size = n.Size,
    //                 StrandColorsJson = n.StrandColorsJson

    //                 // TypeSplitter se puede añadir si es necesario
    //             }).ToList(),

    //             Connections = drawing.Connections.Select(c => new ConnectionDto
    //             {
    //                 Color = c.Color,
    //                 Thickness = c.Thickness ?? 2, // Usamos un valor por defecto si es nulo
    //                                               // Buscamos los ClienteId que acabamos de crear para los nodos de origen y destino
    //                 OrigenNodoClienteId = "db_nodo_" + c.OrigenNodoId,
    //                 DestinationNodoClienteId = "db_nodo_" + c.DestinationNodoId,
    //                 JsonIntermediatePoints = c.JsonIntermediatePoints,
    //                 OrigenPuntoIndex = c.OrigenPuntoIndex,
    //                 DestinoPuntoIndex = c.DestinoPuntoIndex
    //             }).ToList()
    //         };

    //         System.Console.WriteLine("DEBUG C#: " + System.Text.Json.JsonSerializer.Serialize(drawingDto, new JsonSerializerOptions { WriteIndented = true }));


    //         return Ok(drawingDto); // Enviamos los datos del dibujo como respuesta JSON
    //     }
    //     catch (Exception ex)
    //     {
    //         // Loguear el error (ex)
    //         return StatusCode(500, "Ocurrió un error interno al obtener los datos del dibujo.");
    //     }
    // }


    // [HttpPost]
    // [Route("Project/LimpiarTempData")]
    // public IActionResult LimpiarTempData()
    // {
    //     var processed = TempData["ProcessedCoordinates"];
    //     var projectId = TempData["ProjectId"];
    //     Console.WriteLine($"[TempData] ProcessedCoordinates: {processed}");
    //     Console.WriteLine($"[TempData] ProjectId: {projectId}");
    //     TempData.Remove("ProcessedCoordinates");
    //     TempData.Remove("ProjectId");
    //     return Ok(new { success = true, message = "TempData limpiado correctamente" });
    // }

    public class CoordinateListWrapper
    {
        public int ProjectId { get; set; }
        public string Name { get; set; }
        public List<CoordinateDto> Coordinates { get; set; }
        public List<PlacedElement>? PlacedElements { get; set; }
        public List<PolylineSegmentDto>? Segments { get; set; }
    }



}