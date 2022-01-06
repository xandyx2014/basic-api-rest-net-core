﻿using WebApiAutores.DTO;

namespace WebApiAutores.Utils
{
    public static class IQueryableExtensions
    {
        public static IQueryable<T> Paginar<T>(this IQueryable<T> queryable, PaginacionDto paginacionDto)
        {
            return queryable
                .Skip((paginacionDto.Pagina - 1) * paginacionDto.RecordsPorPagina)
                .Take(paginacionDto.RecordsPorPagina);

        }
    }
}
