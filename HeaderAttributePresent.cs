using Microsoft.AspNetCore.Mvc.ActionConstraints;
using System;

namespace WebApiAutores
{
    public class HeaderAttributePresent : Attribute, IActionConstraint
    {
        private readonly string cabezera;
        private readonly string valor;

        public HeaderAttributePresent(string cabezera, string valor)
        {
            this.cabezera = cabezera;
            this.valor = valor;
        }

        public int Order => 0;

        public bool Accept(ActionConstraintContext context)
        {
            var cabezeras = context.RouteContext.HttpContext.Request.Headers;
            if (!cabezeras.ContainsKey(cabezera))
            {
                return false;
            }
            return string.Equals(cabezeras[cabezera], valor, StringComparison.OrdinalIgnoreCase);
        }
    }
}
