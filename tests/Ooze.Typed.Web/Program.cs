using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ooze.Typed.Extensions;
using Ooze.Typed.Query.Extensions;
using Ooze.Typed.Web.OozeConfiguration;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<DatabaseContext>(opts => opts.UseSqlite("Data Source=./database.db;").EnableSensitiveDataLogging());
builder.Services.AddHostedService<SeedService>();
builder.Services.AddOozeTyped()
    .AddQueryHandler()
    .Add<BlogFiltersProvider>()
    .Add<BlogSortersProvider>()
    .AddQueryFilter<BlogQueryFiltersProvider>();
builder.Services.AddControllers();
builder.Services.Configure<ApiBehaviorOptions>(opts =>
{
    opts.SuppressModelStateInvalidFilter = true;
});

var app = builder.Build();
app.UseRouting();
app.UseEndpoints(opts => opts.MapDefaultControllerRoute());
app.Run();
