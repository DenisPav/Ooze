using Ooze.Typed.Expressions;
using Ooze.Typed.Filters;

namespace Ooze.Typed.Tests;

public class BasicExpressionsTests
{
    [Fact]
    public void Equal_With_Matching_Int_Value_Should_Return_True()
    {
        var entity = new TestEntity(42, null, "Test", null, 0, null, DateTime.Now, null, false, null, 0, null);
        var expression = BasicExpressions.Equal<TestEntity, int>(x => x.Int, 42);

        var compiledExpression = expression.Compile();
        var result = compiledExpression(entity);

        Assert.True(result);
    }

    [Fact]
    public void Equal_With_Non_Matching_Int_Value_Should_Return_False()
    {
        var entity = new TestEntity(42, null, "Test", null, 0, null, DateTime.Now, null, false, null, 0, null);
        var expression = BasicExpressions.Equal<TestEntity, int>(x => x.Int, 99);

        var compiledExpression = expression.Compile();
        var result = compiledExpression(entity);

        Assert.False(result);
    }

    [Fact]
    public void Equal_With_Matching_String_Value_Should_Return_True()
    {
        var entity = new TestEntity(1, null, "TestName", null, 0, null, DateTime.Now, null, false, null, 0, null);
        var expression = BasicExpressions.Equal<TestEntity, string>(x => x.Text, "TestName");

        var compiledExpression = expression.Compile();
        var result = compiledExpression(entity);

        Assert.True(result);
    }

    [Fact]
    public void Equal_With_Nullable_Int_Value_Should_Return_True()
    {
        var entity = new TestEntity(1, 100, "Test", null, 0, null, DateTime.Now, null, false, null, 0, null);
        var expression = BasicExpressions.Equal<TestEntity, int?>(x => x.NullableInt, 100);

        var compiledExpression = expression.Compile();
        var result = compiledExpression(entity);

        Assert.True(result);
    }

    [Fact]
    public void Equal_With_Decimal_Value_Should_Return_True()
    {
        var entity = new TestEntity(1, null, "Test", null, 99.99m, null, DateTime.Now, null, false, null, 0, null);
        var expression = BasicExpressions.Equal<TestEntity, decimal>(x => x.Price, 99.99m);

        var compiledExpression = expression.Compile();
        var result = compiledExpression(entity);

        Assert.True(result);
    }

    [Fact]
    public void Equal_With_Bool_Value_Should_Return_True()
    {
        var entity = new TestEntity(1, null, "Test", null, 0, null, DateTime.Now, null, true, null, 0, null);
        var expression = BasicExpressions.Equal<TestEntity, bool>(x => x.Flag, true);

        var compiledExpression = expression.Compile();
        var result = compiledExpression(entity);

        Assert.True(result);
    }

    [Fact]
    public void Equal_With_DateTime_Value_Should_Return_True()
    {
        var testDate = new DateTime(2025, 1, 15, 10, 30, 0);
        var entity = new TestEntity(1, null, "Test", null, 0, null, testDate, null, false, null, 0, null);
        var expression = BasicExpressions.Equal<TestEntity, DateTime>(x => x.Date, testDate);

        var compiledExpression = expression.Compile();
        var result = compiledExpression(entity);

        Assert.True(result);
    }

    [Fact]
    public void NotEqual_With_Different_Int_Value_Should_Return_True()
    {
        var entity = new TestEntity(42, null, "Test", null, 0, null, DateTime.Now, null, false, null, 0, null);
        var expression = BasicExpressions.NotEqual<TestEntity, int>(x => x.Int, 99);

        var compiledExpression = expression.Compile();
        var result = compiledExpression(entity);

        Assert.True(result);
    }

    [Fact]
    public void NotEqual_With_Same_Int_Value_Should_Return_False()
    {
        var entity = new TestEntity(42, null, "Test", null, 0, null, DateTime.Now, null, false, null, 0, null);
        var expression = BasicExpressions.NotEqual<TestEntity, int>(x => x.Int, 42);

        var compiledExpression = expression.Compile();
        var result = compiledExpression(entity);

        Assert.False(result);
    }

