using GoogleRuta.Data;
using GoogleRuta.Dtos;
using GoogleRuta.Models;
using GoogleRuta.Services.Interfaces;
using GoogleRuta.ViewModels;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace GoogleRuta.Services;

public class ProjectService : IProjectService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<ProjectService> _logger;
    public ProjectService(ApplicationDbContext context, ILogger<ProjectService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<ProjectViewModel> GetById(int id)
    {
        try
        {
            var project = await _context.Projects
                          .Include(x => x.CoordinateBs)
                          .Include(x=>x.ElementProjects)//agrege
                          .FirstOrDefaultAsync(x => x.Id == id);
            if (project == null)
            {
                throw new Exception("Proyecto no encontrado");

            }

            return new ProjectViewModel
            {
                Id = project.Id,
                Name = project.Name,
                Coordinates = project.CoordinateBs.Select(c => new CoordinateDto
                {
                    Lat = Math.Round(Convert.ToDouble(c.Lat), 12),
                    Lng = Math.Round(Convert.ToDouble(c.Lng), 12)
                }).ToList(),
                
                PlacedElementse = project.ElementProjects.Select(e => new PlacedElement
                {
                    Id = e.Id,
                    Lat = e.Lat,
                    Lng = e.Lng,
                    ElementTypeId = e.ElementTypeId,
                    DrawingId=e.DrawingId
            }).ToList()

            };

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error al obtener el proyecto con ID {id}");
            throw;

        }
    }

    public async Task<List<ProjectViewModel>> GetAll()
    {
        try
        {
            var projects = await _context.Projects
                .Include(p => p.CoordinateBs)
                .ToListAsync();//to : indica convertir algo especifico 
                               //list:indica que el tipo que se convierte es a list<t>
                               //indica que la operacion es asincrona

            return projects.Select(p => new ProjectViewModel//select es operador link transforma cada elementos de una coleccion a nueva forma
            {                                               //p es expresion lambda , para cada project p voy a construir una nueva ProjectViewModel
                Id = p.Id,                                  // p es una instancia de project los datos de mi base de datos se lo paso a projectViewModel 
                Name = p.Name,
                Coordinates = p.CoordinateBs.Select(c => new CoordinateDto
                {
                    Lat = Math.Round(Convert.ToDouble(c.Lat), 12),
                    Lng = Math.Round(Convert.ToDouble(c.Lng), 12)
                }).ToList()
            }).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener todos los proyectos");
            throw;
        }
    }

    public async Task Add(ProjectViewModel viewModel)
    {
        try
        {
            Project projects;
            //si encontramos un proyecto existente hacemos esa busqueda sino es un proyecto nuevo y no es para editar.
            if (viewModel.Id > 0)
            {   //aqui buscamos lo projectos y sus relaciones con coordinate, elementos y buscamos el primer elemento que coincide y es de forma asincrona
                projects = await _context.Projects
                          .Include(p => p.CoordinateBs)
                          .Include(p=>p.ElementProjects)
                          .FirstOrDefaultAsync(p => p.Id == viewModel.Id);//busca el primer elemento que busca una condicion
                                                                          //la accion la realiza de forma asincrona. 
                if (projects == null) { throw new Exception("proyecto no encontrado"); }
                ;
            }
            else
            {
                //aqui instanciamos el objeto la variable projects es la instancia del objeto Project y agregamos el nuevo proyecto al contexto pero aun no guardamos
                projects = new Project();
                _context.Projects.Add(projects);
            }
            //recibimos los nuevos valores del proyecto a editar o crear y limpiamos las coordenadas del proyecto para evitar confusiones.
            projects.Name = viewModel.Name;
            projects.CoordinateBs.Clear();
             //asi una condicion si las coordenas son diferentes de null y si las coordenas traen datos entonces agregaremos las nuevas coordenadas a la tabla CoordinateB
            if (viewModel.Coordinates != null && viewModel.Coordinates.Any())
            {
                foreach (var coorDto in viewModel.Coordinates)
                {
                    projects.CoordinateBs.Add(new CoordinateB
                    {
                        Lat = coorDto.Lat,
                        Lng = coorDto.Lng,
                    });
                }

            }//luego lo guardamos en la base de datos.
            await _context.SaveChangesAsync();

            //si los elementos ligados al proyecto existen entonces es poque se va a editar y no crear un nuevo
            // es por eso que aqui lo eliminamos
            if (projects.ElementProjects != null)
            {
                _context.ElementProjects.RemoveRange(projects.ElementProjects);
            }

            //aqui validamos que no este vacio los elementos antes de recorrer 
            //recorremos la lista que nos llega , hacemos una instancia del objeto elementProject para hacer el uso de sus campos y le pasamos los
            //datos, luego agregamos al contexto y al terminar de recorrer cuando ya se recorrio todo , guardamos 
            if (viewModel.PlacedElementse != null && viewModel.PlacedElementse.Any())
            {
                foreach (var elementViewModel in viewModel.PlacedElementse)
                {
                    var newDbElement = new ElementProject
                    {
                        ElementTypeId = elementViewModel.ElementTypeId,
                        ProjectId = projects.Id,
                        Lat = elementViewModel.Lat,
                        Lng = elementViewModel.Lng,
                        Description = "exito"
                    };

                    _context.ElementProjects.Add(newDbElement);
                }


            }

            await _context.SaveChangesAsync();

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ocurrio un error al crear");
            throw;

        }
    }

    public async Task<bool> Delete(int id)
    {
        try
        {//Aqui vamos a buscar en la tabla project por el id que se le pasa desde el parametro con firs.... nos trae el primer elemento encontrado
            var result = await _context.Projects
                                      .FirstOrDefaultAsync(p => p.Id == id);
            // validamos si la resuesta despues de la busqueda contiene el objeto en este caso si es null hacemos un log para almacenar informacion
            //retornamos un false                           
            if (result == null)
            {
                _logger.LogError("Projecto no encontrado para eliminar");
                return false;
            }
            //si no es null entonces reovemos ese objeto de la tabla, y actualizamos informacion 
            //si es mayor a cero es porque no hubo error y elimino. 
            _context.Projects.Remove(result);
             
            var changesSave = await _context.SaveChangesAsync();

            return changesSave > 0;
        }

        catch (DbUpdateConcurrencyException ex)
        {
            // Este error ocurre si la entidad fue modificada o eliminada por otra persona/proceso
            // entre que la leíste y la intentaste guardar.
            Console.Error.WriteLine($"Error de concurrencia al eliminar el proyecto con ID {id}: {ex.Message}");
            // Puedes registrar el error con un logger más sofisticado aquí
            return false;
        }

        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error al eliminar el proyecto con ID {id}: {ex.Message}");
            return false;

        }


    }

}
