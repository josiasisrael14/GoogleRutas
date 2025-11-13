using GoogleRuta.Data;
using Microsoft.AspNetCore.Mvc;
using GoogleRuta.Services.Interfaces;
using GoogleRuta.Views.ViewModel;
using GoogleRuta.Dtos;
using GoogleRuta.Models;
using Microsoft.EntityFrameworkCore;
using GoogleRuta.ViewModels;

namespace GoogleRuta.Controllers;

public class DeviceConnection : Controller
{
    private readonly ILogger<DeviceConnection> _logger;
    private readonly IRouterService _routerService;
    private readonly ISwitchService _switchService;
    private readonly IOdlService _odlService;
    private readonly IOdfService _odfService;
    private readonly IElfaService _elfaService;
    private readonly ApplicationDbContext _context;

    public DeviceConnection(ILogger<DeviceConnection> logger, IRouterService routerService, ISwitchService switchService, IOdlService odlService, IOdfService odfService, IElfaService elfaService, ApplicationDbContext context)
    {
        _logger = logger;
        _routerService = routerService;
        _switchService = switchService;
        _odlService = odlService;
        _odfService = odfService;
        _elfaService = elfaService;
        _context = context;

    }

    // public async Task<IActionResult> Index()
    // {
    //     var savedDiagram = await _context.Diagrams.FirstOrDefaultAsync();
    //     // 1. Creamos el ViewModel
    //     var viewModel = new DiagramaViewModel
    //     {
    //         // 2. Obtenemos todos los routers y switches de los servicios
    //         Routers = await _routerService.GetAll(),
    //         Switches = await _switchService.GetAll(),
    //         DiagramJson = savedDiagram?.JsonContent ?? "[]"
    //     };

    //     // 3. Pasamos el ViewModel (con los datos adentro) a la vista
    //     return View(viewModel);
    // }

    public async Task<IActionResult> Index()
    {
        var savedDiagram = await _context.Diagrams.FirstOrDefaultAsync();

        // --- AÑADE ESTO: Obtener las conexiones ---
        var connections = await _context.SwitchPorts
            .AsNoTracking()
            .Select(sp => new ConnectionEquipmentDto
            {
                Id = sp.Id,
                SwitchId = sp.Switchs.Id,
                SwitchName = sp.Switchs.Name,
                PortNumber = sp.PortNumber,
                GroupNumber = sp.GroupNumber,
                RouterName = sp.Router != null ? sp.Router.Name : "No conectado",
                RouterId = sp.RouterId
            })
            .ToListAsync();

        var telecomConnections = await _context.ConnectionTelecoms.AsNoTracking().ToListAsync();

        var switchesFromDb = await _switchService.GetAll();
        var routersFromDb = await _routerService.GetAll();
        var odlFromDb = await _odlService.GetAll();
        var odfFrom = await _odfService.GetAll();
        var elfaFrom = await _elfaService.GetAll();
        var viewModel = new DiagramaViewModel
        {
            Routers = routersFromDb,
            Switches = switchesFromDb,
            Odls = odlFromDb,
            Odfs = odfFrom,
            Elfas = elfaFrom,
            DiagramJson = savedDiagram?.JsonContent ?? "{}",
            Connections = connections,
            TelecomConnections = telecomConnections// <-- Pasa las conexiones a la vista
        };

        return View(viewModel);
    }


    public async Task<IActionResult> EquipmentConnection()
    {
        var allRouters = await _routerService.GetAll();
        var allSwitches = await _switchService.GetAll();

        // --- CAMBIO AQUÍ ---
        // Ahora, el .Select() crea una instancia de nuestra nueva clase ConnectionDto
        var existingConnections = await _context.SwitchPorts
            .AsNoTracking()
            .Select(sp => new ConnectionEquipmentDto // <--- Usamos la clase DTO
            {
                Id = sp.Id,
                SwitchId = sp.Switchs.Id,
                SwitchName = sp.Switchs.Name,
                PortNumber = sp.PortNumber,
                GroupNumber = sp.GroupNumber,
                RouterName = sp.Router != null ? sp.Router.Name : "No conectado",
                RouterId = sp.RouterId
            })
            .ToListAsync();

        var viewModel = new EquipmentConnectionViewModel
        {
            Connections = existingConnections,
            AllRouters = allRouters,
            AllSwitches = allSwitches
        };

        return View("EquipmentConnection", viewModel);
    }


