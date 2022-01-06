using Microsoft.EntityFrameworkCore;

namespace WebApiAutores.Utils
{
    public static class HttpContextExtensions
    {
        public async static Task InsertarParametroPaginacionCabezera<T>(this HttpContext httpContext, IQueryable<T> queryable)
        {
            if (httpContext == null)
            {
                throw new ArgumentNullException(nameof(httpContext));
            }
            double cantidad = await queryable.CountAsync();
            httpContext.Response.Headers.Add("cantidadTotalRegistro", cantidad.ToString());
        }
    }
}
