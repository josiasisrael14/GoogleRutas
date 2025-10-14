using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GoogleRuta.Models;
using GoogleRuta.Services;
using GoogleRuta.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GoogleRuta.Controllers;

public class ElfaController : Controller
{

    private readonly IElfaService _service;

    private ILogger<ElfaController> _logger;

    public ElfaController(IElfaService service, ILogger<ElfaController> logger)
    {

        _logger = logger;
        _service = service;

    }


    public async Task<IActionResult> Index(string filtroBusqueda)
    {

        IEnumerable<Elfa> elfa = await _service.GetAll();
        if (!String.IsNullOrEmpty(filtroBusqueda))
        {
            elfa = elfa.Where(r => r.Name.Contains(filtroBusqueda));
        }

        if (!string.IsNullOrEmpty(filtroBusqueda))
        {
            elfa = elfa.Where(r => r.Name.Contains(filtroBusqueda));
        }

        // La vista (View) puede manejar un IEnumerable<Router> sin ningún problema.
        return View(elfa.ToList());

    }


    [HttpPost]
    [Route("Elfa/Add")]
    public async Task<IActionResult> Add([FromBody] Elfa elfa)
    {

        try
        {
            var total = elfa.TotalPorts;
            var grupo = elfa.GroupCount;
            var puerto = elfa.PortsPerGroup;

            if (total != (grupo * puerto))
            {

                return Ok(new
                {
                    success = false,
                    message = "El calculo entre puertos y grupo no coinciden con el total"

                });

            }


            var result = await _service.Add(elfa);
            if (result != null)
            {
                return Ok(new
                { // Se usa 'new' para crear el objeto anónimo
                    success = true,
                    message = "elfa agregado exitosamente",
                    elfa = result
                });
            }
            else
            {

                _logger.LogWarning("No se pudo agregar el elfa");
                return Conflict(new
                {
                    success = false,
                    message = "No se pudo agregar el elfa"
                });
            }

        }
        catch (ArgumentException ex)
        {
            _logger.LogError(ex, "Error al agregar el elfa");
            return BadRequest(new { success = false, message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error inesperado al agregar el elfa");
            return StatusCode(500, new { success = false, message = "Error inesperado al agregar el elfa" });
        }

    }


    [HttpPost]
    [Route("Elfa/Update")]
    public async Task<IActionResult> Update([FromBody] Elfa elfa)
    {

        try
        {
            var result = await _service.Update(elfa);
            if (result != null)
            {
                return Ok(new
                { // Se usa 'new' para crear el objeto anónimo
                    success = true,
                    message = "elfa actualizado exitosamente",
                    elfa = result
                });
            }
            else
            {

                _logger.LogWarning("No se pudo actualizar el elfa");
                return Conflict(new
                {
                    success = false,
                    message = "No se pudo actualizar el elfa"
                });
            }

        }
        catch (ArgumentException ex)
        {
            _logger.LogError(ex, "Error al actualizar el elfa");
            return BadRequest(new { success = false, message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error inesperado al actualizar el elfa");
            return StatusCode(500, new { success = false, message = "Error inesperado al actualizar el elfa" });
        }

    }

    [HttpGet]
    [Route("Elfa/GetById/{id}")]
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
                    message = "elfa encontrado",
                    elfa = result
                });
            }
            else
            {

                _logger.LogWarning("elfa no encontrado");
                return NotFound(new
                {
                    success = false,
                    message = "elfa no encontrado"
                });
            }

        }
        catch (ArgumentException ex)
        {
            _logger.LogError(ex, "Error al buscar el elfa");
            return BadRequest(new { success = false, message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error inesperado al buscar el elfa");
            return StatusCode(500, new { success = false, message = "Error inesperado al buscar el elfa" });
        }

    }


    [HttpDelete]
    [Route("Elfa/Delete/{id}")]
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
                    message = "elfa eliminado exitosamente"
                });
            }
            else
            {

                _logger.LogWarning("No se pudo eliminar el elfa");
                return Ok(new
                {
                    success = false,
                    message = " el elfa no ha sido encontrado"
                });
            }

        }
        catch (ArgumentException ex)
        {
            _logger.LogError(ex, "Error al eliminar el elfa");
            return BadRequest(new { success = false, message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error inesperado al eliminar el elfa");
            return StatusCode(500, new { success = false, message = "Error inesperado al eliminar el elfa" });
        }

    }

}