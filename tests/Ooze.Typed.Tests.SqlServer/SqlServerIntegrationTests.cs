using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Ooze.Typed.Extensions;
using Ooze.Typed.Tests.SqlServer.OozeConfiguration;
using Testcontainers.MsSql;

namespace Ooze.Typed.Tests.SqlServer;

public class SqlServerIntegrationTests : IAsyncLifetime
{
    private readonly MsSqlContainer _sqlServerContainer = new MsSqlBuilder()
        .WithImage("mcr.microsoft.com/mssql/server:2022-CU4-ubuntu-20.04")
        .WithPortBinding(1433, true)
        .WithCleanUp(true)
        .Build();

    private static IServiceCollection CreateServiceCollection()
    {
        var services = new ServiceCollection().AddLogging();
        services.AddOozeTyped()
            .Add<PostFiltersProvider>()
            .Add<PostSortersProvider>();

        return services;
    }

    private readonly IServiceProvider _serviceProvider = new DefaultServiceProviderFactory(
        new ServiceProviderOptions
        {
            ValidateScopes = false
        }).CreateServiceProvider(CreateServiceCollection());

    private SqlServerContext CreateContext()
    {
        var correctConnectionString = _sqlServerContainer.GetConnectionString();
        var sqlServerOptions = new DbContextOptionsBuilder()
            .UseSqlServer(correctConnectionString);
        return new SqlServerContext(sqlServerOptions.Options);
    }

    [Fact]
    public async Task IsDate_Should_Update_Query_And_Return_Correct_Items()
    {
        await using var context = CreateContext();
        await context.Database.EnsureCreatedAsync();
    
        var resolver = _serviceProvider.GetRequiredService<IOozeTypedResolver>();
        IQueryable<Post> query = context.Set<Post>();
        query = resolver.Filter(query, new PostFilters(true, null, null, null, null, null, null, null, null, null, null, null, null));
    
        var sql = query.ToQueryString();
        var sqlContainsCall = sql.Contains("ISDATE", StringComparison.InvariantCultureIgnoreCase);
        Assert.True(sqlContainsCall);
    
        var hasFilteredItems = await query.AnyAsync();
        Assert.True(hasFilteredItems == false);
    }
    
    [Fact]
    public async Task IsNumeric_Should_Update_Query_And_Return_Correct_Items()
    {
        await using var context = CreateContext();
    
        var resolver = _serviceProvider.GetRequiredService<IOozeTypedResolver>();
        IQueryable<Post> query = context.Set<Post>();
        query = resolver.Filter(query, new PostFilters(null, true, null, null, null, null, null, null, null, null, null, null, null));
    
        var sql = query.ToQueryString();
        var sqlContainsCall = sql.Contains("ISNUMERIC", StringComparison.InvariantCultureIgnoreCase);
        Assert.True(sqlContainsCall);
    
        var hasFilteredItems = await query.AnyAsync();
        Assert.True(hasFilteredItems == false);
    }
    
    [Fact]
    public async Task DateDiffDay_Should_Update_Query_And_Return_Correct_Items()
    {
        await using var context = CreateContext();
    
        var resolver = _serviceProvider.GetRequiredService<IOozeTypedResolver>();
        IQueryable<Post> query = context.Set<Post>();
        query = resolver.Filter(query,
            new PostFilters(null, null, new DateTime(2022, 5, 20), null, null, null, null, null, null, null, null, null, null));
    
        var sql = query.ToQueryString();
        var sqlContainsCall = sql.Contains("DATEDIFF(day,", StringComparison.InvariantCultureIgnoreCase);
        Assert.True(sqlContainsCall);
    
        var hasFilteredItems = await query.AnyAsync();
        Assert.True(hasFilteredItems == true);
    }
    
    [Fact]
    public async Task DateDiffMonth_Should_Update_Query_And_Return_Correct_Items()
    {
        await using var context = CreateContext();
    
        var resolver = _serviceProvider.GetRequiredService<IOozeTypedResolver>();
        IQueryable<Post> query = context.Set<Post>();
        query = resolver.Filter(query,
            new PostFilters(null, null, null, new DateTime(2022, 5, 20), null, null, null, null, null, null, null, null, null));
    
        var sql = query.ToQueryString();
        var sqlContainsCall = sql.Contains("DATEDIFF(month,", StringComparison.InvariantCultureIgnoreCase);
        Assert.True(sqlContainsCall);
    
        var hasFilteredItems = await query.AnyAsync();
        Assert.True(hasFilteredItems == true);
    }
    
    [Fact]
    public async Task DateDiffYear_Should_Update_Query_And_Return_Correct_Items()
    {
        await using var context = CreateContext();
    
        var resolver = _serviceProvider.GetRequiredService<IOozeTypedResolver>();
        IQueryable<Post> query = context.Set<Post>();
        query = resolver.Filter(query,
            new PostFilters(null, null, null, null, new DateTime(2022, 5, 20), null, null, null, null, null, null, null, null));
    
        var sql = query.ToQueryString();
        var sqlContainsCall = sql.Contains("DATEDIFF(year,", StringComparison.InvariantCultureIgnoreCase);
        Assert.True(sqlContainsCall);
    
        var hasFilteredItems = await query.AnyAsync();
        Assert.True(hasFilteredItems == false);
    }
    
