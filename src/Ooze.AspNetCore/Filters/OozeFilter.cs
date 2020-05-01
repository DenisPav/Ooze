using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;

namespace Ooze.AspNetCore.Filters
{
    public sealed class OozeFilter<TEntity> : IAsyncResultFilter
        where TEntity : class
    {
        readonly IOozeResolver _resolver;
        readonly ILogger<OozeFilter<TEntity>> _log;

        public OozeFilter(
            IOozeResolver resolver,
            ILogger<OozeFilter<TEntity>> log)
        {
            _resolver = resolver;
            _log = log;
        }

        public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
        {
            var objectResult = context.Result as ObjectResult;

            if (objectResult?.Value is IQueryable<TEntity> query
                && context.Controller is ControllerBase controller)
            {
                _log.LogDebug("Binding {modelName}", nameof(OozeModel));

                var model = new OozeModel();
                if (await controller.TryUpdateModelAsync(model))
                {
                    objectResult.Value = _resolver.Apply(query, model);
                }
                else
                {
                    _log.LogWarning("Binding of {modelName} failed", nameof(OozeModel));
                }
            }

            await next();
        }
    }
}