    [Fact]
    public void NotEqual_With_Different_String_Value_Should_Return_True()
    {
        var entity = new TestEntity(1, null, "TestName", null, 0, null, DateTime.Now, null, false, null, 0, null);
        var expression = BasicExpressions.NotEqual<TestEntity, string>(x => x.Text, "DifferentName");

        var compiledExpression = expression.Compile();
        var result = compiledExpression(entity);

        Assert.True(result);
    }

    [Fact]
    public void NotEqual_With_Nullable_Decimal_Value_Should_Return_True()
    {
        var entity = new TestEntity(1, null, "Test", null, 0, 50.00m, DateTime.Now, null, false, null, 0, null);
        var expression = BasicExpressions.NotEqual<TestEntity, decimal?>(x => x.NullablePrice, 99.99m);

        var compiledExpression = expression.Compile();
        var result = compiledExpression(entity);

        Assert.True(result);
    }

    [Fact]
    public void GreaterThan_With_Larger_Int_Value_Should_Return_True()
    {
        var entity = new TestEntity(100, null, "Test", null, 0, null, DateTime.Now, null, false, null, 0, null);
        var expression = BasicExpressions.GreaterThan<TestEntity, int>(x => x.Int, 50);

        var compiledExpression = expression.Compile();
        var result = compiledExpression(entity);

        Assert.True(result);
    }

    [Fact]
    public void GreaterThan_With_Smaller_Int_Value_Should_Return_False()
    {
        var entity = new TestEntity(30, null, "Test", null, 0, null, DateTime.Now, null, false, null, 0, null);
        var expression = BasicExpressions.GreaterThan<TestEntity, int>(x => x.Int, 50);

        var compiledExpression = expression.Compile();
        var result = compiledExpression(entity);

        Assert.False(result);
    }

    [Fact]
    public void GreaterThan_With_Equal_Int_Value_Should_Return_False()
    {
        var entity = new TestEntity(50, null, "Test", null, 0, null, DateTime.Now, null, false, null, 0, null);
        var expression = BasicExpressions.GreaterThan<TestEntity, int>(x => x.Int, 50);

        var compiledExpression = expression.Compile();
        var result = compiledExpression(entity);

        Assert.False(result);
    }

    [Fact]
    public void GreaterThan_With_Decimal_Value_Should_Return_True()
    {
        var entity = new TestEntity(1, null, "Test", null, 150.50m, null, DateTime.Now, null, false, null, 0, null);
        var expression = BasicExpressions.GreaterThan<TestEntity, decimal>(x => x.Price, 100.00m);

        var compiledExpression = expression.Compile();
        var result = compiledExpression(entity);

        Assert.True(result);
    }

    [Fact]
    public void GreaterThan_With_Double_Value_Should_Return_True()
    {
        var entity = new TestEntity(1, null, "Test", null, 0, null, DateTime.Now, null, false, null, 4.5, null);
        var expression = BasicExpressions.GreaterThan<TestEntity, double>(x => x.Rating, 3.0);

        var compiledExpression = expression.Compile();
        var result = compiledExpression(entity);

        Assert.True(result);
    }

    [Fact]
    public void GreaterThan_With_DateTime_Value_Should_Return_True()
    {
        var laterDate = new DateTime(2025, 6, 1);
        var earlierDate = new DateTime(2025, 1, 1);
        var entity = new TestEntity(1, null, "Test", null, 0, null, laterDate, null, false, null, 0, null);
        var expression = BasicExpressions.GreaterThan<TestEntity, DateTime>(x => x.Date, earlierDate);

        var compiledExpression = expression.Compile();
        var result = compiledExpression(entity);

        Assert.True(result);
    }

