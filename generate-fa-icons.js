// // generate-fa-icons.js
// const fs = require('fs');
// const path = require('path');
// const { fas } = require('@fortawesome/free-solid-svg-icons');

// // Define la ruta donde se guardará el JSON. Ajusta 'wwwroot/js' según tu estructura.
// const outputDir = path.join(__dirname, 'wwwroot', 'js');
// const outputPath = path.join(outputDir, 'fa-icons.json');

// // Mapea los íconos al formato de clase que necesitas (ej: 'fa-solid fa-star')
// const allSolidIcons = Object.values(fas).map(icon => `fa-solid fa-${icon.iconName}`);

// // Asegúrate de que el directorio de salida exista
// if (!fs.existsSync(outputDir)) {
//     fs.mkdirSync(outputDir, { recursive: true });
// }

// // Escribe el archivo JSON
// fs.writeFileSync(outputPath, JSON.stringify(allSolidIcons, null, 2));

// console.log(`¡Éxito! Se generó el archivo fa-icons.json con ${allSolidIcons.length} íconos en ${outputPath}`);

// generate-fa-icons.js
const fs = require('fs');
const path = require('path');
const { fas } = require('@fortawesome/free-solid-svg-icons');

// --- Tarea 1: Generar el archivo JSON con las clases de los íconos ---

const jsonOutputDir = path.join(__dirname, 'wwwroot', 'js');
const jsonOutputPath = path.join(jsonOutputDir, 'fa-icons.json');
const allSolidIconClasses = Object.values(fas).map(icon => `fa-solid fa-${icon.iconName}`);

if (!fs.existsSync(jsonOutputDir)) {
    fs.mkdirSync(jsonOutputDir, { recursive: true });
}
fs.writeFileSync(jsonOutputPath, JSON.stringify(allSolidIconClasses, null, 2));
console.log(`✅ Tarea 1/2: Se generó ${jsonOutputPath} con ${allSolidIconClasses.length} clases de íconos.`);


// --- Tarea 2 (NUEVO): Extraer cada ícono como un archivo .svg individual ---

const svgOutputDir = path.join(__dirname, 'wwwroot', 'icons', 'svg');

// Asegurarse de que la carpeta de destino exista
if (!fs.existsSync(svgOutputDir)) {
    fs.mkdirSync(svgOutputDir, { recursive: true });
}

let svgFileCount = 0;
for (const icon of Object.values(fas)) {
    const iconName = icon.iconName;
    const [width, height, , , pathData] = icon.icon; // Extraemos los datos del ícono

    // Construimos el contenido completo del archivo SVG
    const svgContent = `<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 ${width} ${height}"><path fill="currentColor" d="${pathData}"></path></svg>`;
    
    // Definimos la ruta del archivo de salida
    const outputFilePath = path.join(svgOutputDir, `${iconName}.svg`);

    // Escribimos el archivo SVG
    fs.writeFileSync(outputFilePath, svgContent);
    svgFileCount++;
}

console.log(`✅ Tarea 2/2: Se generaron ${svgFileCount} archivos .svg en la carpeta ${svgOutputDir}.`);
console.log('\n¡Proceso completado!');