using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GoogleRuta.Models;

namespace GoogleRuta.Services.Interfaces;

public interface IColorTracesService
{

    Task<IEnumerable<ColorTraces>> GetAll();
    Task Add(ColorTraces colorTrace);
    Task<ColorTraces> GetById(int id);
    Task<bool> Delete(int id);
}