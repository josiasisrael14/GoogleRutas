using GoogleRuta.Data;
using GoogleRuta.Dtos;
using GoogleRuta.Models;
using GoogleRuta.Services.Interfaces;
using GoogleRuta.ViewModels;
using Microsoft.AspNetCore.DataProtection;
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

    // public async Task<ProjectViewModel> GetById(int id)
    // {
    //     try
    //     {
    //         var project = await _context.Projects
    //                       .Include(x => x.CoordinateBs)
    //                       .Include(x => x.ElementProjects)//agrege
    //                       .FirstOrDefaultAsync(x => x.Id == id);
    //         if (project == null)
    //         {
    //             throw new Exception("Proyecto no encontrado");

    //         }

    //         return new ProjectViewModel
    //         {
    //             Id = project.Id,
    //             Name = project.Name,
    //             Coordinates = project.CoordinateBs.Select(c => new CoordinateDto
    //             {
    //                 Lat = Math.Round(Convert.ToDouble(c.Lat), 12),
    //                 Lng = Math.Round(Convert.ToDouble(c.Lng), 12)
    //             }).ToList(),

    //             PlacedElementse = project.ElementProjects.Select(e => new PlacedElement
    //             {
    //                 Id = e.Id,
    //                 Lat = e.Lat,
    //                 Lng = e.Lng,
    //                 ElementTypeId = e.ElementTypeId,
    //                 DrawingId = e.DrawingId
    //             }).ToList()

    //         };

    //     }
    //     catch (Exception ex)
    //     {
    //         _logger.LogError(ex, $"Error al obtener el proyecto con ID {id}");
    //         throw;

    //     }
    // }

    public async Task<ProjectViewModel> GetById(int id)
    {
        try
        {
            var project = await _context.Projects
                          .Include(x => x.CoordinateBs)
                          .Include(x => x.ElementProjects)
                          .FirstOrDefaultAsync(x => x.Id == id);

            if (project == null)
            {
                throw new Exception("Proyecto no encontrado");
            }

            return new ProjectViewModel
            {
                Id = project.Id,
                Name = project.Name,

                // --- LÓGICA DE AGRUPACIÓN DE SEGMENTOS AÑADIDA AQUÍ ---
                Segments = project.CoordinateBs
                            .GroupBy(c => c.SegmentId) // 1. Agrupamos todas las coordenadas por su SegmentId
                            .Select(group => new PolylineSegmentDto // 2. Para cada grupo, creamos un nuevo objeto de segmento
                            {
                                SegmentId = group.Key, // La "llave" del grupo es el SegmentId
                                Color = group.FirstOrDefault()?.Color ?? "#0000FF", // Tomamos el color del primer elemento (con un color por defecto)
                                Coordinates = group.Select(c => new CoordinateDto // 3. Llenamos las coordenadas del segmento
                                {
                                    Lat = Math.Round(Convert.ToDouble(c.Lat), 12),
                                    Lng = Math.Round(Convert.ToDouble(c.Lng), 12)
                                }).ToList()
                            }).ToList(),

                // --- LÓGICA DE ELEMENTOS COLOCADOS (YA LA TENÍAS BIEN) ---
                PlacedElementse = project.ElementProjects.Select(e => new PlacedElement
                {
                    Id = e.Id,
                    Lat = e.Lat,
                    Lng = e.Lng,
                    ElementTypeId = e.ElementTypeId,
                    DrawingId = e.DrawingId
                }).ToList()

                // NOTA: La propiedad 'Coordinates' que aplana todo ya no es tan necesaria aquí,
                // pero podemos dejarla por si algún código antiguo la necesita.
                // Los segmentos son la fuente de verdad para las polilíneas.
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


    //     public async Task<List<ProjectViewModel>> GetAll()
    // {
    //     try
    //     {
    //         var projects = await _context.Projects
    //             .Include(p => p.CoordinateBs)
    //             .ToListAsync();

    //         return projects.Select(p => new ProjectViewModel
    //         {
    //             Id = p.Id,
    //             Name = p.Name,

    //             // --- INICIO DE LA LÓGICA MODIFICADA ---

    //             // Verificamos si las coordenadas corresponden a segmentos
    //             // (lo sabemos si tienen SegmentId y no es el valor por defecto para líneas simples)
    //             Segments = p.CoordinateBs
    //                         .GroupBy(c => c.SegmentId) // 1. Agrupamos todas las coordenadas por su SegmentId
    //                         .Select(group => new PolylineSegmentDto // 2. Para cada grupo, creamos un nuevo objeto de segmento
    //                         {
    //                             SegmentId = group.Key, // La "llave" del grupo es el SegmentId
    //                             Color = group.First().Color, // Tomamos el color del primer elemento del grupo (todos deberían tener el mismo)
    //                             Coordinates = group.Select(c => new CoordinateDto // 3. Llenamos las coordenadas del segmento
    //                             {
    //                                 Lat = Math.Round(Convert.ToDouble(c.Lat), 12),
    //                                 Lng = Math.Round(Convert.ToDouble(c.Lng), 12)
    //                             }).ToList()
    //                         }).ToList(),

    //             // Mantenemos la lógica anterior por si tienes polígonos o líneas simples guardadas
    //             // aunque con el nuevo sistema, esta podría quedar obsoleta.
    //             Coordinates = p.CoordinateBs.Select(c => new CoordinateDto
    //             {
    //                 Lat = Math.Round(Convert.ToDouble(c.Lat), 12),
    //                 Lng = Math.Round(Convert.ToDouble(c.Lng), 12)
    //             }).ToList()

    //         }).ToList();
    //     }
    //     catch (Exception ex)
    //     {
    //         _logger.LogError(ex, "Error al obtener todos los proyectos");
    //         throw;
    //     }
    // }





    // public async Task Add(ProjectViewModel viewModel)
    // {
    //     try
    //     {
    //         Project projects;
    //         // Si encontramos un proyecto existente, lo buscamos; de lo contrario, es un proyecto nuevo.
    //         if (viewModel.Id > 0)
    //         {
    //             // Aquí buscamos los proyectos y sus relaciones con CoordinateB y ElementProjects.
    //             projects = await _context.Projects
    //                       .Include(p => p.CoordinateBs)
    //                       .Include(p => p.ElementProjects) // Seguimos incluyendo ElementProjects para tener los existentes
    //                       .FirstOrDefaultAsync(p => p.Id == viewModel.Id);

    //             if (projects == null) { throw new Exception("Proyecto no encontrado"); }
    //         }
    //         else
    //         {
    //             // Aquí instanciamos el objeto Project y agregamos el nuevo proyecto al contexto, pero aún no guardamos.
    //             projects = new Project();
    //             _context.Projects.Add(projects);
    //         }

    //         // Recibimos los nuevos valores del proyecto a editar o crear y limpiamos las coordenadas existentes.
    //         projects.Name = viewModel.Name;
    //         projects.CoordinateBs.Clear(); // Esto está bien, ya que las coordenadas suelen reemplazarse.

    //         //if (viewModel.Type == "multi_polyline" && viewModel.Segments != null)
    //         if (viewModel.Segments != null)
    //         {
    //             // Múltiples segmentos
    //             foreach (var segment in viewModel.Segments)
    //             {
    //                 foreach (var coord in segment.Coordinates)
    //                 {
    //                     projects.CoordinateBs.Add(new CoordinateB
    //                     {
    //                         Lat = coord.Lat,
    //                         Lng = coord.Lng,
    //                         Color = segment.Color,
    //                         SegmentId = segment.SegmentId
    //                     });
    //                 }
    //             }
    //         }
    //         else if (viewModel.Type == "polyline" && viewModel.Coordinates != null)
    //         {
    //             // Línea simple CON color
    //             foreach (var coord in viewModel.Coordinates)
    //             {
    //                 projects.CoordinateBs.Add(new CoordinateB
    //                 {
    //                     Lat = coord.Lat,
    //                     Lng = coord.Lng,
    //                     Color = viewModel.LineColor ?? "#0000FF", // Usar el color de la línea
    //                     SegmentId = "simple_line" // ID fijo para líneas simples
    //                 });
    //             }
    //         }
    //         else if (viewModel.Coordinates != null)
    //         {
    //             // Polígonos (sin color)
    //             foreach (var coord in viewModel.Coordinates)
    //             {
    //                 projects.CoordinateBs.Add(new CoordinateB
    //                 {
    //                     Lat = coord.Lat,
    //                     Lng = coord.Lng,
    //                 });
    //             }
    //         }

    //         // Guardamos los cambios del proyecto y las coordenadas primero.
    //         await _context.SaveChangesAsync();

    //         // --- INICIO DE LA LÓGICA MODIFICADA PARA ElementProject ---

    //         // 1. Obtenemos una lista de los ElementProjects que ya existen para este proyecto en la base de datos.
    //         var existingElementProjects = projects.ElementProjects.ToList();
    //         // 2. Creamos un HashSet para almacenar los ElementTypeId (o la clave que uses para identificar un elemento)
    //         //    de los elementos que nos llegan en el `viewModel`. Esto nos ayudará a saber cuáles existen y cuáles no.
    //         //hashset solo almacena valores unicos 
    //         var incomingElementProjectKeys = new HashSet<string>(); // Usaremos una clave compuesta (ElementTypeId-Lat-Lng) o solo ElementTypeId si es único

    //         // Itera sobre los elementos que nos llegan en el `viewModel`
    //         if (viewModel.PlacedElementse != null && viewModel.PlacedElementse.Any())
    //         {
    //             foreach (var elementViewModel in viewModel.PlacedElementse)
    //             {
    //                 // **IMPORTANTE**: Define cómo identificas un elemento único.
    //                 // Aquí, he creado una clave simple combinando ElementTypeId, Lat y Lng.
    //                 // Si tienes un `Id` único para `ElementProject` en tu `elementViewModel`, úsalo.
    //                 var currentElementKey = $"{elementViewModel.ElementTypeId}-{elementViewModel.Lat}-{elementViewModel.Lng}";
    //                 incomingElementProjectKeys.Add(currentElementKey);

    //                 // Intenta encontrar un ElementProject existente que coincida con el que viene en el `viewModel`.
    //                 var existingDbElement = existingElementProjects
    //                                         .FirstOrDefault(ep => ep.ElementTypeId == elementViewModel.ElementTypeId &&
    //                                                               ep.Lat == elementViewModel.Lat &&
    //                                                               ep.Lng == elementViewModel.Lng);

    //                 if (existingDbElement != null)
    //                 {
    //                     // Si encontramos un elemento existente, lo ACTUALIZAMOS.
    //                     // **Aquí es donde PRESERVAMOS el DrawingId**: NO lo tocamos.
    //                     existingDbElement.Description = "exito";
    //                     existingDbElement.Lat = elementViewModel.Lat;
    //                     existingDbElement.Lng = elementViewModel.Lng;
    //                     // Actualiza solo los campos que deseas cambiar.
    //                     // Si otros campos como Lat, Lng, ElementTypeId pudiesen cambiar para un elemento existente,
    //                     // también los actualizarías aquí, pero ten cuidado porque son parte de nuestra clave de búsqueda.
    //                     // _context.ElementProjects.Update(existingDbElement); // Marcarlo como modificado explícitamente si tu contexto no lo detecta automáticamente.
    //                 }
    //                 else
    //                 {
    //                     // Si NO encontramos un elemento existente, es un elemento NUEVO.
    //                     // Lo INSERTAMOS en la base de datos.
    //                     var newDbElement = new ElementProject
    //                     {
    //                         ElementTypeId = elementViewModel.ElementTypeId,
    //                         ProjectId = projects.Id,
    //                         Lat = elementViewModel.Lat,
    //                         Lng = elementViewModel.Lng,
    //                         Description = "exito",
    //                         //DrawingId = Guid.NewGuid() // ASIGNAMOS un nuevo DrawingId solo para los elementos nuevos.
    //                     };
    //                     _context.ElementProjects.Add(newDbElement);
    //                 }
    //             }
    //         }

    //         // 3. Ahora, eliminamos los ElementProjects que estaban en la base de datos pero
    //         //    ya NO están en la lista que nos llegó en el `viewModel`.
    //         foreach (var existingDbElement in existingElementProjects)
    //         {
    //             var existingElementKey = $"{existingDbElement.ElementTypeId}-{existingDbElement.Lat}-{existingDbElement.Lng}";

    //             // Si la clave del elemento existente NO está en nuestro conjunto de claves entrantes, lo eliminamos.
    //             if (!incomingElementProjectKeys.Contains(existingElementKey))
    //             {
    //                 _context.ElementProjects.Remove(existingDbElement);
    //             }
    //         }

    //         // --- FIN DE LA LÓGICA MODIFICADA PARA ElementProject ---

    //         // Guardamos todos los cambios relacionados con los ElementProjects (actualizaciones, inserciones, eliminaciones).
    //         await _context.SaveChangesAsync();
    //     }
    //     catch (Exception ex)
    //     {
    //         _logger.LogError(ex, "Ocurrió un error al crear/actualizar el proyecto.");
    //         throw;
    //     }
    // }

    public async Task Add(ProjectViewModel viewModel)
    {
        try
        {
            Project project;
            if (viewModel.Id > 0)
            {
                project = await _context.Projects
                          .Include(p => p.CoordinateBs)
                          .Include(p => p.ElementProjects)
                          .FirstOrDefaultAsync(p => p.Id == viewModel.Id);

                if (project == null) { throw new Exception("Proyecto no encontrado"); }
            }
            else
            {
                project = new Project();
                _context.Projects.Add(project);
            }

            project.Name = viewModel.Name;
            project.CoordinateBs.Clear();

            if (viewModel.Segments != null)
            {
                foreach (var segment in viewModel.Segments)
                {
                    if (segment.Coordinates != null)
                    {
                        foreach (var coord in segment.Coordinates)
                        {
                            project.CoordinateBs.Add(new CoordinateB
                            {
                                Lat = coord.Lat,
                                Lng = coord.Lng,
                                Color = segment.Color,
                                SegmentId = segment.SegmentId
                            });
                        }
                    }
                }
            }

            // =========================================================================
            // =========== LÓGICA DE ACTUALIZACIÓN CON ORDEN CORREGIDO =================
            // =========================================================================

            if (viewModel.PlacedElementse != null)
            {
                // ===== PASO 1 (NUEVO Y CRUCIAL): Corregir los IDs primero =====
                // Antes de hacer nada más, recorremos la lista que llega y nos aseguramos
                // de que la propiedad 'Id' tenga el valor correcto que viene en 'DatabaseId'.
                foreach (var elementVm in viewModel.PlacedElementse)
                {
                    if (elementVm.DatabaseId.HasValue)
                    {
                        elementVm.Id = elementVm.DatabaseId;
                    }
                }
                // A partir de aquí, podemos confiar en que 'elementVm.Id' tiene el valor correcto.

                // ===== PASO 2: Ahora sí, calculamos los IDs a conservar =====
                var incomingElementIds = viewModel.PlacedElementse
                    .Where(e => e.Id.HasValue && e.Id > 0)
                    .Select(e => e.Id.Value)
                    .ToHashSet();

                // ===== PASO 3: Identificamos qué borrar =====
                var elementsToDelete = project.ElementProjects
                    .Where(ep => !incomingElementIds.Contains(ep.Id))
                    .ToList();

                _context.ElementProjects.RemoveRange(elementsToDelete);

                // ===== PASO 4: Actualizamos o añadimos =====
                foreach (var elementVm in viewModel.PlacedElementse)
                {
                    if (elementVm.Id.HasValue && elementVm.Id > 0)
                    {
                        // ACTUALIZAR (ahora funciona porque el Id es correcto)
                        var existingElement = project.ElementProjects.FirstOrDefault(ep => ep.Id == elementVm.Id.Value);
                        if (existingElement != null)
                        {
                            existingElement.Lat = elementVm.Lat;
                            existingElement.Lng = elementVm.Lng;
                        }
                    }
                    else
                    {
                        // AÑADIR NUEVO
                        var newElement = new ElementProject
                        {
                            ProjectId = project.Id,
                            ElementTypeId = elementVm.ElementTypeId.Value,
                            Lat = elementVm.Lat,
                            Lng = elementVm.Lng,
                            Description = "Elemento nuevo"
                        };
                        project.ElementProjects.Add(newElement);
                    }
                }
            }
            else
            {
                _context.ElementProjects.RemoveRange(project.ElementProjects);
            }

            // =========================================================================
            // =========================================================================

            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ocurrió un error al crear/actualizar el proyecto.");
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
