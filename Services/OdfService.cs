using Microsoft.EntityFrameworkCore;
using GoogleRuta.Data;
using GoogleRuta.Models;
using GoogleRuta.Services.Interfaces;

namespace GoogleRuta.Services;

public class OdfService : IOdfService
{

    private readonly ApplicationDbContext _context;

    public OdfService(ApplicationDbContext context)
    {

        _context = context;

    }


    public async Task<Odf> Add(Odf odf)
    {

        _context.Odfs.Add(odf);
        await _context.SaveChangesAsync();
        return odf;


    }

    public async Task<Odf> Update(Odf odf)
    {

        if (odf.Id <= 0)
        {
            throw new ArgumentException("El ID de la odf no es válido.");
        }

        var existingOdf = await _context.Odfs.FindAsync(odf.Id);
        if (existingOdf is null)
        {

            return existingOdf;

        }

        existingOdf.Name = odf.Name;
        existingOdf.TotalPorts = odf.TotalPorts;
        existingOdf.Description = odf.Description;
        existingOdf.GroupCount = odf.GroupCount;
        existingOdf.PortsPerGroup = odf.PortsPerGroup;

        await _context.SaveChangesAsync();
        return existingOdf;

    }

    public async Task<IEnumerable<Odf>> GetAll()
    {
        return await _context.Odfs.AsNoTracking().ToListAsync();
    }

    public async Task<Odf> GetById(int id)
    {
        return await _context.Odfs.FindAsync(id);
    }

    public async Task<bool> Delete(int id)
    {
        var odf = await _context.Odfs.FindAsync(id);
        if (odf is null)
        {
            return false;
        }

        _context.Odfs.Remove(odf);
        await _context.SaveChangesAsync();
        return true;
    }

}