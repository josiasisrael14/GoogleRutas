using GoogleRuta.Models;
using GoogleRuta.Data;
using GoogleRuta.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GoogleRuta.Services;

public class ElfaService : IElfaService
{

    private readonly ApplicationDbContext _context;

    public ElfaService(ApplicationDbContext context)
    {
        _context = context;

    }

    public async Task<Elfa> Add(Elfa elfa)
    {
        elfa.IsDuplex = true;
        _context.Elfas.Add(elfa);
        await _context.SaveChangesAsync();
        return elfa;


    }

    public async Task<Elfa> Update(Elfa elfa)
    {

        if (elfa.Id <= 0)
        {
            throw new ArgumentException("El ID de la elfa no es válido.");
        }

        var existingElfa = await _context.Elfas.FindAsync(elfa.Id);
        if (existingElfa is null)
        {

            return existingElfa;

        }

        existingElfa.Name = elfa.Name;
        existingElfa.TotalPorts = elfa.TotalPorts;
        existingElfa.Description = elfa.Description;
        existingElfa.GroupCount = elfa.GroupCount;
        existingElfa.PortsPerGroup = elfa.PortsPerGroup;
        existingElfa.IsDuplex = elfa.IsDuplex;

        await _context.SaveChangesAsync();
        return existingElfa;

    }

    public async Task<IEnumerable<Elfa>> GetAll()
    {
        return await _context.Elfas.AsNoTracking().ToListAsync();
    }

    public async Task<Elfa> GetById(int id)
    {
        return await _context.Elfas.FindAsync(id);
    }

    public async Task<bool> Delete(int id)
    {
        var elfa = await _context.Elfas.FindAsync(id);
        if (elfa is null)
        {
            return false;
        }

        _context.Elfas.Remove(elfa);
        await _context.SaveChangesAsync();
        return true;
    }

}