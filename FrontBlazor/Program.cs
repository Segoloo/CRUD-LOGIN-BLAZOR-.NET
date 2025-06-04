// Importa los espacios de nombres necesarios para la aplicación Blazor WebAssembly
using Microsoft.AspNetCore.Components.Web;       // Contiene componentes web y características fundamentales para aplicaciones Blazor
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;  // Proporciona la infraestructura para hospedar aplicaciones Blazor WebAssembly en el navegador
using FrontBlazor;                              // El espacio de nombres raíz de nuestro proyecto
using FrontBlazor.Services;                     // Contiene los servicios personalizados para comunicación con la API

// Crea un constructor de host WebAssembly con configuración predeterminada
// Este objeto se utiliza para configurar los servicios y componentes de la aplicación
var builder = WebAssemblyHostBuilder.CreateDefault(args);

// Registra el componente App (definido en App.razor) como componente raíz
// Lo asocia al elemento HTML con el id "app" en wwwroot/index.html
builder.RootComponents.Add<App>("#app");

// Registra el componente HeadOutlet después del elemento <head> en el documento HTML
// Esto permite gestionar el contenido dinámico del encabezado HTML desde los componentes Blazor
// como títulos de página o metadatos
builder.RootComponents.Add<HeadOutlet>("head::after");

// Configura un servicio HttpClient con ámbito (scoped) en el contenedor de inyección de dependencias
// AddScoped significa que se creará una nueva instancia del servicio para cada sesión de usuario
// Este HttpClient se inicializa con la dirección base de la API genérica
// La URL base debe terminar con una barra "/" para que las rutas relativas funcionen correctamente
builder.Services.AddScoped(sp => new HttpClient { 
    BaseAddress = new Uri("http://localhost:5178/") 
});

// Registra el servicio genérico de entidades en el contenedor de inyección de dependencias
// Esto permitirá inyectar el servicio en cualquier componente Blazor que lo necesite
// El servicio maneja todas las operaciones CRUD con cualquier tabla de la base de datos
builder.Services.AddScoped<ServicioEntidad>();

// Construye la aplicación a partir de la configuración establecida en el builder y la ejecuta
// builder.Build() crea la instancia de WebAssemblyHost con todas las configuraciones definidas
// RunAsync() inicia la aplicación y espera hasta que se cierre
// El uso de await asegura que la aplicación siga ejecutándose mientras el navegador esté abierto
await builder.Build().RunAsync();