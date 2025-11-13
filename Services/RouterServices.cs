using GoogleRuta.Data;
using GoogleRuta.Services.Interfaces;
using GoogleRuta.Models;
using Microsoft.EntityFrameworkCore;

namespace GoogleRuta.Services;

public class RouterServices : IRouterService
{
    private readonly ApplicationDbContext _context;
    public RouterServices(ApplicationDbContext context)
    {
        _context = context;

    }

    public async Task<Router> Add(Router router)
    {

        _context.Routers.Add(router);
        await _context.SaveChangesAsync();
        return router;


    }

    public async Task<Router> Update(Router router)
    {

        if (router.Id <= 0)
        {
            throw new ArgumentException("El ID del router no es válido.");
        }

        var existingRouter = await _context.Routers.FindAsync(router.Id);
        if (existingRouter is null)
        {

            return existingRouter;

        }

        existingRouter.Name = router.Name;
        existingRouter.IPAddress = router.IPAddress;
        existingRouter.Description = router.Description;

        await _context.SaveChangesAsync();
        return existingRouter;

    }

    public async Task<IEnumerable<Router>> GetAll()
    {
        return await _context.Routers.AsNoTracking().ToListAsync();
    }

    public async Task<Router> GetById(int id)
    {
        return await _context.Routers.FindAsync(id);
    }

    public async Task<bool> Delete(int id)
    {
        var router = await _context.Routers.FindAsync(id);
        if (router is null)
        {
            return false;
        }

        var switchPort=_context.SwitchPorts.Where(sp=>sp.RouterId==id);
       _context.SwitchPorts.RemoveRange(switchPort);
       
        _context.Routers.Remove(router);
        await _context.SaveChangesAsync();
        return true;
    }

}