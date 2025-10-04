using GoogleRuta.Models;
using GoogleRuta.Services;
using GoogleRuta.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GoogleRuta.Controllers
{
    public class ColorTracesController : Controller
    {
        private readonly IColorTracesService _service;
        private readonly ILogger<ColorTracesController> _logger;
        public ColorTracesController(IColorTracesService service, ILogger<ColorTracesController> logger)
        {
            _service = service;
            _logger = logger;

        }

        public async Task<IActionResult> Index()
        {
            var result = await _service.GetAll();

            return View(result);

        }

        [HttpPost]
        [Route("ColorTraces/Add")]

        public async Task<IActionResult> Add([FromBody] ColorTraces result)
        {

            try
            {
                await _service.Add(result);
                return Json(new
                {
                    success = true,
                    message = "Guardado Correctamente"
                });
            }

            catch (Exception ex)
            {
                _logger.LogError(ex, "Ocurrió un error al guardar el ColorTrace.");
                return StatusCode(500, new { success = false, message = "Ocurrió un error inesperado en el servidor." });
            }

        }


        [HttpGet]
        [Route("ColorTraces/GetById/{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var colorTrace = await _service.GetById(id);
                 //se modifico de == a is null
                if (colorTrace is null)
                {
                    return NotFound(new { success = false, message = "No se encontro" });
                }

                return Ok(colorTrace);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"error al obtener el {id}.");
                return StatusCode(500, new { success = false, message = "ocurio un error en el serviod" });

            }

        }

        [HttpDelete]
        [Route("ColorTraces/Delete/{id}")]

        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _service.Delete(id);
                return Json(new { success = true, message = "Registro eliminado correctamente." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Ocurrió un error inesperado al eliminar el color con ID {id}.");
                return StatusCode(500, new { success = false, message = "Ocurrió un error inesperado en el servidor." });
            }



        }

    }
}