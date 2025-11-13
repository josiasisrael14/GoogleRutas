using Microsoft.AspNetCore.Mvc;
using GoogleRuta.Services.Interfaces;
using GoogleRuta.Models;
using Microsoft.AspNetCore.Components.Forms;
using GoogleRuta.Data;
using Microsoft.EntityFrameworkCore;
using System.Runtime.InteropServices;
using GoogleRuta.Views.ViewModel;

namespace GoogleRuta.Controllers
{
    public class ConnectionTelecomController : Controller
    {

        private readonly IConnectionTelecomService _serviceConectionTelecom;
        private readonly ApplicationDbContext _context;
        private readonly IOdfService _odfService;
        private readonly IOdlService _odlService;
        private readonly IElfaService _elfaService;
        private ILogger<ConnectionTelecomController> _logger;

        public ConnectionTelecomController(IConnectionTelecomService serviceConectionTelecom, ApplicationDbContext context, ILogger<ConnectionTelecomController> logger, IOdfService odfService,
        IOdlService odlService, IElfaService elfaService
        )
        {
            _serviceConectionTelecom = serviceConectionTelecom;
            _context = context;
            _logger = logger;
            _odfService = odfService;
            _odlService = odlService;
            _elfaService = elfaService;

        }



        // [HttpPost]
        // [Route("ConnectionTelecom")]
        // public async Task<IActionResult> AddConnection([FromBody] ConnectionTelecom connectionTelecom)
        // {

        //     try
        //     {

        //         var elfa = await _elfaService.GetById(connectionTelecom.ElfaId);
        //         var odf = await _odfService.GetById(connectionTelecom.OdfId);
        //         var odt = await _odlService.GetById(connectionTelecom.OdlId);

        //         if (elfa is null || odf is null || odt is null)
        //         {

        //             return Ok(new
        //             {

        //                 success = false,
        //                 message = "verificar si elfa , odf , odt existen"

        //             });

        //         }

        //         var verificarPuertoExistent = await _context.ConnectionTelecoms.AnyAsync(p => p.OdlId == odt.Id && p.PortOdl == odt.PortsPerColumn);

        //         if (verificarPuertoExistent)
        //         {
        //             return Ok(new
        //             {
        //                 success = false,
        //                 message = "Puerto de esta odl ya esta ocupado "
        //             });
        //         }

        //         var verificarOdf = await _context.ConnectionTelecoms.AnyAsync(p => (p.OdlId == odt.Id && p.PortOdl == odt.PortsPerColumn) || (p.OdfId == odf.Id && p.PortOdf == odf.PortsPerGroup));

        //         if (verificarOdf)
        //         {
        //             return Ok(new
        //             {

        //                 success = false,
        //                 message = "Odf ya esta conectado a ese puerto de la odt"


        //             });

        //         }

        //         //var verificarEdfa = await _context.ConnectionTelecoms.AnyAsync(p => (p.OdlId == odt.Id && p.PortOdl == odt.PortsPerColumn) || (p.ElfaId==elfa.Id && p.PortElfaInput==elfa.));

        //         var result = await _serviceConectionTelecom.ConnectionTelecom(connectionTelecom);
        //         if (result is null)
        //         {

        //             return BadRequest();

        //         }

        //         return Ok(new
        //         {
        //             message = "Connecion realizada correctamente",
        //             success = true,
        //             data = result

        //         });



        //     }
        //     catch (Exception ex)
        //     {
        //         _logger.LogError("error al realizar la conexion", ex);

        //         return StatusCode(500, new
        //         {
        //             success = false,
        //             message = "Error interno del servidor al procesar la conexión."
        //         });




        //     }





        // }

        public async Task<IActionResult> Index()
        {
            var edfa = await _elfaService.GetAll();
            var odf = await _odfService.GetAll();
            var odl = await _odlService.GetAll();

            var conneciotns = await _context.ConnectionTelecoms
                               .Include(c => c.Odl)
                               .Include(c => c.Elfa)
                               .Include(c => c.Odf)
                               .AsNoTracking()
                               .ToListAsync();

            var telecom = new TelecomViewModel
            {
                Elfas = edfa,
                Odfs = odf,
                Odls = odl,
                ExistingConnections = conneciotns

            };

            return View(telecom);


        }

