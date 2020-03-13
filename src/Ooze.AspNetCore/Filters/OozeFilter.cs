using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Linq;
using System.Threading.Tasks;

namespace Ooze.AspNetCore.Filters
{
    public sealed class OozeFilter<TEntity> : IAsyncResultFilter
        where TEntity : class
    {
        readonly IOozeResolver _resolver;

        public OozeFilter(
            IOozeResolver resolver)
        {
            _resolver = resolver;
        }

        public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
        {
            var objectResult = context.Result as ObjectResult;

            if (objectResult?.Value is IQueryable<TEntity> query
                && context.Controller is ControllerBase controller)
            {
                var model = new OozeModel();

                if (await controller.TryUpdateModelAsync(model))
                {
                    objectResult.Value = _resolver.Apply(query, model);
                }
            }

            await next();
        }
    }
}