    [Fact]
    public void LessThan_With_Smaller_Int_Value_Should_Return_True()
    {
        var entity = new TestEntity(30, null, "Test", null, 0, null, DateTime.Now, null, false, null, 0, null);
        var expression = BasicExpressions.LessThan<TestEntity, int>(x => x.Int, 50);

        var compiledExpression = expression.Compile();
        var result = compiledExpression(entity);

        Assert.True(result);
    }

    [Fact]
    public void LessThan_With_Larger_Int_Value_Should_Return_False()
    {
        var entity = new TestEntity(100, null, "Test", null, 0, null, DateTime.Now, null, false, null, 0, null);
        var expression = BasicExpressions.LessThan<TestEntity, int>(x => x.Int, 50);

        var compiledExpression = expression.Compile();
        var result = compiledExpression(entity);

        Assert.False(result);
    }

    [Fact]
    public void LessThan_With_Equal_Int_Value_Should_Return_False()
    {
        var entity = new TestEntity(50, null, "Test", null, 0, null, DateTime.Now, null, false, null, 0, null);
        var expression = BasicExpressions.LessThan<TestEntity, int>(x => x.Int, 50);

        var compiledExpression = expression.Compile();
        var result = compiledExpression(entity);

        Assert.False(result);
    }

    [Fact]
    public void LessThan_With_Decimal_Value_Should_Return_True()
    {
        var entity = new TestEntity(1, null, "Test", null, 50.00m, null, DateTime.Now, null, false, null, 0, null);
        var expression = BasicExpressions.LessThan<TestEntity, decimal>(x => x.Price, 100.00m);

        var compiledExpression = expression.Compile();
        var result = compiledExpression(entity);

        Assert.True(result);
    }

    [Fact]
    public void LessThan_With_Nullable_Double_Value_Should_Return_True()
    {
        var entity = new TestEntity(1, null, "Test", null, 0, null, DateTime.Now, null, false, null, 0, 2.5);
        var expression = BasicExpressions.LessThan<TestEntity, double?>(x => x.NullableRating, 5.0);

        var compiledExpression = expression.Compile();
        var result = compiledExpression(entity);

        Assert.True(result);
    }

    [Fact]
    public void In_With_Value_In_Collection_Should_Return_True()
    {
        var entity = new TestEntity(2, null, "Test", null, 0, null, DateTime.Now, null, false, null, 0, null);
        var expression = BasicExpressions.In<TestEntity, int>(x => x.Int, [1, 2, 3], false);

        var compiledExpression = expression.Compile();
        var result = compiledExpression(entity);

        Assert.True(result);
    }

    [Fact]
    public void In_With_Value_Not_In_Collection_Should_Return_False()
    {
        var entity = new TestEntity(5, null, "Test", null, 0, null, DateTime.Now, null, false, null, 0, null);
        var expression = BasicExpressions.In<TestEntity, int>(x => x.Int, [1, 2, 3], false);

        var compiledExpression = expression.Compile();
        var result = compiledExpression(entity);

        Assert.False(result);
    }

    [Fact]
    public void In_With_Negated_And_Value_In_Collection_Should_Return_False()
    {
        var entity = new TestEntity(2, null, "Test", null, 0, null, DateTime.Now, null, false, null, 0, null);
        var expression = BasicExpressions.In<TestEntity, int>(x => x.Int, [1, 2, 3], true);

        var compiledExpression = expression.Compile();
        var result = compiledExpression(entity);

        Assert.False(result);
    }

    [Fact]
    public void In_With_Negated_And_Value_Not_In_Collection_Should_Return_True()
    {
        var entity = new TestEntity(5, null, "Test", null, 0, null, DateTime.Now, null, false, null, 0, null);
        var expression = BasicExpressions.In<TestEntity, int>(x => x.Int, [1, 2, 3], true);

        var compiledExpression = expression.Compile();
        var result = compiledExpression(entity);

        Assert.True(result);
    }

