
using GoogleRuta.Services.Interfaces;
using GoogleRuta.Models;
using GoogleRuta.Data;
using Microsoft.EntityFrameworkCore;
namespace GoogleRuta.Services;

public class ConnectionTelecomService : IConnectionTelecomService
{


    private readonly ApplicationDbContext _context;
    //private readonly IOdfService _serviceOdf;

    public ConnectionTelecomService(ApplicationDbContext context)
    {
        _context = context;
        //_serviceOdf = odfService;

    }

    public async Task<ConnectionTelecom> ConnectionTelecom(ConnectionTelecom connectionTelecom)
    {

        //var odf = await _serviceOdf.GetById(connectionTelecom.OdfId);

        var conection = new ConnectionTelecom
        {
            OdlId = connectionTelecom.OdlId,
            PortOdl = connectionTelecom.PortOdl,
            ElfaId = connectionTelecom.ElfaId,
            PortElfaInput = connectionTelecom.PortElfaInput,
            PortElfaOutput = connectionTelecom.PortElfaOutput,
            OdfId = connectionTelecom.OdfId,
            PortOdf = connectionTelecom.PortOdf,
            Description = connectionTelecom.Description,
            Datetime = connectionTelecom.Datetime

        };

        _context.ConnectionTelecoms.Add(conection);
        await _context.SaveChangesAsync();
       
        return conection;

    }

}