    [Fact]
    public async Task DateDiffWeek_Should_Update_Query_And_Return_Correct_Items()
    {
        await using var context = CreateContext();
    
        var resolver = _serviceProvider.GetRequiredService<IOozeTypedResolver>();
        IQueryable<Post> query = context.Set<Post>();
        query = resolver.Filter(query,
            new PostFilters(null, null, null, null, null, new DateTime(2022, 5, 20), null, null, null, null, null, null, null));
    
        var sql = query.ToQueryString();
        var sqlContainsCall = sql.Contains("DATEDIFF(week,", StringComparison.InvariantCultureIgnoreCase);
        Assert.True(sqlContainsCall);
    
        var hasFilteredItems = await query.AnyAsync();
        Assert.True(hasFilteredItems == true);
    }
    
    [Fact]
    public async Task DateDiffHour_Should_Update_Query_And_Return_Correct_Items()
    {
        await using var context = CreateContext();
    
        var resolver = _serviceProvider.GetRequiredService<IOozeTypedResolver>();
        IQueryable<Post> query = context.Set<Post>();
        query = resolver.Filter(query,
            new PostFilters(null, null, null, null, null, null, new DateTime(2022, 5, 20), null, null, null, null, null, null));
    
        var sql = query.ToQueryString();
        var sqlContainsCall = sql.Contains("DATEDIFF(hour,", StringComparison.InvariantCultureIgnoreCase);
        Assert.True(sqlContainsCall);
    
        var hasFilteredItems = await query.AnyAsync();
        Assert.True(hasFilteredItems == true);
    }
    
    [Fact]
    public async Task DateDiffMinute_Should_Update_Query_And_Return_Correct_Items()
    {
        await using var context = CreateContext();
    
        var resolver = _serviceProvider.GetRequiredService<IOozeTypedResolver>();
        IQueryable<Post> query = context.Set<Post>();
        query = resolver.Filter(query,
            new PostFilters(null, null, null, null, null, null, null, new DateTime(2022, 5, 20), null, null, null, null, null));
    
        var sql = query.ToQueryString();
        var sqlContainsCall = sql.Contains("DATEDIFF(minute,", StringComparison.InvariantCultureIgnoreCase);
        Assert.True(sqlContainsCall);
    
        var hasFilteredItems = await query.AnyAsync();
        Assert.True(hasFilteredItems == true);
    }
    
    [Fact]
    public async Task DateDiffSecond_Should_Update_Query_And_Return_Correct_Items()
    {
        await using var context = CreateContext();
    
        var resolver = _serviceProvider.GetRequiredService<IOozeTypedResolver>();
        IQueryable<Post> query = context.Set<Post>();
        query = resolver.Filter(query,
            new PostFilters(null, null, null, null, null, null, null, null, new DateTime(2022, 5, 20), null, null, null, null));
    
        var sql = query.ToQueryString();
        var sqlContainsCall = sql.Contains("DATEDIFF(second,", StringComparison.InvariantCultureIgnoreCase);
        Assert.True(sqlContainsCall);
    
        var hasFilteredItems = await query.AnyAsync();
        Assert.True(hasFilteredItems == true);
    }

    [Fact]
    public async Task DateDiffMillisecond_Should_Update_Query_And_Return_Correct_Items()
    {
        await using var context = CreateContext();

        var resolver = _serviceProvider.GetRequiredService<IOozeTypedResolver>();
        IQueryable<Post> query = context.Set<Post>();
        query = resolver.Filter(query,
            new PostFilters(null, null, null, null, null, null, null, null, null, new DateTime(2022, 2, 2, 20, 20, 22), new DateTime(2022, 2, 2), null, null));

        var sql = query.ToQueryString();
        var sqlContainsCall = sql.Contains("DATEDIFF(millisecond,", StringComparison.InvariantCultureIgnoreCase);
        Assert.True(sqlContainsCall);

        var hasFilteredItems = await query.AnyAsync();
        Assert.True(hasFilteredItems == false);
    }
    
    [Fact]
    public async Task DateDiffMicrosecond_Should_Update_Query_And_Return_Correct_Items()
    {
        await using var context = CreateContext();

        var resolver = _serviceProvider.GetRequiredService<IOozeTypedResolver>();
        IQueryable<Post> query = context.Set<Post>();
        query = resolver.Filter(query,
            new PostFilters(null, null, null, null, null, null, null, null, null, null, new DateTime(2022, 2, 2, 20, 20, 22), new DateTime(2022, 2, 2, 20, 20, 22), null));

        var sql = query.ToQueryString();
        var sqlContainsCall = sql.Contains("DATEDIFF(microsecond,", StringComparison.InvariantCultureIgnoreCase);
        Assert.True(sqlContainsCall);

        var hasFilteredItems = await query.AnyAsync();
        Assert.True(hasFilteredItems == false);
    }
    
    [Fact]
    public async Task DateDiffNanosecond_Should_Update_Query_And_Return_Correct_Items()
    {
        await using var context = CreateContext();

        var resolver = _serviceProvider.GetRequiredService<IOozeTypedResolver>();
        IQueryable<Post> query = context.Set<Post>();
        query = resolver.Filter(query,
            new PostFilters(null, null, null, null, null, null, null, null, null, null, new DateTime(2022, 2, 2, 20, 20, 22), null, new DateTime(2022, 2, 2, 20, 20, 22)));

        var sql = query.ToQueryString();
        var sqlContainsCall = sql.Contains("DATEDIFF(nanosecond,", StringComparison.InvariantCultureIgnoreCase);
        Assert.True(sqlContainsCall);

        var hasFilteredItems = await query.AnyAsync();
        Assert.True(hasFilteredItems == false);
    }

    public async Task InitializeAsync()
    {
        await _sqlServerContainer.StartAsync()
            .ConfigureAwait(false);
        await using var context = CreateContext();
        await context.Database.EnsureCreatedAsync();
        await context.Seed();
    }

    public async Task DisposeAsync() => await _sqlServerContainer.DisposeAsync()
        .ConfigureAwait(false);
}