    [Fact]
    public void In_With_String_Collection_Should_Return_True()
    {
        var entity = new TestEntity(1, null, "Beta", null, 0, null, DateTime.Now, null, false, null, 0, null);
        var expression = BasicExpressions.In<TestEntity, string>(x => x.Text, ["Alpha", "Beta", "Gamma"], false);

        var compiledExpression = expression.Compile();
        var result = compiledExpression(entity);

        Assert.True(result);
    }

    [Fact]
    public void In_With_Nullable_Int_Collection_Should_Return_True()
    {
        var entity = new TestEntity(1, 10, "Test", null, 0, null, DateTime.Now, null, false, null, 0, null);
        // Note: BasicExpressions.In unwraps nullable types via GetMemberExpression, so we use int not int?
        var expression = BasicExpressions.In<TestEntity, int?>(x => x.NullableInt, [10, 20, 30], false);

        var compiledExpression = expression.Compile();
        var result = compiledExpression(entity);

        Assert.True(result);
    }

    [Fact]
    public void In_With_Empty_Collection_Should_Return_False()
    {
        var entity = new TestEntity(1, null, "Test", null, 0, null, DateTime.Now, null, false, null, 0, null);
        var expression = BasicExpressions.In<TestEntity, int>(x => x.Int, [], false);

        var compiledExpression = expression.Compile();
        var result = compiledExpression(entity);

        Assert.False(result);
    }

    [Fact]
    public void Range_With_Value_Within_Range_Should_Return_True()
    {
        var entity = new TestEntity(50, null, "Test", null, 0, null, DateTime.Now, null, false, null, 0, null);
        var rangeFilter = new RangeFilter<int> { From = 1, To = 100 };
        var expression = BasicExpressions.Range<TestEntity, int>(x => x.Int, rangeFilter, false);

        var compiledExpression = expression.Compile();
        var result = compiledExpression(entity);

        Assert.True(result);
    }

    [Fact]
    public void Range_With_Value_At_Lower_Boundary_Should_Return_True()
    {
        var entity = new TestEntity(1, null, "Test", null, 0, null, DateTime.Now, null, false, null, 0, null);
        var rangeFilter = new RangeFilter<int> { From = 1, To = 100 };
        var expression = BasicExpressions.Range<TestEntity, int>(x => x.Int, rangeFilter, false);

        var compiledExpression = expression.Compile();
        var result = compiledExpression(entity);

        Assert.True(result);
    }

    [Fact]
    public void Range_With_Value_At_Upper_Boundary_Should_Return_True()
    {
        var entity = new TestEntity(100, null, "Test", null, 0, null, DateTime.Now, null, false, null, 0, null);
        var rangeFilter = new RangeFilter<int> { From = 1, To = 100 };
        var expression = BasicExpressions.Range<TestEntity, int>(x => x.Int, rangeFilter, false);

        var compiledExpression = expression.Compile();
        var result = compiledExpression(entity);

        Assert.True(result);
    }

    [Fact]
    public void Range_With_Value_Below_Range_Should_Return_False()
    {
        var entity = new TestEntity(0, null, "Test", null, 0, null, DateTime.Now, null, false, null, 0, null);
        var rangeFilter = new RangeFilter<int> { From = 1, To = 100 };
        var expression = BasicExpressions.Range<TestEntity, int>(x => x.Int, rangeFilter, false);

        var compiledExpression = expression.Compile();
        var result = compiledExpression(entity);

        Assert.False(result);
    }

    [Fact]
    public void Range_With_Value_Above_Range_Should_Return_False()
    {
        var entity = new TestEntity(101, null, "Test", null, 0, null, DateTime.Now, null, false, null, 0, null);
        var rangeFilter = new RangeFilter<int> { From = 1, To = 100 };
        var expression = BasicExpressions.Range<TestEntity, int>(x => x.Int, rangeFilter, false);

        var compiledExpression = expression.Compile();
        var result = compiledExpression(entity);

        Assert.False(result);
    }

