using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Ooze.Typed.Query.Exceptions;
using Ooze.Typed.Query.Tests.Entities;
using Ooze.Typed.Query.Tests.OozeQueryConfiguration;
using Ooze.Typed.Query.Tokenization;
using System.Linq;

namespace Ooze.Typed.Query.Tests;

public class QueryIntegrationTests(SqlServerFixture fixture) : IClassFixture<SqlServerFixture>
{
    #region Multiple conditions
    
    [Fact]
    public async Task Multiple_Conditions_Should_Update_Query_And_Return_Correct_Query()
    {
        await using var context = fixture.CreateContext();

        var resolver = fixture.CreateServiceProvider<PostQueryFilterProvider>()
            .GetRequiredService<IQueryLanguageOperationResolver>();
        IQueryable<Post> query = context.Set<Post>();

        query = resolver.FilterWithQueryLanguage(query, @$"{nameof(Post.Id)} >> '20' 
&& {nameof(Post.Name)} == '23_Sample_post_23'");

        var filteredItemsCount = await query.CountAsync();
        Assert.Equal(1, filteredItemsCount);
    }
    
    [Fact]
    public async Task Multiple_Conditions_With_Nested_Filter_Should_Update_Query_And_Return_Correct_Query()
    {
        await using var context = fixture.CreateContext();

        var resolver = fixture.CreateServiceProvider<CommentQueryFilterProvider>()
            .GetRequiredService<IQueryLanguageOperationResolver>();
        IQueryable<Comment> query = context.Set<Comment>();

        query = resolver.FilterWithQueryLanguage(query, @$"{nameof(Comment.Id)} >> '20' 
&& {nameof(Comment.User.Email)} == 'sample_26@email.com'");

        var filteredItemsCount = await query.CountAsync();
        Assert.Equal(1, filteredItemsCount);
    }
    
    [Fact]
    public async Task Multiple_Conditions_With_Brackets_Should_Update_Query_And_Return_Correct_Query()
    {
        await using var context = fixture.CreateContext();

        var resolver = fixture.CreateServiceProvider<PostQueryFilterProvider>()
            .GetRequiredService<IQueryLanguageOperationResolver>();
        IQueryable<Post> query = context.Set<Post>();

        query = resolver.FilterWithQueryLanguage(query, @$"{nameof(Post.Id)} >> '20' 
|| ({nameof(Post.Name)} == '2_Sample_post_2' && {nameof(Post.Id)} == '2')");

        var filteredItemsCount = await query.CountAsync();
        Assert.Equal(81, filteredItemsCount);
    }
    
    #endregion
    
    [Theory]
    [InlineData(QueryLanguageTokenizer.GreaterThan, 20, 80)]
    [InlineData(QueryLanguageTokenizer.GreaterThanOrEqual, 20, 81)]
    [InlineData(QueryLanguageTokenizer.LessThan, 20, 19)]
    [InlineData(QueryLanguageTokenizer.LessThanOrEqual, 20, 20)]
    [InlineData(QueryLanguageTokenizer.EqualTo, 20, 1)]
    [InlineData(QueryLanguageTokenizer.NotEqualTo, 20, 99)]
    [InlineData(QueryLanguageTokenizer.StartsWith, 1, null, true)]
    [InlineData(QueryLanguageTokenizer.EndsWith, 1, null, true)]
    [InlineData(QueryLanguageTokenizer.Contains, 1, null, true)]
    public async Task Operation_On_Int_Should_Update_Query_And_Return_Correct_Query(
        string operation,
        int value,
        int? expectedCount = null,
        bool throws = false)
    {
        var jao_meni = 0;
        
        await using var context = fixture.CreateContext();

        var resolver = fixture.CreateServiceProvider<PostQueryFilterProvider>()
            .GetRequiredService<IQueryLanguageOperationResolver>();
        IQueryable<Post> query = context.Set<Post>();
        var languageQuery = $"{nameof(Post.Id)} {operation} '{value}' ";
        
        if (throws == true)
        {
            Assert.Throws<ExpressionCreatorException>(() => resolver.FilterWithQueryLanguage(query, languageQuery));
            return;
        }
        
        query = resolver.FilterWithQueryLanguage(query, languageQuery);
        var filteredItemsCount = await query.CountAsync();
        Assert.Equal(expectedCount, filteredItemsCount);
    }
    
    [Theory]
    [InlineData(QueryLanguageTokenizer.EqualTo, "5_Sample_post_5", 1)]
    [InlineData(QueryLanguageTokenizer.NotEqualTo, "5_Sample_post_5", 99)]
    [InlineData(QueryLanguageTokenizer.StartsWith, "5_", 1)]
    [InlineData(QueryLanguageTokenizer.EndsWith, "post_5", 1)]
    [InlineData(QueryLanguageTokenizer.Contains, "5_Sa", 10)]
    [InlineData(QueryLanguageTokenizer.GreaterThan, "5_Sample_post_5", null, true)]
    [InlineData(QueryLanguageTokenizer.GreaterThanOrEqual, "5_Sample_post_5", null, true)]
    [InlineData(QueryLanguageTokenizer.LessThan, "5_Sample_post_5", null, true)]
    [InlineData(QueryLanguageTokenizer.LessThanOrEqual, "5_Sample_post_5", null, true)]
    public async Task Operation_On_String_Should_Update_Query_And_Return_Correct_Query(
        string operation,
        string value,
        int? expectedCount = null,
        bool throws = false)
    {
        await using var context = fixture.CreateContext();

        var resolver = fixture.CreateServiceProvider<PostQueryFilterProvider>()
            .GetRequiredService<IQueryLanguageOperationResolver>();
        IQueryable<Post> query = context.Set<Post>();
        var languageQuery = $"{nameof(Post.Name)} {operation} '{value}' ";
        
        if (throws == true)
        {
            Assert.Throws<ExpressionCreatorException>(() => resolver.FilterWithQueryLanguage(query, languageQuery));
            return;
        }
        
        query = resolver.FilterWithQueryLanguage(query, languageQuery);
        var filteredItemsCount = await query.CountAsync();
        Assert.Equal(expectedCount, filteredItemsCount);
    }
    
    [Theory]
    [InlineData(QueryLanguageTokenizer.EqualTo, true, 50)]
    [InlineData(QueryLanguageTokenizer.NotEqualTo, true, 50)]
    [InlineData(QueryLanguageTokenizer.GreaterThan, true, null, true)]
    [InlineData(QueryLanguageTokenizer.GreaterThanOrEqual, true, null, true)]
    [InlineData(QueryLanguageTokenizer.LessThan, true, null, true)]
    [InlineData(QueryLanguageTokenizer.LessThanOrEqual, true, null, true)]
    [InlineData(QueryLanguageTokenizer.StartsWith, true, null, true)]
    [InlineData(QueryLanguageTokenizer.EndsWith, true, null, true)]
    [InlineData(QueryLanguageTokenizer.Contains, true, null, true)]
    public async Task Operation_On_Bool_Should_Update_Query_And_Return_Correct_Query(
        string operation,
        bool value,
        int? expectedCount = null,
        bool throws = false)
    {
        await using var context = fixture.CreateContext();

        var resolver = fixture.CreateServiceProvider<PostQueryFilterProvider>()
            .GetRequiredService<IQueryLanguageOperationResolver>();
        IQueryable<Post> query = context.Set<Post>();
        var languageQuery = $"{nameof(Post.Enabled)} {operation} '{value}' ";
        
        if (throws == true)
        {
            Assert.Throws<ExpressionCreatorException>(() => resolver.FilterWithQueryLanguage(query, languageQuery));
            return;
        }
        
        query = resolver.FilterWithQueryLanguage(query, languageQuery);
        var filteredItemsCount = await query.CountAsync();
        Assert.Equal(expectedCount, filteredItemsCount);
    }
    
    [Theory]
    [InlineData(QueryLanguageTokenizer.EqualTo, 2.5, 1)]
    [InlineData(QueryLanguageTokenizer.NotEqualTo, 2.5, 99)]
    [InlineData(QueryLanguageTokenizer.GreaterThan, 2.5, 98)]
    [InlineData(QueryLanguageTokenizer.GreaterThanOrEqual, 2.5, 99)]
    [InlineData(QueryLanguageTokenizer.LessThan, 2.5, 1)]
    [InlineData(QueryLanguageTokenizer.LessThanOrEqual, 2.5, 2)]
    [InlineData(QueryLanguageTokenizer.StartsWith, 2.5, null, true)]
    [InlineData(QueryLanguageTokenizer.EndsWith, 2.5, null, true)]
    [InlineData(QueryLanguageTokenizer.Contains, 2.5, null, true)]
    public async Task Operation_On_Decimal_Should_Update_Query_And_Return_Correct_Query(
        string operation,
        decimal value,
        int? expectedCount = null,
        bool throws = false)
    {
        await using var context = fixture.CreateContext();

        var resolver = fixture.CreateServiceProvider<PostQueryFilterProvider>()
            .GetRequiredService<IQueryLanguageOperationResolver>();
        IQueryable<Post> query = context.Set<Post>();
        var languageQuery = $"{nameof(Post.Rating)} {operation} '{value}' ";
        
        if (throws == true)
        {
            Assert.Throws<ExpressionCreatorException>(() => resolver.FilterWithQueryLanguage(query, languageQuery));
            return;
        }
        
        query = resolver.FilterWithQueryLanguage(query, languageQuery);
        var filteredItemsCount = await query.CountAsync();
        Assert.Equal(expectedCount, filteredItemsCount);
    }
    
    [Theory]
    [InlineData(QueryLanguageTokenizer.EqualTo, "1.4.2022", 1)]
    [InlineData(QueryLanguageTokenizer.NotEqualTo, "1.4.2022", 99)]
    [InlineData(QueryLanguageTokenizer.GreaterThan, "1.4.2022", 97)]
    [InlineData(QueryLanguageTokenizer.GreaterThanOrEqual, "1.4.2022", 98)]
    [InlineData(QueryLanguageTokenizer.LessThan, "1.4.2022", 2)]
    [InlineData(QueryLanguageTokenizer.LessThanOrEqual, "1.4.2022", 3)]
    [InlineData(QueryLanguageTokenizer.StartsWith, "1.4.2022", null, true)]
    [InlineData(QueryLanguageTokenizer.EndsWith, "1.4.2022", null, true)]
    [InlineData(QueryLanguageTokenizer.Contains, "1.4.2022", null, true)]
    public async Task Operation_On_DateOnly_Should_Update_Query_And_Return_Correct_Query(
        string operation,
        string value,
        int? expectedCount = null,
        bool throws = false)
    {
        await using var context = fixture.CreateContext();

        var resolver = fixture.CreateServiceProvider<PostQueryFilterProvider>()
            .GetRequiredService<IQueryLanguageOperationResolver>();
        IQueryable<Post> query = context.Set<Post>();
        var languageQuery = $"{nameof(Post.OnlyDate)} {operation} '{value}' ";
        
        if (throws == true)
        {
            Assert.Throws<ExpressionCreatorException>(() => resolver.FilterWithQueryLanguage(query, languageQuery));
            return;
        }
        
        query = resolver.FilterWithQueryLanguage(query, languageQuery);
        var filteredItemsCount = await query.CountAsync();
        Assert.Equal(expectedCount, filteredItemsCount);
    }
}