using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiAutores.DTO;
using WebApiAutores.Entidades;
using WebApiAutores.Helper;
namespace WebApiAutores.Controllers.v1
{
    [ApiController]
    [Route("api/v1/Libro")]
    public class LibroController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper mapper;

        public LibroController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            this.mapper = mapper;
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<LibroDto>> Get(int id)
        {
            // var libro = await _context.Libro.Include(librodb => librodb.Comentarios).FirstOrDefaultAsync(x => x.Id == id);
            var libro = await _context.Libro
                .Include(libroDb => libroDb.AutorLibro)
                .ThenInclude(autorLibroDb => autorLibroDb.Autor)
                .FirstOrDefaultAsync(x => x.Id == id);

            libro.AutorLibro = libro.AutorLibro.OrderBy(x => x.Orden).ToList();
            return mapper.Map<LibroDto>(libro);
        }


        [HttpPost]
        public async Task<ActionResult<LibroDto>> Post(LibroDto libroDto)
        {
            var autores = await _context.Autores.Where(autorDb => libroDto.AutorId.Contains(autorDb.Id))
                .Select(autorDb => autorDb.Id)
                .ToListAsync();
            if (libroDto.AutorId.Count != autores.Count)
            {
                return BadRequest("NO existe uno de los autores seleccionado");
            }

            var libro = mapper.Map<Libro>(libroDto);
            if (libro.AutorLibro != null)
            {
                libro.AutorLibro.ForEachWithIndex((autorLibro, index) =>
                {
                    libro.AutorLibro[index].Orden = index;
                });
            }
            _context.Add(libro);
            await _context.SaveChangesAsync();
            return Ok();
        }
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Put(int id, LibroDto libroDto)
        {
            var libroDb = await _context.Libro.Include(x => x.AutorLibro).FirstOrDefaultAsync(x => x.Id == id);
            if (libroDb == null)
            {
                return NotFound();
            }
            libroDb = mapper.Map(libroDto, libroDb);
            await _context.SaveChangesAsync();
            return NoContent();
        }
        [HttpPatch("{id:int}")]
        public async Task<ActionResult<LibroPatchDto>> Patch(int id, JsonPatchDocument<LibroPatchDto> jsonPatchDocumnet)
        {
            if (jsonPatchDocumnet == null)
            {
                return BadRequest();
            }
            var libroDb = await _context.Libro.FirstOrDefaultAsync(x => x.Id == id);
            if (libroDb == null)
            {
                return NotFound();
            }
            var libroDto = mapper.Map<LibroPatchDto>(libroDb);
            jsonPatchDocumnet.ApplyTo(libroDto, ModelState);
            mapper.Map(libroDto, libroDb);
            var isValid = TryValidateModel(libroDto);
            if (!isValid)
            {
                return BadRequest(ModelState);
            }

            await _context.SaveChangesAsync();
            return NoContent();
        }
        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            var exist = await _context.Libro.AnyAsync(x => x.Id == id);
            if (!exist)
            {
                return NotFound(ModelState);
            }
            _context.Remove(new Libro() { Id = id });
            await _context.SaveChangesAsync();
            return Ok();

        }

    }
}