    [Fact]
    public void Range_With_Negated_And_Value_Within_Range_Should_Return_False()
    {
        var entity = new TestEntity(50, null, "Test", null, 0, null, DateTime.Now, null, false, null, 0, null);
        var rangeFilter = new RangeFilter<int> { From = 1, To = 100 };
        var expression = BasicExpressions.Range<TestEntity, int>(x => x.Int, rangeFilter, true);

        var compiledExpression = expression.Compile();
        var result = compiledExpression(entity);

        Assert.False(result);
    }

    [Fact]
    public void Range_With_Negated_And_Value_Outside_Range_Should_Return_True()
    {
        var entity = new TestEntity(150, null, "Test", null, 0, null, DateTime.Now, null, false, null, 0, null);
        var rangeFilter = new RangeFilter<int> { From = 1, To = 100 };
        var expression = BasicExpressions.Range<TestEntity, int>(x => x.Int, rangeFilter, true);

        var compiledExpression = expression.Compile();
        var result = compiledExpression(entity);

        Assert.True(result);
    }

    [Fact]
    public void Range_With_Decimal_Range_Should_Return_True()
    {
        var entity = new TestEntity(1, null, "Test", null, 75.50m, null, DateTime.Now, null, false, null, 0, null);
        var rangeFilter = new RangeFilter<decimal> { From = 50.00m, To = 100.00m };
        var expression = BasicExpressions.Range<TestEntity, decimal>(x => x.Price, rangeFilter, false);

        var compiledExpression = expression.Compile();
        var result = compiledExpression(entity);

        Assert.True(result);
    }

    [Fact]
    public void Range_With_DateTime_Range_Should_Return_True()
    {
        var testDate = new DateTime(2025, 3, 15);
        var entity = new TestEntity(1, null, "Test", null, 0, null, testDate, null, false, null, 0, null);
        var rangeFilter = new RangeFilter<DateTime>
        {
            From = new DateTime(2025, 1, 1),
            To = new DateTime(2025, 12, 31)
        };
        var expression = BasicExpressions.Range<TestEntity, DateTime>(x => x.Date, rangeFilter, false);

        var compiledExpression = expression.Compile();
        var result = compiledExpression(entity);

        Assert.True(result);
    }

    [Fact]
    public void Range_With_Nullable_Decimal_Range_Should_Return_True()
    {
        var entity = new TestEntity(1, null, "Test", null, 0, 75.00m, DateTime.Now, null, false, null, 0, null);
        var rangeFilter = new RangeFilter<decimal?> { From = 50.00m, To = 100.00m };
        var expression = BasicExpressions.Range<TestEntity, decimal?>(x => x.NullablePrice, rangeFilter, false);

        var compiledExpression = expression.Compile();
        var result = compiledExpression(entity);

        Assert.True(result);
    }

    [Fact]
    public void StringOperation_StartsWith_With_Matching_Prefix_Should_Return_True()
    {
        var entity = new TestEntity(1, null, "TestName", null, 0, null, DateTime.Now, null, false, null, 0, null);
        var expression =
            BasicExpressions.StringOperation<TestEntity>(x => x.Text, "Test", CommonMethods.StringStartsWith, false);

        var compiledExpression = expression.Compile();
        var result = compiledExpression(entity);

        Assert.True(result);
    }

    [Fact]
    public void StringOperation_StartsWith_With_Non_Matching_Prefix_Should_Return_False()
    {
        var entity = new TestEntity(1, null, "TestName", null, 0, null, DateTime.Now, null, false, null, 0, null);
        var expression =
            BasicExpressions.StringOperation<TestEntity>(x => x.Text, "Name", CommonMethods.StringStartsWith, false);

        var compiledExpression = expression.Compile();
        var result = compiledExpression(entity);

        Assert.False(result);
    }

