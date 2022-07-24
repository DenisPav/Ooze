using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ooze.Typed;
using Ooze.Typed.Extensions;
using Ooze.Typed.Filters;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<DatabaseContext>(opts => opts.UseSqlite("Data Source=./database.db;"));
builder.Services.AddHostedService<SeedService>();
builder.Services.AddOozeTyped();
builder.Services.AddSingleton<IOozeFilterProvider<Blog, BlogFilters>, BlogFiltersProvider>();
var app = builder.Build();

app.MapGet("/", ([FromServices]DatabaseContext db, [FromServices] IOozeTypedResolver resolver) =>
{
    IQueryable<Blog> query = db.Set<Blog>();

    query = resolver.Filter(query, new BlogFilters
    {
        //BlogId = 7,
        //BlogIds = new[] { 5, 500 },
        IdRange = new RangeFilter<int>
        {
            From = 5,
            To = 10
        },
        Name = "Eden Rutherford"
    });

    return query;
});

app.Run();

public class BlogFiltersProvider : IOozeFilterProvider<Blog, BlogFilters>
{
    public IEnumerable<IFilterDefinition<Blog, BlogFilters>> GetFilters()
    {
        return Filters.CreateFor<Blog, BlogFilters>()
            //.NotEqual(blog => blog.Id, filter => filter.BlogId)
            //.In(blog => blog.Id, filter => filter.BlogIds)
            .Range(blog => blog.Id, filter => filter.IdRange)
            //.Equal(blog => blog.Name, filter => filter.Name)
            //.Custom(filter =>
            //{
            //    var rangeFilter = filter.IdRange;
            //    return rangeFilter != null;
            //}, 
            //filter =>
            //{
            //    var rangeFilter = filter.IdRange;
            //    var from = rangeFilter.From;
            //    var to = rangeFilter.To;
            //    return blog => from <= blog.Id && blog.Id <= to;
            //})
            .Build();
    }
}

public class BlogFilters
{
    public int? BlogId { get; set; }
    public int[] BlogIds { get; set; }
    public string Name { get; set; }
    public IEnumerable<string> Names { get; set; }
    public RangeFilter<int> IdRange { get; set; }
}
