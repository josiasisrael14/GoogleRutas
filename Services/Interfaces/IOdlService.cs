using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GoogleRuta.Models;

namespace GoogleRuta.Services.Interfaces;

public interface IOdlService
{
    Task<Odl> Add(Odl odl);
    Task<Odl> Update(Odl odl);
    Task<Odl> GetById(int id);
    Task<IEnumerable<Odl>> GetAll();
    Task<bool> Delete(int id);
}