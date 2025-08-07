using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GoogleRuta.Data;
using GoogleRuta.Services.Interfaces;
using GoogleRuta.Models;
using Microsoft.EntityFrameworkCore;

namespace GoogleRuta.Services
{
    public class ColorTracesServices : IColorTracesService
    {

        private readonly ApplicationDbContext _context;
        private readonly ILogger<ColorTracesServices> _logger;

        public ColorTracesServices(ApplicationDbContext context)
        {
            _context = context;

        }

        public async Task<IEnumerable<ColorTraces>> GetAll()
        {
            //toListAsync: trae todo los regitros de la tabla y lo convierte en una List<ColorTraces>
            return await _context.ColorTraces.ToListAsync();

        }

        public async Task Add(ColorTraces colorTrace)
        {
            if (colorTrace.Id == 0)
            {
                _context.ColorTraces.Add(colorTrace);
            }
            else
            {
                var colorTraces = await _context.ColorTraces.FirstOrDefaultAsync(p => p.Id == colorTrace.Id);
                if (colorTraces == null)
                {
                    _logger.LogWarning("No encontrado");
                    return;
                }

                colorTraces.Name = colorTrace.Name;
                colorTraces.Color = colorTrace.Color;
            }

            await _context.SaveChangesAsync();


        }

        public async Task<ColorTraces> GetById(int id)
        {

            var result = await _context.ColorTraces.FirstOrDefaultAsync(p => p.Id == id);
            if (result == null)
            {
                _logger.LogWarning("No encontrado");
                return null;
            }

            return result;

        }

        public async Task<bool> Delete(int id)
        {
            var color = await _context.ColorTraces
                                        .FirstOrDefaultAsync(p => p.Id == id);
            if (color == null)
            {

                throw new Exception("Color no encontrado");
            }
            _context.ColorTraces.Remove(color);
            var changesSave = await _context.SaveChangesAsync();
            return changesSave > 0;





        }

    }
}