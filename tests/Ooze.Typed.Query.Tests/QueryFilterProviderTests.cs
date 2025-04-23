using Ooze.Typed.Query.Exceptions;
using Ooze.Typed.Query.Tests.Entities;
using Ooze.Typed.Query.Tests.OozeQueryConfiguration;

namespace Ooze.Typed.Query.Tests;

public class QueryFilterProviderTests
{
    [Fact]
    public void GetMappings_Should_Return_Correct_Mappings()
    {
        var provider = new PostQueryFilterProvider();
        var mappings = provider.GetMappings()
            .ToList();
        
        Assert.Equal(7, mappings.Count);
        
        var idField = mappings.FirstOrDefault(x => x.Name == nameof(Post.Id));
        Assert.NotNull(idField);
        Assert.Equal(typeof(long), idField.PropertyType);
        Assert.Equal("x.Id", idField.MemberExpression.ToString());
        
        var guidIdField = mappings.FirstOrDefault(x => x.Name == nameof(Post.GuidId));
        Assert.NotNull(guidIdField);
        Assert.Equal(typeof(Guid), guidIdField.PropertyType);
        Assert.Equal($"x.{nameof(Post.GuidId)}", guidIdField.MemberExpression.ToString());
        
        var dateField = mappings.FirstOrDefault(x => x.Name == nameof(Post.Date));
        Assert.NotNull(dateField);
        Assert.Equal(typeof(DateTime), dateField.PropertyType);
        Assert.Equal($"x.{nameof(Post.Date)}", dateField.MemberExpression.ToString());
        
        var dateOnlyField = mappings.FirstOrDefault(x => x.Name == nameof(Post.OnlyDate));
        Assert.NotNull(dateOnlyField);
        Assert.Equal(typeof(DateOnly), dateOnlyField.PropertyType);
        Assert.Equal($"x.{nameof(Post.OnlyDate)}", dateOnlyField.MemberExpression.ToString());
        
        var enabledField = mappings.FirstOrDefault(x => x.Name == nameof(Post.Enabled));
        Assert.NotNull(enabledField);
        Assert.Equal(typeof(bool), enabledField.PropertyType);
        Assert.Equal($"x.{nameof(Post.Enabled)}", enabledField.MemberExpression.ToString());
        
        var nameField = mappings.FirstOrDefault(x => x.Name == nameof(Post.Name));
        Assert.NotNull(nameField);
        Assert.Equal(typeof(string), nameField.PropertyType);
        Assert.Equal($"x.{nameof(Post.Name)}", nameField.MemberExpression.ToString());
        
        var ratingField = mappings.FirstOrDefault(x => x.Name == nameof(Post.Rating));
        Assert.NotNull(ratingField);
        Assert.Equal(typeof(decimal), ratingField.PropertyType);
        Assert.Equal($"x.{nameof(Post.Rating)}", ratingField.MemberExpression.ToString());
    }
    

    [Fact]
    public void GetMappings_Should_Throw_Exception()
    {
        var provider = new FailingPostQueryFilterProvider();

        Assert.Throws<MemberExpressionException>(() => provider.GetMappings());
    }
}