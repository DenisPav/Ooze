using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ooze.Typed.Extensions;
using Ooze.Typed.Web;
using Ooze.Typed.Web.Filters;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<SqliteDatabaseContext>(opts =>
    //TODO: move to connection strings section in appsettings to be consistent with other connection strings
    opts.UseSqlite("Data Source=./database.db;").EnableSensitiveDataLogging());
builder.Services.AddDbContext<SqlServerDatabaseContext>(opts =>
    opts.UseSqlServer(builder.Configuration.GetConnectionString("SqlServer")).EnableSensitiveDataLogging());
builder.Services.AddDbContext<PostgresDatabaseContext>(opts =>
    opts.UseNpgsql(builder.Configuration.GetConnectionString("Postgres")).EnableSensitiveDataLogging());
var serverVersion = ServerVersion.Create(new Version("10.7.4"), ServerType.MariaDb);
builder.Services.AddDbContext<MariaDbDatabaseContext>(opts =>
opts.UseMySql(builder.Configuration.GetConnectionString("MariaDb"), serverVersion).EnableSensitiveDataLogging());
builder.Services.AddHostedService<SeedService>();
builder.Services.AddOozeTyped()
    .EnableAsyncResolvers()
    .Add<BlogFiltersProvider>()
    .Add<BlogSortersProvider>()
    .Add<CommentsSortersProvider>()
    .Add<CommentFiltersProvider>();
builder.Services.AddControllers();
builder.Services.AddHttpContextAccessor();
builder.Services.Configure<ApiBehaviorOptions>(opts => { opts.SuppressModelStateInvalidFilter = true; });
builder.Services.AddScoped(typeof(OozeFilter<,,>));

var app = builder.Build();
app.UseRouting();
app.MapControllers();
app.Run();