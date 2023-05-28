using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ooze.Typed.Paging;
using Ooze.Typed.Web.Entities;

namespace Ooze.Typed.Web.Controllers;

[ApiController, Route("~/")]
public class BlogController : ControllerBase
{
    private readonly DatabaseContext _db;
    private readonly SqlServerDatabaseContext _sqlServerDb;
    private readonly PostgresDatabaseContext _postgresDb;
    private readonly IOozeTypedResolver _nonTypedResolver;
    private readonly IOozeTypedResolver<Blog, BlogFilters, BlogSorters> _resolver;

    public BlogController(
        DatabaseContext db,
        SqlServerDatabaseContext sqlServerDb,
        PostgresDatabaseContext postgresDb,
        IOozeTypedResolver nonTypedResolver,
        IOozeTypedResolver<Blog, BlogFilters, BlogSorters> resolver)
    {
        _db = db;
        _sqlServerDb = sqlServerDb;
        _postgresDb = postgresDb;
        _nonTypedResolver = nonTypedResolver;
        _resolver = resolver;
    }

    [HttpPost]
    public async Task<IActionResult> Post(Input model)
    {
        IQueryable<Blog> query = _db.Set<Blog>();

        query = _nonTypedResolver
            .Filter(query, model.Filters);
        query = _nonTypedResolver.Sort(query, model.Sorters);

        var results = await query.ToListAsync();
        return Ok(results);
    }

    [HttpPost("/typed")]
    public async Task<IActionResult> PostTyped(Input model)
    {
        IQueryable<Blog> query = _db.Set<Blog>();

        query = _resolver.Apply(query, model.Sorters, model.Filters, model.Paging);

        var results = await query.ToListAsync();
        return Ok(results);
    }

    [HttpPost("/typed-expanded")]
    public async Task<IActionResult> PostTypedExpanded(Input model)
    {
        IQueryable<Blog> query = _db.Set<Blog>();

        query = _resolver
            .WithQuery(query)
            .Sort(model.Sorters)
            .Filter(model.Filters)
            .Page(model.Paging)
            .Apply();

        var results = await query.ToListAsync();
        return Ok(results);
    }

    [HttpPost("/sql-server")]
    public async Task<IActionResult> PostSqlServer(Input model)
    {
        IQueryable<Blog> query = _sqlServerDb.Set<Blog>();

        query = _nonTypedResolver
            .Filter(query, model.Filters);
        query = _nonTypedResolver.Sort(query, model.Sorters);

        var results = await query.ToListAsync();
        return Ok(results);
    }
    
    [HttpPost("/postgres")]
    public async Task<IActionResult> PostPostgres(Input model)
    {
        IQueryable<Blog> query = _postgresDb.Set<Blog>();

        query = _nonTypedResolver
            .Filter(query, model.Filters);
        query = _nonTypedResolver.Sort(query, model.Sorters);

        var results = await query.ToListAsync();
        return Ok(results);
    }
}

public record class Input(BlogFilters Filters, IEnumerable<BlogSorters> Sorters, PagingOptions Paging);