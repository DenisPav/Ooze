using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ooze.Typed.Paging;
using Ooze.Typed.Query;
using Ooze.Typed.Web.Entities;

namespace Ooze.Typed.Web.Controllers;

[ApiController, Route("~/")]
public class BlogController(
    SqliteDatabaseContext db,
    IQueryLanguageOperationResolver nonTypedResolver,
    IQueryLanguageOperationResolver<Blog, BlogFilters, BlogSorters> resolver)
    : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Post(Input model)
    {
        IQueryable<Blog> query = db.Set<Blog>();

        query = nonTypedResolver
            .Filter(query, model.Filters);
        query = nonTypedResolver.Sort(query, model.Sorters);

        var results = await query.ToListAsync();
        return Ok(results);
    }

    [HttpPost("/typed")]
    public async Task<IActionResult> PostTyped(Input model)
    {
        IQueryable<Blog> query = db.Set<Blog>();

        // query = _nonTypedResolver.Filter(query, model.Filters);
        query = nonTypedResolver.FilterWithQueryLanguage(query, model.Query);

        var results = await query.ToListAsync();
        return Ok(results);
    }

    [HttpPost("/typed-expanded")]
    public async Task<IActionResult> PostTypedExpanded(Input model)
    {
        IQueryable<Blog> query = db.Set<Blog>();

        query = resolver
            .WithQuery(query)
            .Sort(model.Sorters)
            .Filter(model.Filters)
            .Page(model.Paging)
            .Apply();

        var results = await query.ToListAsync();
        return Ok(results);
    }
}

public record Input(BlogFilters Filters, IEnumerable<BlogSorters> Sorters, PagingOptions Paging, string Query);
public record CommentInput(CommentFilters Filters);