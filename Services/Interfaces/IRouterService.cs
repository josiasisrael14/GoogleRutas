using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GoogleRuta.Models;

namespace GoogleRuta.Services.Interfaces
{
    public interface IRouterService
    {
        Task<Router> Add(Router router);
        Task<Router> Update(Router router);
        Task<Router> GetById(int id);
        Task<IEnumerable<Router>> GetAll();
        Task<bool> Delete(int id);
    }
}