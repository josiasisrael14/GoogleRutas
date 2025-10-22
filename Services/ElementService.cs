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
    private readonly IConfiguration _configuration;
    public ElementService(ApplicationDbContext context, ILogger<ElementService> logger, IConfiguration configuration)
    {
        _context = context;
        _logger = logger;
        _configuration = configuration;
    }

    public async Task Add(ElementViewModel elementViewModel)
    {
        try
        {//aqui es un svg nuevo porque el id es menor a 0
            string iconPath = null;

            if (elementViewModel.Id <= 0)
            {
                // NUEVO ELEMENTO - igual que antes
                //si el icnfile es diferenete de vacio y el iconfile es maor a 0 es porque hay data
                if (elementViewModel.IconoFile != null && elementViewModel.IconoFile.Length > 0)
                {
                    //llama a una funcion y le paso el elementviewmodel como parametro y esa funcion es asincrona el valo lo guardo en la variable de tipo string iconPtah

                    iconPath = await ProcessNewIcon(elementViewModel);
                }
                // aqui creo un nuevo objeot elementtype y le asigno los vlaores que viene desde index por el modelo elementviewmodel
                // y lo guardo en la variable element y le paso el iconpath a la ruta y agrego al contexto de elementtypes
                var element = new ElementType
                {
                    Name = elementViewModel.Name,
                    IconoUrl = iconPath,
                    IconColor = elementViewModel.IconColor
                };

                _context.ElementTypes.Add(element);
            }
            else
            {
                // EDITAR ELEMENTO EXISTENTE 
                //entonces busco por id el elemento existente y compruebo si es nulo , sino es null entonces asigno los nuevos valores

                var existing = await _context.ElementTypes.FindAsync(elementViewModel.Id);
                if (existing == null)
                    throw new Exception("Elemento no encontrado");

                existing.Name = elementViewModel.Name;
                existing.IconColor = elementViewModel.IconColor;

                // Si se cargó nueva imagen
                if (elementViewModel.IconoFile != null && elementViewModel.IconoFile.Length > 0)
                {
                    //si se cargo nuevo elemento le paso a la funcion process al igual que el iconpath le asigno a la ruta
                    iconPath = await ProcessNewIcon(elementViewModel);
                    existing.IconoUrl = iconPath;
                }
                // Si SOLO cambió el color (sin nueva imagen)
                //aqui solo verificamos si solo se asigno un nuevo color
                //verificon si el texto del iconURL termina con la extension .svg  stringComparison.OrdinalIgnoreCase evito problema de mayusculas y minusulas
                //no importa si es mayusicula o minuscula con tal que sea un svg  
                else if (!string.IsNullOrEmpty(existing.IconoUrl) &&
                         existing.IconoUrl.EndsWith(".svg", StringComparison.OrdinalIgnoreCase))
                {
                    // REGENERAR el SVG con el nuevo color
                    //aqui vamos a regenrar el nuevo color que se le asigno el elemento ya que cuando editaba solo el color 
                    //guardaba el color pero no se veia reflejado en el svg porque el svg ya tenia un color asignado y eso como que se guaradaba en la cache del index
                    //traai problemas. a esta funcion le paso la url del icono existente y el nuevo color que se asigno.
                    iconPath = await RegenerateIconWithNewColor(existing.IconoUrl, elementViewModel.IconColor);
                    existing.IconoUrl = iconPath;
                }
            }

            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al guardar el elemento");
            throw;
        }
    }

    // Método para procesar iconos nuevos
    // private async Task<string> ProcessNewIcon(ElementViewModel elementViewModel)
    // {   //aqui vamos a concantenar 
    //     var fileName = Guid.NewGuid().ToString() + Path.GetExtension(elementViewModel.IconoFile.FileName);
    //     //vamos a combinar la ruta actual del directorio con la carpeta wwwroot/img
    //     var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img");

    //     //si no existe la carpeta la crea uando directory.createDirectory
    //     if (!Directory.Exists(folderPath))
    //         Directory.CreateDirectory(folderPath);

    //     //combino la ruta de la carpeta con el nombre del archivo
    //     var filePath = Path.Combine(folderPath, fileName);
    //     //si la extension es un arichivo svg entponces aplico el color al svg 
    //     if (Path.GetExtension(elementViewModel.IconoFile.FileName).ToLower() == ".svg")
    //     {
    //         //creoq ue un streamreader para leer el archivo que se cargo en iconofile
    //         using (var reader = new StreamReader(elementViewModel.IconoFile.OpenReadStream()))
    //         {
    //             //lee todos el texto del archivo svg y lo guarda en la variable svgcontent
    //             var svgContent = await reader.ReadToEndAsync();
    //             // llamao a la funcion applyColor y le paso el svgcontent y el color que se selecciono
    //             svgContent = ApplyColorToSvg(svgContent, elementViewModel.IconColor);
    //             //luego escribo el contenido del svg ya modificado con el nuevo color en la ruta del archivo
    //             await File.WriteAllTextAsync(filePath, svgContent);
    //         }
    //     }
    //     else
    //     {
    //         using (var stream = new FileStream(filePath, FileMode.Create))
    //         {
    //             await elementViewModel.IconoFile.CopyToAsync(stream);
    //         }
    //     }

    //     return "/img/" + fileName;
    // }


    private async Task<string> ProcessNewIcon(ElementViewModel elementViewModel)
    {
        // --- PASO 1: OBTENER LA RUTA FÍSICA SEGURA ---
        // Lee la ruta "C:\\AplicacionContenido\\icono" desde el archivo appsettings.json
        var folderPath = _configuration["RutaContenidoIconos"];

        // Si la ruta no está configurada, es un error grave. Detenemos la ejecución.
        if (string.IsNullOrEmpty(folderPath))
        {
            _logger.LogError("La ruta 'RutaContenidoIconos' no está configurada en appsettings.json");
            throw new Exception("Error de configuración del servidor.");
        }

        // --- PASO 2: PREPARAR EL NOMBRE DEL NUEVO ARCHIVO ---
        // Crea un nombre de archivo único y aleatorio (un GUID) para evitar colisiones.
        // Mantiene la extensión original del archivo subido (ej: ".svg", ".png").
        var fileName = Guid.NewGuid().ToString() + Path.GetExtension(elementViewModel.IconoFile.FileName);

        // Si la carpeta física (ej: C:\AplicacionContenido\icono) no existe, la crea.
        if (!Directory.Exists(folderPath))
            Directory.CreateDirectory(folderPath);

        // Combina la ruta de la carpeta con el nuevo nombre para obtener la ruta completa del archivo.
        // ej: "C:\\AplicacionContenido\\icono\\tu-guid-aqui.svg"
        var filePath = Path.Combine(folderPath, fileName);


        // --- PASO 3: PROCESAR Y GUARDAR EL ARCHIVO ---
        // Comprueba si el archivo subido es un SVG.
        if (Path.GetExtension(elementViewModel.IconoFile.FileName).ToLower() == ".svg")
        {
            // SI ES UN SVG: Lo leemos como texto para poder aplicarle el color.
            using (var reader = new StreamReader(elementViewModel.IconoFile.OpenReadStream()))
            {
                // Lee todo el contenido del archivo SVG a una variable de texto.
                var svgContent = await reader.ReadToEndAsync();

                // Llama a tu método helper para modificar el SVG y añadirle el atributo fill="color".
                svgContent = ApplyColorToSvg(svgContent, elementViewModel.IconColor);

                // Escribe el contenido del SVG ya modificado en el nuevo archivo.
                await File.WriteAllTextAsync(filePath, svgContent);
            }
        }
        else
        {
            // SI NO ES UN SVG (es PNG, JPG, etc.): No podemos aplicarle color.
            // Simplemente copiamos el archivo tal cual a la nueva ubicación.
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await elementViewModel.IconoFile.CopyToAsync(stream);
            }
        }

        // --- PASO 4: DEVOLVER LA URL VIRTUAL ---
        // Devuelve la ruta que se guardará en la base de datos y que usará el navegador.
        // ej: "/contenido-iconos/tu-guid-aqui.svg"
        return "/contenido-iconos/" + fileName;
    }




    // NUEVO MÉTODO - Regenerar SVG con nuevo color
    // private async Task<string> RegenerateIconWithNewColor(string existingIconUrl, string newColor)
    // {
    //     try
    //     {
    //         // Obtener la ruta física del archivo actual
    //         var currentFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot" + existingIconUrl);

    //         if (!File.Exists(currentFilePath))
    //             return existingIconUrl; // Si no existe, mantener la URL actual

    //         // Leer el contenido SVG actual
    //         var svgContent = await File.ReadAllTextAsync(currentFilePath);

    //         // Generar nuevo nombre de archivo para evitar caché
    //         var newFileName = Guid.NewGuid().ToString() + ".svg";
    //         var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img");
    //         var newFilePath = Path.Combine(folderPath, newFileName);

    //         // Aplicar el nuevo color
    //         svgContent = ApplyColorToSvg(svgContent, newColor);

    //         // Guardar con nuevo nombre
    //         await File.WriteAllTextAsync(newFilePath, svgContent);

    //         // Opcional: eliminar archivo anterior para no acumular archivos
    //         try
    //         {
    //             File.Delete(currentFilePath);
    //         }
    //         catch
    //         {
    //             // Ignorar errores al eliminar archivo anterior
    //         }

    //         return "/img/" + newFileName;
    //     }
    //     catch (Exception ex)
    //     {
    //         _logger.LogError(ex, "Error al regenerar icono con nuevo color");
    //         return existingIconUrl; // En caso de error, mantener la URL actual
    //     }
    // }



    private async Task<string> RegenerateIconWithNewColor(string existingIconUrl, string newColor)
    {
        try
        {
            // --- PASO 1: OBTENER LA RUTA FÍSICA SEGURA ---
            var folderPath = _configuration["RutaContenidoIconos"];

            if (string.IsNullOrEmpty(folderPath))
            {
                _logger.LogError("La ruta 'RutaContenidoIconos' no está configurada en appsettings.json");
                throw new Exception("Error de configuración del servidor.");
            }

            // --- PASO 2: CONSTRUIR LA RUTA FÍSICA DEL ARCHIVO ANTIGUO ---
            // Extrae solo el nombre del archivo de la URL virtual.
            // ej: de "/contenido-iconos/archivo-viejo.svg" obtiene "archivo-viejo.svg"
            var existingFileName = Path.GetFileName(existingIconUrl);

            // Combina la ruta de la carpeta con el nombre del archivo antiguo.
            // ej: "C:\\AplicacionContenido\\icono\\archivo-viejo.svg"
            var currentFilePath = Path.Combine(folderPath, existingFileName);

            // Si por alguna razón el archivo antiguo no existe, no hacemos nada para evitar un error.
            if (!File.Exists(currentFilePath))
                return existingIconUrl; // Devuelve la URL original.

            // --- PASO 3: LEER EL SVG ANTIGUO Y PREPARAR EL NUEVO ---
            // Lee todo el contenido del archivo SVG existente.
            var svgContent = await File.ReadAllTextAsync(currentFilePath);

            // Crea un nuevo nombre de archivo único para el ícono recoloreado.
            var newFileName = Guid.NewGuid().ToString() + ".svg";
            var newFilePath = Path.Combine(folderPath, newFileName);

            // --- PASO 4: APLICAR COLOR, GUARDAR NUEVO ARCHIVO Y BORRAR EL ANTIGUO ---
            // Llama a tu método helper para aplicarle el nuevo color al contenido del SVG.
            svgContent = ApplyColorToSvg(svgContent, newColor);

            // Guarda el SVG modificado con el nuevo nombre de archivo.
            await File.WriteAllTextAsync(newFilePath, svgContent);

            // Intenta eliminar el archivo antiguo para no acumular basura.
            // Lo ponemos en un try-catch por si falla (ej: por permisos), para que no rompa toda la operación.
            try
            {
                File.Delete(currentFilePath);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, $"No se pudo eliminar el archivo de ícono antiguo: {currentFilePath}");
            }

            // --- PASO 5: DEVOLVER LA NUEVA URL VIRTUAL ---
            // Devuelve la nueva ruta que se actualizará en la base de datos.
            return "/contenido-iconos/" + newFileName;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al regenerar icono con nuevo color");
            return existingIconUrl; // En caso de cualquier error, devuelve la URL original para no romper nada.
        }
    }

    // Método helper mejorado para aplicar color al SVG
    private string ApplyColorToSvg(string svgContent, string color)
    {
        // Remover atributos fill existentes
        svgContent = System.Text.RegularExpressions.Regex.Replace(
            svgContent,
            @"fill\s*=\s*[""'][^""']*[""']",
            "",
            System.Text.RegularExpressions.RegexOptions.IgnoreCase
        );

        // Agregar fill al elemento svg principal
        if (svgContent.Contains("<svg"))
        {
            svgContent = System.Text.RegularExpressions.Regex.Replace(
                svgContent,
                @"<svg([^>]*)>",
                $"<svg$1 fill=\"{color}\">",
                System.Text.RegularExpressions.RegexOptions.IgnoreCase
            );
        }

        return svgContent;
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