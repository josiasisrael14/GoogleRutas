using System.Text.Json;
using GoogleRuta.Dtos;
using GoogleRuta.Models;
using GoogleRuta.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using GoogleRuta.ViewModels;

namespace GoogleRuta.Controllers;

public class ProjectController : Controller
{
    private readonly IProjectService _service;
    private readonly IColorTracesService _serviceColor;
    private readonly ILogger<ProjectController> _logger;
    private readonly IElementService _elementService;
    public ProjectController(IProjectService service, ILogger<ProjectController> logger, IElementService elementService, IColorTracesService serviceColor)
    {
        _service = service;
        _logger = logger;
        _elementService = elementService;
        _serviceColor= serviceColor;
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
        var colors= await _serviceColor.GetAll();
        var model = new DiseñoViewModel
        {
            InitialLat = lat,
            InitialLng = lng,
            ElementTypes = elementTypes,
            ColorTraces=colors.ToList()
            
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