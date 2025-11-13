using GoogleRuta.Models;
using GoogleRuta.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GoogleRuta.Controllers;

public class SwitchController : Controller
{

    private readonly ISwitchService _switchService;
    private ILogger<SwitchController> _logger;
    public SwitchController(ISwitchService switchService, ILogger<SwitchController> logger)
    {
        _switchService = switchService;
        _logger = logger;
    }

    public async Task<IActionResult> Index(string filtroBusqueda)
    {

        IEnumerable<Switchs> switchs = await _switchService.GetAll();
        if (!String.IsNullOrEmpty(filtroBusqueda))
        {
            switchs = switchs.Where(r => r.Name.Contains(filtroBusqueda));
        }

        if (!string.IsNullOrEmpty(filtroBusqueda))
        {
            switchs = switchs.Where(r => r.Name.Contains(filtroBusqueda));
        }

        // La vista (View) puede manejar un IEnumerable<Router> sin ningún problema.
        return View(switchs.ToList());

    }

    [HttpPost]
    [Route("Switch/Add")]
    public async Task<IActionResult> Add([FromBody] Switchs switchs)
    {

        try
        {
            var result = await _switchService.Add(switchs);
            if (result != null)
            {
                return Ok(new
                { // Se usa 'new' para crear el objeto anónimo
                    success = true,
                    message = "Switch agregado exitosamente",
                    switchs = result
                });
            }
            else
            {

                _logger.LogWarning("No se pudo agregar el switch");
                return Conflict(new
                {
                    success = false,
                    message = "No se pudo agregar el switch"
                });
            }

        }
        catch (ArgumentException ex)
        {
            _logger.LogError(ex, "Error al agregar el switch");
            return BadRequest(new { success = false, message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error inesperado al agregar el switch");
            return StatusCode(500, new { success = false, message = "Error inesperado al agregar el switch" });
        }

    }


    [HttpPost]
    [Route("Switch/Update")]
    public async Task<IActionResult> Update([FromBody] Switchs switchs)
    {

        try
        {
            var result = await _switchService.Update(switchs);
            if (result != null)
            {
                return Ok(new
                { // Se usa 'new' para crear el objeto anónimo
                    success = true,
                    message = "Switch actualizado exitosamente",
                    switchs = result
                });
            }
            else
            {

                _logger.LogWarning("No se pudo actualizar el router");
                return Conflict(new
                {
                    success = false,
                    message = "No se pudo actualizar el router"
                });
            }

        }
        catch (ArgumentException ex)
        {
            _logger.LogError(ex, "Error al actualizar el router");
            return BadRequest(new { success = false, message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error inesperado al actualizar el router");
            return StatusCode(500, new { success = false, message = "Error inesperado al actualizar el router" });
        }

    }


    [HttpGet]
    [Route("Switch/GetById/{id}")]
    public async Task<IActionResult> GetById(int id)
    {

        try
        {
            var result = await _switchService.GetById(id);
            if (result != null)
            {
                return Ok(new
                { // Se usa 'new' para crear el objeto anónimo
                    success = true,
                    message = "Switch encontrado",
                    switchs = result
                });
            }
            else
            {

                _logger.LogWarning("Switch no encontrado");
                return NotFound(new
                {
                    success = false,
                    message = "Switch no encontrado"
                });
            }

        }
        catch (ArgumentException ex)
        {
            _logger.LogError(ex, "Error al buscar el Switch");
            return BadRequest(new { success = false, message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error inesperado al buscar el Switch");
            return StatusCode(500, new { success = false, message = "Error inesperado al buscar el Switch" });
        }

    }


    [HttpDelete]
    [Route("Switch/Delete/{id}")]
    public async Task<IActionResult> Delete(int id)
    {

        try
        {
            var result = await _switchService.Delete(id);
            if (result)
            {
                return Ok(new
                { // Se usa 'new' para crear el objeto anónimo
                    success = true,
                    message = "Switch eliminado exitosamente"
                });
            }
            else
            {

                _logger.LogWarning("No se pudo eliminar el Switch");
                return Ok(new
                {
                    success = false,
                    message = " el Switch no ha sido encontrado"
                });
            }

        }
        catch (ArgumentException ex)
        {
            _logger.LogError(ex, "Error al eliminar el Switch");
            return BadRequest(new { success = false, message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error inesperado al eliminar el Switch");
            return StatusCode(500, new { success = false, message = "Error inesperado al eliminar el Switch" });
        }

    }

}


