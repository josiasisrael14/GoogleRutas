using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GoogleRuta.Models;

namespace GoogleRuta.Services.Interfaces;

public interface IOdfService
{
    Task<Odf> Add(Odf odf);
    Task<Odf> Update(Odf odf);
    Task<Odf> GetById(int id);
    Task<IEnumerable<Odf>> GetAll();
    Task<bool> Delete(int id);
}