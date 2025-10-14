using GoogleRuta.Models;
using GoogleRuta.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GoogleRuta.Controllers;

public class OdlController : Controller
{

    private readonly IOdlService _service;
    private ILogger<OdlController> _logger;

    public OdlController(IOdlService service, ILogger<OdlController> logger)
    {

        _service = service;
        _logger = logger;

    }

    public async Task<IActionResult> Index(string filtroBusqueda)
    {

        IEnumerable<Odl> odl = await _service.GetAll();
        if (!String.IsNullOrEmpty(filtroBusqueda))
        {
            odl = odl.Where(r => r.Name.Contains(filtroBusqueda));
        }

        if (!string.IsNullOrEmpty(filtroBusqueda))
        {
            odl = odl.Where(r => r.Name.Contains(filtroBusqueda));
        }

        // La vista (View) puede manejar un IEnumerable<Router> sin ningún problema.
        return View(odl.ToList());

    }


    [HttpPost]
    [Route("Odl/Add")]
    public async Task<IActionResult> Add([FromBody] Odl odl)
    {

        try
        {
            var result = await _service.Add(odl);
            if (result != null)
            {
                return Ok(new
                { // Se usa 'new' para crear el objeto anónimo
                    success = true,
                    message = "Odl agregado exitosamente",
                    odl = result
                });
            }
            else
            {

                _logger.LogWarning("No se pudo agregar el odl");
                return Conflict(new
                {
                    success = false,
                    message = "No se pudo agregar el odl"
                });
            }

        }
        catch (ArgumentException ex)
        {
            _logger.LogError(ex, "Error al agregar el odl");
            return BadRequest(new { success = false, message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error inesperado al agregar el odl");
            return StatusCode(500, new { success = false, message = "Error inesperado al agregar el odl" });
        }

    }


    [HttpPost]
    [Route("Odl/Update")]
    public async Task<IActionResult> Update([FromBody] Odl odl)
    {

        try
        {
            var result = await _service.Update(odl);
            if (result != null)
            {
                return Ok(new
                { // Se usa 'new' para crear el objeto anónimo
                    success = true,
                    message = "Odl actualizado exitosamente",
                    odl = result
                });
            }
            else
            {

                _logger.LogWarning("No se pudo actualizar el odl");
                return Conflict(new
                {
                    success = false,
                    message = "No se pudo actualizar el odl"
                });
            }

        }
        catch (ArgumentException ex)
        {
            _logger.LogError(ex, "Error al actualizar el odl");
            return BadRequest(new { success = false, message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error inesperado al actualizar el odl");
            return StatusCode(500, new { success = false, message = "Error inesperado al actualizar el odl" });
        }

    }

    [HttpGet]
    [Route("Odl/GetById/{id}")]
    public async Task<IActionResult> GetById(int id)
    {

        try
        {
            var result = await _service.GetById(id);
            if (result != null)
            {
                return Ok(new
                { // Se usa 'new' para crear el objeto anónimo
                    success = true,
                    message = "Odl encontrado",
                    odl = result
                });
            }
            else
            {

                _logger.LogWarning("Odl no encontrado");
                return NotFound(new
                {
                    success = false,
                    message = "Odl no encontrado"
                });
            }

        }
        catch (ArgumentException ex)
        {
            _logger.LogError(ex, "Error al buscar el odl");
            return BadRequest(new { success = false, message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error inesperado al buscar el odl");
            return StatusCode(500, new { success = false, message = "Error inesperado al buscar el odl" });
        }

    }


    [HttpDelete]
    [Route("Odl/Delete/{id}")]
    public async Task<IActionResult> Delete(int id)
    {

        try
        {
            var result = await _service.Delete(id);
            if (result)
            {
                return Ok(new
                { // Se usa 'new' para crear el objeto anónimo
                    success = true,
                    message = "Odl eliminado exitosamente"
                });
            }
            else
            {

                _logger.LogWarning("No se pudo eliminar el odl");
                return Ok(new
                {
                    success = false,
                    message = " el odl no ha sido encontrado"
                });
            }

        }
        catch (ArgumentException ex)
        {
            _logger.LogError(ex, "Error al eliminar el odl");
            return BadRequest(new { success = false, message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error inesperado al eliminar el odl");
            return StatusCode(500, new { success = false, message = "Error inesperado al eliminar el odl" });
        }

    }

}