using GoogleRuta.Data;
using GoogleRuta.Models;
using GoogleRuta.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
namespace GoogleRuta.Services
{
    public class SwitchService : ISwitchService
    {

        private readonly ApplicationDbContext _context;
        public SwitchService(ApplicationDbContext context)
        {
            _context = context;

        }

        public async Task<Switchs> Add(Switchs switchs)
        {
            _context.Switchs.Add(switchs);
            await _context.SaveChangesAsync();
            return switchs;
        }

        public async Task<Switchs> Update(Switchs switchs)
        {
            if (switchs.Id <= 0)
            {
                throw new ArgumentException("El ID del swich no es válido.");
            }

            var existingSwitch = await _context.Switchs.FindAsync(switchs.Id);
            if (existingSwitch is null)
            {

                return existingSwitch;

            }

            existingSwitch.Name = switchs.Name;
            existingSwitch.Description = switchs.Description;
            existingSwitch.TotalPorts = switchs.TotalPorts;
            existingSwitch.GroupCount = switchs.GroupCount;
            existingSwitch.PortsPerGroup = switchs.PortsPerGroup;

            await _context.SaveChangesAsync();
            return existingSwitch;
        }


        public async Task<Switchs> GetById(int id)
        {
            return await _context.Switchs.FindAsync(id);
        }

        public async Task<IEnumerable<Switchs>> GetAll()
        {
            return await _context.Switchs.AsNoTracking().ToListAsync();
        }

        public async Task<bool> Delete(int id)
        {
            var switchs = await _context.Switchs.FindAsync(id);
            if (switchs is null)
            {
                return false;
            }

            _context.Switchs.Remove(switchs);
            await _context.SaveChangesAsync();
            return true;
        }




    }
}