using System.Net.Http.Json;     // Importar esta librería para usar métodos que ayudan a trabajar con JSON en solicitudes HTTP
using System.Text;              // Necesario para trabajar con codificación de texto (UTF-8 para JSON)
using System.Text.Json;         // Proporcionar funcionalidad para serializar y deserializar JSON

namespace FrontBlazor.Services  // Definir el espacio de nombres donde se ubicará esta clase
{
    /// <summary>
    /// Servicio genérico para realizar operaciones CRUD con cualquier entidad a través de la API genérica.
    /// </summary>
    public class ServicioEntidad  // Declarar una clase pública llamada ServicioEntidad
    {
        private readonly HttpClient _clienteHttp;       // Cliente HTTP que se usará para comunicarse con la API
        private readonly JsonSerializerOptions _opcionesJson;  // Opciones para configurar cómo se serializa/deserializa el JSON

        // Constructor: se ejecuta cuando se crea una instancia de esta clase
        public ServicioEntidad(HttpClient clienteHttp)   // Recibir un HttpClient como parámetro mediante inyección de dependencias
        {
            _clienteHttp = clienteHttp;    // Guardar el HttpClient recibido para usarlo en los métodos
            
            // Configurar las opciones para el serializador JSON
            _opcionesJson = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true   // Permitir que coincidan propiedades aunque tengan diferente capitalización 
                                                    // (por ejemplo: "Nombre" coincidirá con "nombre")
            };
        }

        /// <summary>
        /// Obtiene todas las entidades de una tabla específica.
        /// </summary>
        /// <param name="nombreProyecto">Nombre del proyecto en la API.</param>
        /// <param name="nombreTabla">Nombre de la tabla a consultar.</param>
        public async Task<List<Dictionary<string, object>>?> ObtenerTodosAsync(string nombreProyecto, string nombreTabla)
        {
            // Esta función devuelve una lista de diccionarios, donde cada diccionario representa una fila de la tabla
            // Cada diccionario tiene como clave el nombre de la columna y como valor el dato de esa columna
            
            try  // Intentar ejecutar el código dentro de este bloque
            {
                var url = $"api/{nombreProyecto}/{nombreTabla}";  // Construir la URL usando interpolación de strings
                
                // Realizar la petición GET y convertir la respuesta JSON a una lista de diccionarios
                return await _clienteHttp.GetFromJsonAsync<List<Dictionary<string, object>>>(url);
            }
            catch (Exception ex)  // Si ocurre algún error, capturar la excepción
            {
                // Mostrar el error en la consola para depuración
                Console.WriteLine($"Error al obtener datos: {ex.Message}");
                
                // Devolver una lista vacía en lugar de null para evitar errores en la interfaz
                return new List<Dictionary<string, object>>();
            }
        }

        /// <summary>
        /// Obtiene una entidad por su clave primaria.
        /// </summary>
        /// <param name="nombreProyecto">Nombre del proyecto en la API.</param>
        /// <param name="nombreTabla">Nombre de la tabla a consultar.</param>
        /// <param name="nombreClave">Nombre del campo clave.</param>
        /// <param name="valorClave">Valor de la clave a buscar.</param>
        public async Task<Dictionary<string, object>?> ObtenerPorClaveAsync(
            string nombreProyecto, 
            string nombreTabla, 
            string nombreClave, 
            string valorClave)
        {
            // Esta función busca una entidad específica por su clave primaria
            // Devuelve un diccionario que representa la fila encontrada, o null si no se encuentra
            
            try
            {
                // Construir la URL incluyendo la clave primaria y su valor
                var url = $"api/{nombreProyecto}/{nombreTabla}/{nombreClave}/{valorClave}";
                
                // Realizar la petición GET y convertir la respuesta JSON a una lista de diccionarios
                // (La API devuelve una lista aunque solo contenga un elemento)
                var resultado = await _clienteHttp.GetFromJsonAsync<List<Dictionary<string, object>>>(url);
                
                // Devolver el primer elemento de la lista (o null si la lista está vacía)
                return resultado?.FirstOrDefault();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener entidad: {ex.Message}");
                return null;  // Devolver null si ocurre un error
            }
        }

