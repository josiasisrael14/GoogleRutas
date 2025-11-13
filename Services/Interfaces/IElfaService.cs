using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GoogleRuta.Models;

namespace GoogleRuta.Services.Interfaces;

public interface IElfaService
{
    Task<Elfa> Add(Elfa elfa);
    Task<Elfa> Update(Elfa elfa);
    Task<Elfa> GetById(int id);
    Task<IEnumerable<Elfa>> GetAll();
    Task<bool> Delete(int id);
}