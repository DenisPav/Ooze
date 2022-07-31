using Microsoft.EntityFrameworkCore;
using Ooze.Typed;
using Ooze.Typed.Extensions;
using Ooze.Typed.Filters;
using Ooze.Typed.Sorters;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<DatabaseContext>(opts => opts.UseSqlite("Data Source=./database.db;"));
builder.Services.AddHostedService<SeedService>();
builder.Services.AddOozeTyped()
    .Add<BlogFiltersProvider>()
    .Add<BlogSortersProvider>();
var app = builder.Build();

app.MapGet("/", (
    DatabaseContext db,
    IOozeTypedResolver<Blog, BlogFilters, BlogSorters> resolver,
    int? page,
    string? name,
    SortDirection? idSort) =>
{
    IQueryable<Blog> query = db.Set<Blog>();

    query = resolver

    return query;
});

app.Run();

public class BlogFiltersProvider : IOozeFilterProvider<Blog, BlogFilters>
{
    public IEnumerable<IFilterDefinition<Blog, BlogFilters>> GetFilters()
    {
        return Filters.CreateFor<Blog, BlogFilters>()
            .StartsWith(blog => blog.Name, filter => filter.Name)
            .Build();
    }
}

public class BlogSortersProvider : IOozeSorterProvider<Blog, BlogSorters>
{
    public IEnumerable<ISortDefinition<Blog, BlogSorters>> GetSorters()
    {
        return Sorters.CreateFor<Blog, BlogSorters>()
            .Add(blog => blog.Id, sorter => sorter.BlogIdSort)
            .Build();
    }
}

public class BlogSorters
{
    public SortDirection? BlogIdSort { get; set; }
}

public class BlogFilters
{
    public string Name { get; set; } = default!;
}