    [Fact]
    public void StringOperation_EndsWith_With_Matching_Suffix_Should_Return_True()
    {
        var entity = new TestEntity(1, null, "TestName", null, 0, null, DateTime.Now, null, false, null, 0, null);
        var expression =
            BasicExpressions.StringOperation<TestEntity>(x => x.Text, "Name", CommonMethods.StringEndsWith, false);

        var compiledExpression = expression.Compile();
        var result = compiledExpression(entity);

        Assert.True(result);
    }

    [Fact]
    public void StringOperation_EndsWith_With_Non_Matching_Suffix_Should_Return_False()
    {
        var entity = new TestEntity(1, null, "TestName", null, 0, null, DateTime.Now, null, false, null, 0, null);
        var expression =
            BasicExpressions.StringOperation<TestEntity>(x => x.Text, "Test", CommonMethods.StringEndsWith, false);

        var compiledExpression = expression.Compile();
        var result = compiledExpression(entity);

        Assert.False(result);
    }

    //TODO: add support for Contains() in the filter builder, tests already cover those scenarios
    [Fact]
    public void StringOperation_Contains_With_Matching_Substring_Should_Return_True()
    {
        var entity = new TestEntity(1, null, "TestName", null, 0, null, DateTime.Now, null, false, null, 0, null);
        var containsMethod = typeof(string).GetMethod("Contains", [typeof(string)])!;
        var expression = BasicExpressions.StringOperation<TestEntity>(x => x.Text, "stNa", containsMethod, false);

        var compiledExpression = expression.Compile();
        var result = compiledExpression(entity);

        Assert.True(result);
    }

    [Fact]
    public void StringOperation_Contains_With_Non_Matching_Substring_Should_Return_False()
    {
        var entity = new TestEntity(1, null, "TestName", null, 0, null, DateTime.Now, null, false, null, 0, null);
        var containsMethod = typeof(string).GetMethod("Contains", [typeof(string)])!;
        var expression = BasicExpressions.StringOperation<TestEntity>(x => x.Text, "xyz", containsMethod, false);

        var compiledExpression = expression.Compile();
        var result = compiledExpression(entity);

        Assert.False(result);
    }

    [Fact]
    public void StringOperation_StartsWith_With_Negated_And_Matching_Prefix_Should_Return_False()
    {
        var entity = new TestEntity(1, null, "TestName", null, 0, null, DateTime.Now, null, false, null, 0, null);
        var expression =
            BasicExpressions.StringOperation<TestEntity>(x => x.Text, "Test", CommonMethods.StringStartsWith, true);

        var compiledExpression = expression.Compile();
        var result = compiledExpression(entity);

        Assert.False(result);
    }

    [Fact]
    public void StringOperation_StartsWith_With_Negated_And_Non_Matching_Prefix_Should_Return_True()
    {
        var entity = new TestEntity(1, null, "TestName", null, 0, null, DateTime.Now, null, false, null, 0, null);
        var expression =
            BasicExpressions.StringOperation<TestEntity>(x => x.Text, "Name", CommonMethods.StringStartsWith, true);

        var compiledExpression = expression.Compile();
        var result = compiledExpression(entity);

        Assert.True(result);
    }

    [Fact]
    public void StringOperation_Contains_With_Empty_String_Should_Return_True()
    {
        var entity = new TestEntity(1, null, "TestName", null, 0, null, DateTime.Now, null, false, null, 0, null);
        var containsMethod = typeof(string).GetMethod("Contains", [typeof(string)])!;
        var expression = BasicExpressions.StringOperation<TestEntity>(x => x.Text, "", containsMethod, false);

        var compiledExpression = expression.Compile();
        var result = compiledExpression(entity);

        Assert.True(result);
    }

    private record TestEntity(
        int Int,
        int? NullableInt,
        string Text,
        string? NullableText,
        decimal Price,
        decimal? NullablePrice,
        DateTime Date,
        DateTime? NullableDate,
        bool Flag,
        bool? NullableFlag,
        double Rating,
        double? NullableRating);
}