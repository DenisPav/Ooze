using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ooze.Typed.Extensions;
using Ooze.Typed.Query.Extensions;
using Ooze.Typed.Web;
using Ooze.Typed.Web.Filters;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<SqliteDatabaseContext>(opts =>
    opts.UseSqlite(builder.Configuration.GetConnectionString("Sqlite")).EnableSensitiveDataLogging());
builder.Services.AddHostedService<SeedService>();
builder.Services.AddOozeTyped()
    .EnableAsyncResolvers()
    .Add<BlogFiltersProvider>()
    .Add<BlogSortersProvider>()
    .Add<CommentsSortersProvider>()
    .Add<CommentFiltersProvider>();
builder.Services.AddOozeQueryLanguage()
    .AddQueryProvider<BlogFiltersProvider>();

builder.Services.AddControllers();
builder.Services.AddHttpContextAccessor();
builder.Services.Configure<ApiBehaviorOptions>(opts => { opts.SuppressModelStateInvalidFilter = true; });
builder.Services.AddScoped(typeof(OozeFilter<,,>));

var app = builder.Build();
app.UseRouting();
app.MapControllers();
app.Run();