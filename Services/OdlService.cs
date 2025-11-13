using GoogleRuta.Models;
using GoogleRuta.Data;
using GoogleRuta.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
namespace GoogleRuta.Services;

public class OdlService : IOdlService
{

    private readonly ApplicationDbContext _context;

    public OdlService(ApplicationDbContext applicationDbContext)
    {
        _context = applicationDbContext;

    }


    public async Task<Odl> Add(Odl odl)
    {

        _context.Odls.Add(odl);
        await _context.SaveChangesAsync();
        return odl;


    }

    public async Task<Odl> Update(Odl odl)
    {

        if (odl.Id <= 0)
        {
            throw new ArgumentException("El ID del odl no es válido.");
        }

        var existingOdl = await _context.Odls.FindAsync(odl.Id);
        if (existingOdl is null)
        {

            return existingOdl;

        }

        existingOdl.Name = odl.Name;
        existingOdl.TotalPorts = odl.TotalPorts;
        existingOdl.Description = odl.Description;
        existingOdl.ColumnCount = odl.ColumnCount;
        existingOdl.PortsPerColumn = odl.PortsPerColumn;

        await _context.SaveChangesAsync();
        return existingOdl;

    }

    public async Task<IEnumerable<Odl>> GetAll()
    {
        return await _context.Odls.AsNoTracking().ToListAsync();
    }

    public async Task<Odl> GetById(int id)
    {
        return await _context.Odls.FindAsync(id);
    }

    public async Task<bool> Delete(int id)
    {
        var odl = await _context.Odls.FindAsync(id);
        if (odl is null)
        {
            return false;
        }

        _context.Odls.Remove(odl);
        await _context.SaveChangesAsync();
        return true;
    }

}