using System.Text.Json;
using GoogleRuta.Dtos;
using GoogleRuta.Models;
using GoogleRuta.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using GoogleRuta.ViewModels;
using GoogleRuta.Data;
using Microsoft.EntityFrameworkCore;

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

    [HttpPost]
    //[Route("ProjectController/Coordinate")]//[FromBody] DrawnData data
    public async Task<IActionResult> Coordinate([FromBody] DrawnData data)
    {
        if (data == null)
        {
            return BadRequest("No se recibieron datos o los datos son inválidos.");
        }

        _logger.LogInformation("Coordenadas recibidas");
        //var data = JsonSerializer.Deserialize<DrawnData>(jsonData);
        if (data.Coordinates != null && data.Coordinates.Any())
        {
            Console.WriteLine("Coordenadas recibidas");

            var proccessedCoordinates = new List<Object>();

            foreach (var coorPair in data.Coordinates)
            {
                if (coorPair.Count == 2)
                {
                    double lat = coorPair[0];
                    double lng = coorPair[1];

                    proccessedCoordinates.Add(new { lat = lat, lng = lng });

                    Console.WriteLine($"latitud :{lat}, longitud :{lng}");
                }
            }
            TempData["projectId"] = data.Id;
            TempData["Name"] = data.Name;
            TempData["ProcessedCoordinates"] = JsonSerializer.Serialize(proccessedCoordinates);
            TempData["SuccessMessage"] = "Coordenadas procesadas correctamente";

            if (data.PlacedElements != null && data.PlacedElements.Any())
            {
                Console.WriteLine("Coordenadas de los elementos recibidas");
                var ProcessedCoordinatesElements = new List<object>();
                foreach (var element in data.PlacedElements)
                {
                    ProcessedCoordinatesElements.Add(new
                    {
                        lat = element.Lat,
                        lng = element.Lng,
                        elementTypeId = element.ElementTypeId
                    });
                }
                TempData["PlacedElements"] = JsonSerializer.Serialize(ProcessedCoordinatesElements);
            }
        }
        else
        {
            Console.WriteLine("No se recibieron coordenadas.");
            TempData["ErrorMessage"] = "No se recibieron coordenadas válidas";
        }

        //return RedirectToAction("Index","Project");
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

        if (coordenadas.PlacedElements != null)
        {
            TempData["ExistingPlacedElements"] = JsonSerializer.Serialize(coordenadas.PlacedElements);
        }
        var redirectUrl = Url.Action("Diseño", "Project");
        return Json(new { success = true, redirectUrl = redirectUrl });
    }

    [HttpPost]
    [Route("Project/Save")]
    public async Task<IActionResult> Save([FromBody] SaveDrawingPayloadDto payload)
    {
        if (payload == null || !payload.DrawingData.Nodos.Any())
        {
            return BadRequest("No hay datos para guardar");
        }

        // Usaremos un diccionario para mapear los IDs temporales del cliente
        // a los nuevos IDs generados por la base de datos.
        var nododIdMap = new Dictionary<string, int>();

        // Iniciamos una transacción para asegurar que todo se guarde o nada se guarde.
        await using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            // 1. Crear el objeto Drawing principal
            var newPaint = new Drawing();
            _context.Drawings.Add(newPaint);
            await _context.SaveChangesAsync();// Guardamos para obtener el ID del nuevo dibujo

            foreach (var nodoDto in payload.DrawingData.Nodos)
            {
                var newNodo = new Nodo
                {
                    Type = nodoDto.Type,
                    CoordinateX = nodoDto.CoordinateX,
                    CoordinateY = nodoDto.CoordinateY,
                    DrawingId = newPaint.Id, // Asociamos al dibujo principal
                    Rotation = nodoDto.Rotation,
                    Size = nodoDto.Size,
                    StrandColorsJson = nodoDto.Type == "cable" ? nodoDto.StrandColorsJson : null
                    
                };

                _context.Nodos.Add(newNodo);
                await _context.SaveChangesAsync();
                nododIdMap[nodoDto.ClienteId] = newNodo.Id;
            }

            foreach (var connDto in payload.DrawingData.Connections)
            {
                var newConnection = new Connection
                {
                    Color = connDto.Color,
                    Thickness = connDto.Thickness,
                    JsonIntermediatePoints = connDto.JsonIntermediatePoints,
                    DrawingId = newPaint.Id,
                    OrigenNodoId = nododIdMap[connDto.OrigenNodoClienteId],
                    DestinationNodoId = nododIdMap[connDto.DestinationNodoClienteId],
                    OrigenPuntoIndex = connDto.OrigenPuntoIndex,
                    DestinoPuntoIndex = connDto.DestinoPuntoIndex

                };

                _context.Connections.Add(newConnection);
            }

            await _context.SaveChangesAsync();// Guardamos todas las conexiones

            var elementProject = await _context.ElementProjects.FirstOrDefaultAsync(ep =>
                                ep.Id == payload.ElementProjectId);
            // ep.ProjectId == payload.ProjectId &&
            // ep.Lat == payload.Lat &&
            // ep.Lng == payload.Lng);

            if (elementProject != null)
            {
                elementProject.DrawingId = newPaint.Id;
                await _context.SaveChangesAsync();
            }



            await transaction.CommitAsync();

            return Ok(new
            {
                message = "",
                drawingId = newPaint.Id,
            });


        }

        catch (Exception ex)
        {

            await transaction.RollbackAsync();

            return StatusCode(500, "ocurrio un error interno al guardar el dibujo");

        }
    }


    // EN TU ProjectController.cs

    [HttpGet]
    [Route("Project/GetDrawing/{drawingId}")] // Una ruta clara para obtener un dibujo por su ID
    public async Task<IActionResult> GetDrawing(int drawingId)
    {
        if (drawingId <= 0)
        {
            return BadRequest("ID de dibujo no válido.");
        }

        try
        {
            // Buscamos el dibujo y usamos .Include() para cargar también sus Nodos y Connections relacionados.
            var drawing = await _context.Drawings
                .Include(d => d.Nodos)
                .Include(d => d.Connections)
                .FirstOrDefaultAsync(d => d.Id == drawingId);

            if (drawing == null)
            {
                return NotFound("No se encontró ningún dibujo con ese ID.");
            }

            // Creamos un DTO para enviar los datos de forma limpia. Podemos reutilizar el que ya tenemos.
            var drawingDto = new DrawingDto
            {
                Nodos = drawing.Nodos.Select(n => new NodoDto
                {
                    // NOTA: Enviamos el ID real del nodo por si lo necesitas. El ClienteId solo se usa para guardar.
                    ClienteId = "db_nodo_" + n.Id, // Creamos un ID único basado en el ID de la BD
                    Type = n.Type,
                    CoordinateX = n.CoordinateX,
                    CoordinateY = n.CoordinateY,
                    Rotation = n.Rotation,
                    Size = n.Size,
                    StrandColorsJson = n.StrandColorsJson
                    
                    // TypeSplitter se puede añadir si es necesario
                }).ToList(),

                Connections = drawing.Connections.Select(c => new ConnectionDto
                {
                    Color = c.Color,
                    Thickness = c.Thickness ?? 2, // Usamos un valor por defecto si es nulo
                                                  // Buscamos los ClienteId que acabamos de crear para los nodos de origen y destino
                    OrigenNodoClienteId = "db_nodo_" + c.OrigenNodoId,
                    DestinationNodoClienteId = "db_nodo_" + c.DestinationNodoId,
                    JsonIntermediatePoints = c.JsonIntermediatePoints,
                    OrigenPuntoIndex = c.OrigenPuntoIndex,
                     DestinoPuntoIndex = c.DestinoPuntoIndex
                }).ToList()
            };

        System.Console.WriteLine("DEBUG C#: " + System.Text.Json.JsonSerializer.Serialize(drawingDto, new JsonSerializerOptions { WriteIndented = true }));


            return Ok(drawingDto); // Enviamos los datos del dibujo como respuesta JSON
        }
        catch (Exception ex)
        {
            // Loguear el error (ex)
            return StatusCode(500, "Ocurrió un error interno al obtener los datos del dibujo.");
        }
    }


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

    }



}