// using Microsoft.EntityFrameworkCore;
// using Microsoft.Extensions.DependencyInjection;
// using Ooze.Typed.Tests.Base;
// using Ooze.Typed.Tests.Base.Setup;
// using Ooze.Typed.Tests.Sqlite.Setup;
//
// namespace Ooze.Typed.Tests.Sqlite;
//
// public class AsyncDatabaseFilterGlobIntegrationTests(SqliteFixture fixture)
//     : IClassFixture<SqliteFixture>
// {
//     [Fact]
//     public async Task Should_Correctly_Filter_Data_By_Glob_Filter()
//     {
//         using var scope = DbFixture.CreateServiceProvider<PostGlobFiltersProvider>().CreateScope();
//         var provider = scope.ServiceProvider;
//
//         await using var context = fixture.CreateContext();
//         var oozeResolver = provider.GetRequiredService<IAsyncOperationResolver<Post, PostGlobFilters, PostSorters>>();
//
//         IQueryable<Post> query = context.Posts;
//         query = await oozeResolver.WithQuery(query)
//             .Filter(new PostGlobFilters("*Sample*post*"))
//             .ApplyAsync();
//
//         var results = await query.ToListAsync();
//         Assert.True(results.Count == TestDbContext.TotalCountOfFakes);
//     }
//
//     [Theory]
//     [InlineData(1)]
//     [InlineData(5)]
//     [InlineData(10)]
//     [InlineData(100)]
//     public async Task Should_Correctly_Filter_Data_By_Glob_Int_Filter(int postId)
//     {
//         using var scope = DbFixture.CreateServiceProvider<PostGlobFiltersProvider>().CreateScope();
//         var provider = scope.ServiceProvider;
//
//         await using var context = fixture.CreateContext();
//         var oozeResolver = provider.GetRequiredService<IAsyncOperationResolver<Post, PostGlobFilters, PostSorters>>();
//
//         IQueryable<Post> query = context.Posts;
//         query = await oozeResolver.WithQuery(query)
//             .Filter(new PostGlobFilters($"{postId}_Sample*post*"))
//             .ApplyAsync();
//
//         var results = await query.ToListAsync();
//         Assert.Single(results);
//         Assert.True(results.All(x => x.Id == postId));
//     }
// }