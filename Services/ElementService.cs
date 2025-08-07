using GoogleRuta.ViewModels;
using GoogleRuta.Data;
using GoogleRuta.Services.Interfaces;
using GoogleRuta.Models;
using Microsoft.EntityFrameworkCore;

namespace GoogleRuta.Services;


public class ElementService : IElementService
{
    private ILogger<ElementService> _logger;
    private readonly ApplicationDbContext _context;
    public ElementService(ApplicationDbContext context, ILogger<ElementService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task Add(ElementViewModel elementViewModel)
    {
        try
        {
            string iconPath = null;

            // Procesar nueva imagen si fue cargada
            if (elementViewModel.IconoFile != null && elementViewModel.IconoFile.Length > 0)
            {
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(elementViewModel.IconoFile.FileName);
                var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img");

                if (!Directory.Exists(folderPath))
                    Directory.CreateDirectory(folderPath);

                var filePath = Path.Combine(folderPath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await elementViewModel.IconoFile.CopyToAsync(stream);
                }

                iconPath = "/img/" + fileName;
            }

            if (elementViewModel.Id <= 0)
            {
                // Nuevo registro
                var element = new ElementType
                {
                    Name = elementViewModel.Name,
                    IconoUrl = iconPath
                };

                _context.ElementTypes.Add(element);
            }
            else
            {
                // Edición
                var existing = await _context.ElementTypes.FindAsync(elementViewModel.Id);
                if (existing == null)
                    throw new Exception("Elemento no encontrado");

                existing.Name = elementViewModel.Name;

                // Si se cargó nueva imagen, la usa. Si no, conserva la anterior
                if (!string.IsNullOrEmpty(iconPath))
                    existing.IconoUrl = iconPath;
                else if (!string.IsNullOrEmpty(elementViewModel.IconoUrl))
                    existing.IconoUrl = elementViewModel.IconoUrl;
            }

            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al guardar el elemento");
            throw;
        }
    }


    public async Task<List<ElementType>> GetAll()
    {
        try
        {
            return await _context.ElementTypes
                    .AsNoTracking()
                    .OrderBy(e => e.Id)
                    .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al traer la lista");
            throw;
        }

    }

    public async Task<ElementType> GetById(int id)
    {
        try
        {

            var elementType = await _context.ElementTypes
                                     .FirstOrDefaultAsync(p => p.Id == id);
            if (elementType == null)
            {

                throw new Exception("Elementype no encontrado");
            }

            return new ElementType
            {
                Id = elementType.Id,
                Name = elementType.Name,
                IconoUrl = elementType.IconoUrl
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error al obtener el proyecto con ID {id}");
            throw;

        }


    }

    public async Task<bool> Delete(int id)
    {
        try
        {
            var elementType = await _context.ElementTypes
                                    .FirstOrDefaultAsync(p => p.Id == id);
            if (elementType == null)
            {

                throw new Exception("Elementype no encontrado");
            }
            _context.ElementTypes.Remove(elementType);
            var changesSave = await _context.SaveChangesAsync();
            return changesSave > 0;

        }

        catch (DbUpdateConcurrencyException ex)
        {
            // Este error ocurre si la entidad fue modificada o eliminada por otra persona/proceso
            // entre que la leíste y la intentaste guardar.
            Console.Error.WriteLine($"Error de concurrencia al eliminar el tipoElemento con ID {id}: {ex.Message}");
            // Puedes registrar el error con un logger más sofisticado aquí
            return false;
        }

        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error al eliminar el tipoElemento con ID {id}: {ex.Message}");
            return false;

        }
    }

}