        /// <summary>
        /// Crea una nueva entidad.
        /// </summary>
        /// <param name="nombreProyecto">Nombre del proyecto en la API.</param>
        /// <param name="nombreTabla">Nombre de la tabla donde crear la entidad.</param>
        /// <param name="entidad">Datos de la entidad a crear.</param>
        public async Task<bool> CrearAsync(
            string nombreProyecto, 
            string nombreTabla, 
            Dictionary<string, object> entidad)
        {
            // Esta función crea una nueva entidad en la base de datos
            // Recibe un diccionario con los datos de la entidad a crear
            // Devuelve true si la operación fue exitosa, false en caso contrario
            
            try
            {
                var url = $"api/{nombreProyecto}/{nombreTabla}";  // Construir la URL
                
                // Convertir el diccionario a JSON y prepararlo para enviarlo en la petición HTTP
                var contenido = new StringContent(
                    JsonSerializer.Serialize(entidad),  // Convertir el diccionario a una cadena JSON
                    Encoding.UTF8,                      // Usar codificación UTF-8 (estándar para JSON)
                    "application/json");                // Indicar que el contenido es de tipo JSON
                
                // Enviar la petición POST con los datos de la entidad
                var respuesta = await _clienteHttp.PostAsync(url, contenido);
                
                // Devolver true si la respuesta indica éxito, false en caso contrario
                return respuesta.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al crear entidad: {ex.Message}");
                return false;  // Devolver false si ocurre un error
            }
        }

        /// <summary>
        /// Actualiza una entidad existente.
        /// </summary>
        /// <param name="nombreProyecto">Nombre del proyecto en la API.</param>
        /// <param name="nombreTabla">Nombre de la tabla a actualizar.</param>
        /// <param name="nombreClave">Nombre del campo clave.</param>
        /// <param name="valorClave">Valor de la clave de la entidad a actualizar.</param>
        /// <param name="entidad">Datos actualizados de la entidad.</param>
        public async Task<bool> ActualizarAsync(
        string nombreProyecto, 
        string nombreTabla, 
        string nombreClave,
        string valorClave, 
        Dictionary<string, object> entidad)
        {
            try
            {
                // Esta es la ruta correcta según los atributos Route + HttpPut
                var url = $"api/{nombreProyecto}/{nombreTabla}/{nombreClave}/{valorClave}";

                var contenido = new StringContent(
                    JsonSerializer.Serialize(entidad),
                    Encoding.UTF8,
                    "application/json");

                var respuesta = await _clienteHttp.PutAsync(url, contenido);

                return respuesta.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al actualizar entidad: {ex.Message}");
                return false;
            }
        }


        /// <summary>
        /// Elimina una entidad por su clave primaria.
        /// </summary>
        /// <param name="nombreProyecto">Nombre del proyecto en la API.</param>
        /// <param name="nombreTabla">Nombre de la tabla donde eliminar.</param>
        /// <param name="nombreClave">Nombre del campo clave.</param>
        /// <param name="valorClave">Valor de la clave de la entidad a eliminar.</param>
        public async Task<bool> EliminarAsync(
            string nombreProyecto, 
            string nombreTabla, 
            string nombreClave, 
            string valorClave)
        {
            // Esta función elimina una entidad de la base de datos
            // Solo necesita la información de la clave primaria para identificar qué eliminar
            // Devuelve true si la operación fue exitosa, false en caso contrario
            
            try
            {
                // Construir la URL incluyendo la clave primaria para identificar la entidad a eliminar
                var url = $"api/{nombreProyecto}/{nombreTabla}/{nombreClave}/{valorClave}";
                
                // Enviar la petición DELETE sin necesidad de enviar datos adicionales
                var respuesta = await _clienteHttp.DeleteAsync(url);
                
                // Devolver true si la respuesta indica éxito, false en caso contrario
                return respuesta.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al eliminar entidad: {ex.Message}");
                return false;  // Devolver false si ocurre un error
            }
        }

        public async Task<List<Dictionary<string, object>>?> EjecutarProcedimientoAsync(
            string nombreProyecto,
            string nombreTabla,
            string nombreSP,
            Dictionary<string, object> parametros)
        {
            try
            {
                var url = $"api/{nombreProyecto}/{nombreTabla}/ejecutar-sp";

                // Agregar el nombre del SP como parámetro adicional
                parametros["nombreSP"] = nombreSP;

                var contenido = new StringContent(
                    JsonSerializer.Serialize(parametros),
                    Encoding.UTF8,
                    "application/json"
                );

                var respuesta = await _clienteHttp.PostAsync(url, contenido);
                if (respuesta.IsSuccessStatusCode)
                {
                    var resultado = await respuesta.Content.ReadFromJsonAsync<List<Dictionary<string, object>>>(_opcionesJson);
                    return resultado;
                }

                Console.WriteLine($"Error al ejecutar procedimiento: {respuesta.ReasonPhrase}");
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Excepción en procedimiento: {ex.Message}");
                return null;
            }
        }
    }
}