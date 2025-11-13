using GoogleRuta.Models;
using GoogleRuta.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GoogleRuta.Controllers;

public class OdfController : Controller
{

    private readonly IOdfService _service;
    private ILogger<OdfController> _logger;

    public OdfController(IOdfService service, ILogger<OdfController> logger)
    {

        _service = service;
        _logger = logger;

    }

    public async Task<IActionResult> Index(string filtroBusqueda)
    {

        IEnumerable<Odf> odf = await _service.GetAll();
        if (!String.IsNullOrEmpty(filtroBusqueda))
        {
            odf = odf.Where(r => r.Name.Contains(filtroBusqueda));
        }

        if (!string.IsNullOrEmpty(filtroBusqueda))
        {
            odf = odf.Where(r => r.Name.Contains(filtroBusqueda));
        }

        // La vista (View) puede manejar un IEnumerable<Router> sin ningún problema.
        return View(odf.ToList());

    }


    [HttpPost]
    [Route("Odf/Add")]
    public async Task<IActionResult> Add([FromBody] Odf odf)
    {

        var total = odf.TotalPorts;
        var grupo = odf.GroupCount;
        var puerto = odf.PortsPerGroup;

        if (total != (grupo * puerto))
        {

            return Ok(new
            {
                success = false,
                message = "El calculo entre puertos y grupo no coinciden con el total"

            });

        }


        try
        {
            var result = await _service.Add(odf);
            if (result != null)
            {
                return Ok(new
                { // Se usa 'new' para crear el objeto anónimo
                    success = true,
                    message = "Odf agregado exitosamente",
                    odf = result
                });
            }
            else
            {

                _logger.LogWarning("No se pudo agregar el odf");
                return Conflict(new
                {
                    success = false,
                    message = "No se pudo agregar el odf"
                });
            }

        }
        catch (ArgumentException ex)
        {
            _logger.LogError(ex, "Error al agregar el odf");
            return BadRequest(new { success = false, message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error inesperado al agregar el odf");
            return StatusCode(500, new { success = false, message = "Error inesperado al agregar el odf" });
        }

    }


    [HttpPost]
    [Route("Odf/Update")]
    public async Task<IActionResult> Update([FromBody] Odf odf)
    {

        try
        {
            var result = await _service.Update(odf);
            if (result != null)
            {
                return Ok(new
                { // Se usa 'new' para crear el objeto anónimo
                    success = true,
                    message = "Odf actualizado exitosamente",
                    odf = result
                });
            }
            else
            {

                _logger.LogWarning("No se pudo actualizar el odf");
                return Conflict(new
                {
                    success = false,
                    message = "No se pudo actualizar el odf"
                });
            }

        }
        catch (ArgumentException ex)
        {
            _logger.LogError(ex, "Error al actualizar el odf");
            return BadRequest(new { success = false, message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error inesperado al actualizar el odf");
            return StatusCode(500, new { success = false, message = "Error inesperado al actualizar el odf" });
        }

    }

    [HttpGet]
    [Route("Odf/GetById/{id}")]
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
                    message = "Odf encontrado",
                    odf = result
                });
            }
            else
            {

                _logger.LogWarning("Odf no encontrado");
                return NotFound(new
                {
                    success = false,
                    message = "Odf no encontrado"
                });
            }

        }
        catch (ArgumentException ex)
        {
            _logger.LogError(ex, "Error al buscar el odf");
            return BadRequest(new { success = false, message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error inesperado al buscar el odf");
            return StatusCode(500, new { success = false, message = "Error inesperado al buscar el odf" });
        }

    }


    [HttpDelete]
    [Route("Odf/Delete/{id}")]
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
                    message = "Odf eliminado exitosamente"
                });
            }
            else
            {

                _logger.LogWarning("No se pudo eliminar el odf");
                return Ok(new
                {
                    success = false,
                    message = " el odf no ha sido encontrado"
                });
            }

        }
        catch (ArgumentException ex)
        {
            _logger.LogError(ex, "Error al eliminar el odf");
            return BadRequest(new { success = false, message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error inesperado al eliminar el odf");
            return StatusCode(500, new { success = false, message = "Error inesperado al eliminar el odf" });
        }

    }

}