using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiAutores.DTO;
using WebApiAutores.Entidades;
using WebApiAutores.Utils;

namespace WebApiAutores.Controllers.v1
{
    [ApiController]
    [Route("api/v1/autores")]

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "Admin")]
    public class AutoresController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly IConfiguration configuration;

        public AutoresController(ApplicationDbContext context, IMapper mapper, IConfiguration configuration)
        {
            this.context = context;
            this.mapper = mapper;
            this.configuration = configuration;
        }
        [HttpGet("configuracion")]
        public IActionResult ObtenerConfiguracion()
        {
            // el orden de importacion es 1/ lineCommands /  launchSeetings / UserSecrets / EnvironmentVariables / appSettingsEnviroment  / appsettings
            // linea comandos es desde dotnet run -- "APELLIDO= apellido desde linea de comandos"
            // los environmentVariables tambien es un proveedor de IConfiguration
            return Ok(new
            {
                appsettings = configuration["connectionStrings:defaultConnection"],
                environmentVariables = configuration["APELLIDO"]
            });
        }
        // [HttpGet("/listado")] // listado
        // [HttpGet(listado/{id}/{params2?})]
        // [HttpGet("{id:int}/{params2=defaultValue}")]
        //  [Authorize]
        // agregando un filtro personalziado
        // [ServiceFilter(typeof (FilterAction))]
        // Filtro basico aplicado a una accion
        // [ResponseCache(Duration = 10)] // 10 seconds
        [HttpGet(Name = "ObtenerAutores")]
        [AllowAnonymous]
        public async Task<ColeccionRecurso<AutorDto>> Get([FromQuery] PaginacionDto paginacion)
        {

            // public List<Autor> Get() explicita
            // public ActionResult<Autor> Get() // puedes devolver un autoresolve u autor se especifica que se va a devolver
            // public IActionResult Get() // este es dinamico y puede devolver lo que sea que implemente IActionResult
            var queryable = context.Autores.AsQueryable();
            await HttpContext.InsertarParametroPaginacionCabezera(queryable);
            var autores = await queryable.OrderBy(autor => autor.Nombre).Paginar(paginacion).ToListAsync();
            var autorDto = mapper.Map<List<AutorDto>>(autores);




            autorDto.ForEach(x => GenerarEnlaces(x, enlace: Url.Link("ObtenerAutor", new { id = x.Id }), descripcion: "self", metodo: "Get"));
            var resultado = new ColeccionRecurso<AutorDto>() { Valores = autorDto };
            resultado.Enlaces.Add(new DatoHATEOS(
                enlance: Url.Link("ObtenerAutores", new { }),
                descripcion: "self",
                metodo: "Get")
                );
            resultado.Enlaces.Add(new DatoHATEOS(
                enlance: Url.Link("CrearAutor", new { }),
                descripcion: "self",
                metodo: "Post")
                );
            return resultado;
        }
        private void GenerarEnlaces(Recurso recurso, string enlace, string descripcion, string metodo)
        {
            recurso.Enlaces.Add(new DatoHATEOS(enlace, descripcion, metodo));
        }
        [HttpGet("{id:int}", Name = "ObtenerAutor")]
        public async Task<ActionResult<AutorDto>> Get(int id)
        {

            // public List<Autor> Get() explicita
            // public ActionResult<Autor> Get() // puedes devolver un autoresolve u autor se especifica que se va a devolver
            // public IActionResult Get() // este es dinamico y puede devolver lo que sea
            var autor = await context.Autores
                .Include(autor => autor.AutorLibro)
                .ThenInclude(autorLibro => autorLibro.Libro)
                .FirstOrDefaultAsync(x => x.Id == id);
            if (autor == null)
            {
                return NotFound();
            }
            var autorDto = mapper.Map<AutorDto>(autor);
            GenerarEnlaces(autorDto, enlace: Url.Link("ObtenerAutor", new { id = autorDto.Id }), descripcion: "self", metodo: "Get");
            // retorna un  201 con el header location
            return CreatedAtRoute("ObtenerAutor", new { Id = id }, autorDto);
        }
        // From body del cuerpo de estudiar
        [HttpPost(Name = "CrearAutor")]
        public async Task<ActionResult> Post([FromBody] AutorDto autor)
        {
            // public async Task<ActionResult> Post([FromBody]Autor autor, [FromHeader] int mivalor )
            var autorMapped = mapper.Map<Autor>(autor);
            context.Add(autorMapped);
            await context.SaveChangesAsync();
            return Ok(autor);
        }
        [HttpPut("{id}", Name = "ActualizarAutor")]
        public async Task<ActionResult> Put([FromBody] AutorDto autor, int id)
        {
            if (autor.Id.Equals(null) || !autor.Id.Equals(id))
            {
                return BadRequest("EL Id del autor no conside con la url");
            }

            var autorCreate = mapper.Map<Autor>(autor);
            autor.Id = id;
            context.Update(autorCreate);
            await context.SaveChangesAsync();
            return NoContent();

        }
        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            var isExist = await context.Autores.AnyAsync(e => e.Id == id);
            if (!isExist)
            {
                return NotFound();
            }
            context.Remove(new Autor() { Id = id });
            await context.SaveChangesAsync();
            return Ok();
        }
    }
}
