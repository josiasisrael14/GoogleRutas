using Microsoft.AspNetCore.Mvc;
using GoogleRuta.Services.Interfaces;
using GoogleRuta.ViewModels;
namespace GoogleRuta.Controllers;

public class ElementController : Controller
{
    private readonly IElementService _service;
    private readonly ILogger<ElementController> _logger;
    public ElementController(IElementService service, ILogger<ElementController> logger)
    {
        _service = service;
        _logger = logger;

    }

    public async Task<IActionResult> Index()
    {
        var entities = await _service.GetAll();
        // List<ElementProject>

        var viewModels = entities.Select(e => new ElementViewModel
        {
            Id = e.Id,
            Name = e.Name,
            IconoUrl = e.IconoUrl,
            IconColor = e.IconColor
        }).ToList();

        return View(new ElementListViewModel
        {
            Elements = viewModels
        });
    }

    [HttpPost]
    [Route("Element/Add")]
    public async Task<IActionResult> Add(ElementViewModel elementViewModel)
    {
        try
        {
            await _service.Add(elementViewModel);
            return Ok(new { message = "Tipo Elemento Creado" });
        }
        catch (Exception ex)
        {

            return StatusCode(500, new { messagge = "Ocurrió un error inesperado al procesar su solicitud." });
        }

    }

    [HttpGet]
    [Route("Element/GetById/{id}")]

    public async Task<IActionResult> GetById(int id)
    {
        try
        {
            var elementType = await _service.GetById(id);
            return Ok(elementType);

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener la lista de proyectos");
            return BadRequest("No se pudo obtener el proyecto.");

        }

    }

    [HttpDelete]
    [Route("Element/Delete/{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        if (id <= 0)
        {
            return BadRequest("El identificador del Tipo Elemento no puede estar vacio");
        }

        var result = await _service.Delete(id);

        if (result)
        {
            return Ok(new
            {
                message = "Eliminado Correctamente",
            });
        }
        else
        {
            return StatusCode(500, new { message = "Error interno al eliminar el tipo elemento." });

        }

    }
}