        [HttpPost]
        [Route("ConnectionTelecom")]
        public async Task<IActionResult> AddConnection([FromBody] ConnectionTelecom connectionTelecom)
        {
            try
            {
                // ===============================================================
                // PASO 1: VALIDAR QUE LOS EQUIPOS EXISTEN
                // ===============================================================
                var elfa = await _elfaService.GetById(connectionTelecom.ElfaId);
                var odf = await _odfService.GetById(connectionTelecom.OdfId);
                var odl = await _odlService.GetById(connectionTelecom.OdlId);

                if (elfa is null || odf is null || odl is null)
                {
                    return Ok(new
                    {
                        success = false,
                        message = "Uno de los equipos seleccionados (ODL, Elfa u ODF) no existe."
                    });
                }

                // ===============================================================
                // PASO 2: VALIDAR QUE LOS NÚMEROS DE PUERTO ESTÉN DENTRO DEL RANGO VÁLIDO
                // ===============================================================
                if (connectionTelecom.PortOdl <= 0 || connectionTelecom.PortOdl > odl.TotalPorts ||
                    connectionTelecom.PortOdf <= 0 || connectionTelecom.PortOdf > odf.TotalPorts ||
                    connectionTelecom.PortElfaInput <= 0 || connectionTelecom.PortElfaInput > elfa.TotalPorts ||
                    connectionTelecom.PortElfaOutput <= 0 || connectionTelecom.PortElfaOutput > elfa.TotalPorts)
                {
                    return Ok(new
                    {
                        success = false,
                        message = "El número de puerto seleccionado está fuera del rango válido para uno de los equipos."
                    });
                }

                // ===============================================================
                // PASO 3: VALIDAR LA LÓGICA DÚPLEX SI CORRESPONDE
                // ===============================================================
                if (elfa.IsDuplex)
                {
                    // Esta lógica solo tiene sentido si hay exactamente 2 grupos.
                    if (elfa.GroupCount != 2)
                    {
                        return Ok(new { success = false, message = "La lógica Dúplex de correspondencia vertical solo soporta Elfas con 2 grupos." });
                    }

                    int inputPort = connectionTelecom.PortElfaInput;
                    int outputPort = connectionTelecom.PortElfaOutput;

                    // La regla es que la diferencia entre los puertos absolutos debe ser igual al tamaño de un grupo.
                    if (Math.Abs(inputPort - outputPort) != elfa.PortsPerGroup)
                    {
                        return Ok(new
                        {
                            success = false,
                            message = "Para un Elfa Dúplex, los puertos de entrada y salida no son correspondientes entre los grupos."
                        });
                    }
                }

                // ===============================================================
                // PASO 4: VALIDAR QUE NINGUNO DE LOS PUERTOS ESTÉ YA OCUPADO
                // (Esta única consulta reemplaza todas tus validaciones 'verificar...' anteriores)
                // ===============================================================
                var algunPuertoOcupado = await _context.ConnectionTelecoms.AnyAsync(p =>
                    (p.OdlId == connectionTelecom.OdlId && p.PortOdl == connectionTelecom.PortOdl) ||
                    (p.OdfId == connectionTelecom.OdfId && p.PortOdf == connectionTelecom.PortOdf) ||
                    (p.ElfaId == connectionTelecom.ElfaId && (p.PortElfaInput == connectionTelecom.PortElfaInput || p.PortElfaOutput == connectionTelecom.PortElfaInput)) ||
                    (p.ElfaId == connectionTelecom.ElfaId && (p.PortElfaInput == connectionTelecom.PortElfaOutput || p.PortElfaOutput == connectionTelecom.PortElfaOutput))
                );

                if (algunPuertoOcupado)
                {
                    return Ok(new
                    {
                        success = false,
                        message = "Uno o más de los puertos seleccionados ya están ocupados en otra conexión."
                    });
                }

                // ===============================================================
                // SI TODAS LAS VALIDACIONES PASAN, CREAR LA CONEXIÓN
                // ===============================================================
                var result = await _serviceConectionTelecom.ConnectionTelecom(connectionTelecom);
                if (result is null)
                {
                    // Si el servicio por alguna razón no pudo crear el objeto
                    return BadRequest(new { success = false, message = "No se pudo guardar el registro de la conexión." });
                }

                return Ok(new
                {
                    message = "Conexión realizada correctamente",
                    success = true,
                    //data = result
                });
            }
            catch (Exception ex)
            {
                _logger.LogError("Error al realizar la conexión", ex);
                return StatusCode(500, new
                {
                    success = false,
                    message = "Error interno del servidor al procesar la conexión."
                });
            }
        }


        [HttpDelete]
        [Route("ConnectionTelecom/DeleteConnection/{id}")]
        public async Task<IActionResult> DeleteConnection(int id)
        {
            try
            {
                // Busca la conexión por su ID.
                var connectionTelecom = await _context.ConnectionTelecoms.FindAsync(id);

                // Si no se encuentra, devuelve un error 404 Not Found.
                // Esto es correcto y es una buena práctica.
                if (connectionTelecom is null)
                {
                    return NotFound(new { success = false, message = "La conexión especificada no existe." });
                }

                // Si se encuentra, la elimina y guarda los cambios.
                _context.ConnectionTelecoms.Remove(connectionTelecom);
                await _context.SaveChangesAsync();

                // Devuelve una respuesta 200 OK de éxito.
                return Ok(new { success = true, message = "Conexión eliminada exitosamente." });
            }
            catch (Exception ex)
            {
                // ✅ CAMBIO: Esta es la sección corregida.

                // 1. Registra el error detallado para los desarrolladores.
                _logger.LogError(ex, "Ocurrió un error al intentar eliminar la conexión con ID: {ConnectionId}", id);

                // 2. Devuelve un error 500 Internal Server Error con un mensaje genérico para el usuario.
                return StatusCode(500, new
                {
                    success = false,
                    message = "Ocurrió un error inesperado en el servidor. Por favor, intente de nuevo más tarde."
                });
            }
        }

    }
}