using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WebApiAutores.DTO;
using WebApiAutores.Entidades;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApiAutores.Controllers.v1
{
    [Route("api/v1/libros/{libroId:int}/comentario")]
    [ApiController]
    public class ComentarioController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly UserManager<IdentityUser> userManager;

        public ComentarioController(ApplicationDbContext context, IMapper mapper, UserManager<IdentityUser> userManager)
        {
            this.context = context;
            this.mapper = mapper;
            this.userManager = userManager;
        }

        [HttpGet]
        public async Task<ActionResult<List<ComentarioDto>>> Get(int libroId)
        {
            var comentario = await context.Comentario.Where(comentarioDb => comentarioDb.LibroId == libroId).ToListAsync();
            return mapper.Map<List<ComentarioDto>>(comentario);
        }



        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> Post(int libroId, ComentarioDto comentarioDto)
        {
            // El http context user claims solo sirve con el [Authorize]
            var emailClaim = HttpContext.User.Claims.Where(claim => claim.Type == "email").FirstOrDefault();
            var email = emailClaim?.Value ?? "";
            var user = await userManager.FindByEmailAsync(email);
            if (user == null) return NotFound();
            var userId = user.Id;
            var existLibro = await context.Libro.AnyAsync(libroDb => libroDb.Id == libroId);
            if (!existLibro)
            {
                return NotFound();
            }
            var comentario = mapper.Map<Comentario>(comentarioDto);
            comentario.LibroId = libroId;
            comentario.UserId = userId;
            context.Add(comentario);
            await context.SaveChangesAsync();
            return Ok(comentario);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int libroId, int id, [FromBody] ComentarioDto comentarioDto)
        {
            var existLibro = await context.Libro.AnyAsync(libroDb => libroDb.Id == libroId);
            if (!existLibro)
            {
                return NotFound();
            }
            var existComentary = await context.Libro.AnyAsync(comentarioDb => comentarioDb.Id == id);
            if (!existComentary)
            {
                return NotFound();
            }
            var comentario = mapper.Map<Comentario>(comentarioDto);
            context.Update(comentario);
            comentario.Id = id;
            comentario.LibroId = libroId;
            await context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
