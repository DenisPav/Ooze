using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ooze.Typed.Extensions;
using Ooze.Typed.Web;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<DatabaseContext>(opts => opts.UseSqlite("Data Source=./database.db;").EnableSensitiveDataLogging());
builder.Services.AddDbContext<SqlServerDatabaseContext>(opts => opts.UseSqlServer(builder.Configuration.GetConnectionString("SqlServer")).EnableSensitiveDataLogging());
builder.Services.AddDbContext<PostgresDatabaseContext>(opts => opts.UseNpgsql(builder.Configuration.GetConnectionString("Postgres")).EnableSensitiveDataLogging());
builder.Services.AddHostedService<SeedService>();
builder.Services.AddOozeTyped()
    .Add<BlogFiltersProvider>()
    .Add<BlogSortersProvider>()
    .Add<CommentsSortersProvider>()
    .Add<CommentFiltersProvider>();
builder.Services.AddControllers();
builder.Services.Configure<ApiBehaviorOptions>(opts =>
{
    opts.SuppressModelStateInvalidFilter = true;
});

var app = builder.Build();
app.UseRouting();
app.MapControllers();
app.Run();
