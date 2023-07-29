using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Ooze.Typed.Paging;

namespace Ooze.Typed.Web.Filters;

public sealed class OozeFilter<TEntity, TFilters, TSorters> : IAsyncResultFilter
    where TEntity : class
{
    private readonly IOozeTypedResolver<TEntity, TFilters, TSorters> _resolver;
    private readonly ILogger<OozeFilter<TEntity, TFilters, TSorters>> _log;

    public OozeFilter(
        IOozeTypedResolver<TEntity, TFilters, TSorters> resolver,
        ILogger<OozeFilter<TEntity, TFilters, TSorters>> log)
    {
        _resolver = resolver;
        _log = log;
    }

    public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
    {
        var objectResult = context.Result as ObjectResult;

        if (objectResult?.Value is IQueryable<TEntity> query
            && context.Controller is ControllerBase)
        {
            _log.LogDebug("Binding {modelName}", nameof(RequestInput));
            var model = await JsonSerializer.DeserializeAsync<RequestInput?>(context.HttpContext.Request.Body,
                new JsonSerializerOptions(JsonSerializerDefaults.Web));

            if (model is not null)
            {
                objectResult.Value = _resolver.Apply(query, model.Sorters, model.Filters, model.Paging);
            }
            else
            {
                _log.LogWarning("Binding of {modelName} failed", nameof(RequestInput));
            }
        }

        await next();
    }

    private class RequestInput
    {
        public TFilters? Filters { get; set; }
        public IEnumerable<TSorters>? Sorters { get; set; }
        public PagingOptions? Paging { get; set; }
    }
}