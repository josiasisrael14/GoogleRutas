using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GoogleRuta.Models;
using GoogleRuta.ViewModels;

namespace GoogleRuta.Services.Interfaces;

public interface IElementService
{
    Task Add(ElementViewModel elementViewModel);
    Task<List<ElementType>> GetAll();
    Task<ElementType> GetById(int id);
    Task<bool> Delete(int id);
}