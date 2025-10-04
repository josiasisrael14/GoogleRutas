using GoogleRuta.Data;
using GoogleRuta.Models;
using GoogleRuta.Services.Interfaces;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GoogleRuta.Controllers;

public class RouterController : Controller
{
    private readonly IRouterService _routerService;
    private readonly ApplicationDbContext _context;

    private ILogger<RouterController> _logger;
    public RouterController(IRouterService routerService, ApplicationDbContext context, ILogger<RouterController> logger)
    {
        _routerService = routerService;
        _context = context;
        _logger = logger;
    }

    public async Task<IActionResult> Index(string filtroBusqueda)
    {

        IEnumerable<Router> routers = await _routerService.GetAll();
        if (!String.IsNullOrEmpty(filtroBusqueda))
        {
            routers = routers.Where(r => r.Name.Contains(filtroBusqueda));
        }

        if (!string.IsNullOrEmpty(filtroBusqueda))
        {
            routers = routers.Where(r => r.Name.Contains(filtroBusqueda));
        }

        // La vista (View) puede manejar un IEnumerable<Router> sin ningún problema.
        return View(routers.ToList());

    }


    [HttpPost]
    [Route("Router/Add")]
    public async Task<IActionResult> Add([FromBody]Router router)
    {

        try
            {
                var result = await _routerService.Add(router);
                if (result != null)
                {
                    return Ok(new
                    { // Se usa 'new' para crear el objeto anónimo
                        success = true,
                        message = "Router agregado exitosamente",
                        router = result
                    });
                }
                else
                {

                    _logger.LogWarning("No se pudo agregar el router");
                    return Conflict(new
                    {
                        success = false,
                        message = "No se pudo agregar el router"
                    });
                }

            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex, "Error al agregar el router");
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al agregar el router");
                return StatusCode(500, new { success = false, message = "Error inesperado al agregar el router" });
            }

    }

    
    [HttpPost]
    [Route("Router/Update")]
    public async Task<IActionResult> Update([FromBody]Router router)
    {

        try
        {
            var result = await _routerService.Update(router);
            if (result != null)
            {
                return Ok(new
                { // Se usa 'new' para crear el objeto anónimo
                    success = true,
                    message = "Router actualizado exitosamente",
                    router = result
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
    [Route("Router/GetById/{id}")]
    public async Task<IActionResult> GetById(int id)
    {

        try
        {
            var result = await _routerService.GetById(id);
            if (result != null)
            {
                return Ok(new
                { // Se usa 'new' para crear el objeto anónimo
                    success = true,
                    message = "Router encontrado",
                    router = result
                });
            }
            else
            {

                _logger.LogWarning("Router no encontrado");
                return NotFound(new
                {
                    success = false,
                    message = "Router no encontrado"
                });
            }

        }
        catch (ArgumentException ex)
        {
            _logger.LogError(ex, "Error al buscar el router");
            return BadRequest(new { success = false, message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error inesperado al buscar el router");
            return StatusCode(500, new { success = false, message = "Error inesperado al buscar el router" });
        }

    }


    [HttpDelete]
    [Route("Router/Delete/{id}")]
    public async Task<IActionResult> Delete(int id)
    {

        try
        {
            var result = await _routerService.Delete(id);
            if (result)
            {
                return Ok(new
                { // Se usa 'new' para crear el objeto anónimo
                    success = true,
                    message = "Router eliminado exitosamente"
                });
            }
            else
            {

                _logger.LogWarning("No se pudo eliminar el router");
                return Ok(new
                {
                    success = false,
                    message = " el router no ha sido encontrado"
                });
            }

        }
        catch (ArgumentException ex)
        {
            _logger.LogError(ex, "Error al eliminar el router");
            return BadRequest(new { success = false, message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error inesperado al eliminar el router");
            return StatusCode(500, new { success = false, message = "Error inesperado al eliminar el router" });
        }

    }



}