    [HttpDelete]
    [Route("DeviceConnection/DeleteConnection/{id}")]
    public async Task<IActionResult> DeleteConnection(int id)
    {
        var switchPort = await _context.SwitchPorts.FindAsync(id);
        if (switchPort is null)
        {
            return NotFound(new { success = false, message = "La conexión especificada no existe." });
        }

        _context.SwitchPorts.Remove(switchPort);
        await _context.SaveChangesAsync();
        return Ok(new { success = true, message = "Conexión eliminada exitosamente." });
    }

    [HttpPost]
    [Route("DeviceConnection/SaveDiagram")]
    public async Task<IActionResult> SaveDiagram([FromBody] SaveDiagramDto dto)
    {
        if (dto is null || string.IsNullOrWhiteSpace(dto.JsonContent))
        {
            return BadRequest(new { success = false, message = "El contenido del diagrama no puede estar vacío." });
        }

        var existingDiagram = await _context.Diagrams.FirstOrDefaultAsync();
        if (existingDiagram != null)
        {
            existingDiagram.JsonContent = dto.JsonContent;
        }

        else
        {
            var newDiagram = new Diagram
            {
                Name = "Diagrama Principal",
                JsonContent = dto.JsonContent
            };

            _context.Diagrams.Add(newDiagram);
        }

        await _context.SaveChangesAsync();
        return Ok(new { success = true, message = "Diagrama guardado exitosamente." });

    }

    [HttpPost]
    [Route("DeviceConnection/ConnectDevices")]
    public async Task<IActionResult> ConnectDevices([FromBody] SwitchPort switchPort)
    {
        if (switchPort is null || switchPort.GroupNumber <= 0 || switchPort.PortNumber <= 0 || switchPort.RouterId <= 0)
        {
            return BadRequest(new { success = false, message = "Datos inválidos para conectar dispositivos." });
        }

        if (switchPort.Id > 0)
        {
            var result = await _context.SwitchPorts.FindAsync(switchPort.Id);
            if (result is null)
            {
                return NotFound(new { success = false, message = "El puerto especificado no existe." });
            }

            result.RouterId = switchPort.RouterId;

            await _context.SaveChangesAsync();
            return Ok(new { success = true, message = "Conexión actualizada exitosamente." });
        }

        else
        {
            var router = await _routerService.GetById(switchPort.RouterId);
            var switchDevice = await _switchService.GetById(switchPort.SwitchsId);

            if (router is null || switchDevice is null)
            {
                return NotFound(new { success = false, message = "Router o Switch no encontrado." });
            }

            var portExists = await _context.SwitchPorts.AnyAsync(sp => sp.SwitchsId == switchPort.SwitchsId && sp.GroupNumber == switchPort.GroupNumber && sp.PortNumber == switchPort.PortNumber);
            if (portExists)
            {
                return Conflict(new { success = false, message = "El puerto ya está ocupado." });
            }
            _context.SwitchPorts.Add(switchPort);
            await _context.SaveChangesAsync();

            return Ok(new { success = true, message = $"Router {router.Name} conectado al Switch {switchDevice.Name} en el puerto {switchPort.PortNumber}." });

        }


    }

    //Codigo comentado para posible uso futuro
    // [HttpGet]
    // [Route("DeviceConnection/GetConnections")]
    // public async Task<IActionResult> GetConnections()
    // {
    //     var connections = await _context.SwitchPorts
    //         .Include(sp => sp.Router)
    //         .Include(sp => sp.Switchs)
    //         .AsNoTracking()
    //         .ToListAsync();

    //     var connectionDtos = connections.Select(sp => new
    //     {
    //         sp.Id,
    //         SwitchId = sp.Switchs.Id,
    //         SwitchName = sp.Switchs.Name,
    //         sp.PortNumber,
    //         sp.GroupNumber,
    //         RouterName = sp.Router != null ? sp.Router.Name : "No conectado",
    //         RouterId = sp.RouterId
    //     });

    //     return Ok(connectionDtos);
    // }

    [HttpGet]
    [Route("DeviceConnection/GetConnections")]
    public async Task<IActionResult> GetConnections()
    {
        var connectionDtos = await _context.SwitchPorts
            .AsNoTracking() // AsNoTracking es útil aquí también
            .Select(sp => new // Proyectamos directamente a nuestro objeto de respuesta
            {
                sp.Id,
                SwitchId = sp.Switchs.Id,
                SwitchName = sp.Switchs.Name,
                sp.PortNumber,
                sp.GroupNumber,
                RouterName = sp.Router != null ? sp.Router.Name : "No conectado",
                RouterId = sp.RouterId
            })
            .ToListAsync();

        // Ya no necesitamos el .Include() porque el .Select() le dice a EF qué tablas necesita unir.
        // EF es lo suficientemente inteligente como para generar los JOINs necesarios.

        return Ok(connectionDtos);
    }

}
