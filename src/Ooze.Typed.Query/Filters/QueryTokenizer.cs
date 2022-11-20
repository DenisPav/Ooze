using Superpower;
using Superpower.Model;
using Superpower.Parsers;
using Superpower.Tokenizers;

namespace Ooze.Typed.Query.Filters;

internal static class QueryTokenizer
{
    #region BracketParsers

    private static readonly TextParser<char> LeftBracketParser = Character.EqualTo('(');
    private static readonly TextParser<char> RightBracketParser = Character.EqualTo(')');

    #endregion

    #region OperationParsers

    private static readonly TextParser<TextSpan> GreaterThanParser = Span.EqualToIgnoreCase(">");
    // private static readonly TextParser<TextSpan> GreaterThanOrEqualParser = Span.EqualToIgnoreCase(">@");
    private static readonly TextParser<TextSpan> LessThanParser = Span.EqualToIgnoreCase("<");
    // private static readonly TextParser<TextSpan> LessThanOrEqualParser = Span.EqualToIgnoreCase("<@");
    private static readonly TextParser<TextSpan> EqualToParser = Span.EqualToIgnoreCase("==");
    private static readonly TextParser<TextSpan> NotEqualToParser = Span.EqualToIgnoreCase("!=");
    private static readonly TextParser<Unit> OperatorParser;

    #endregion

    #region LogicalOperationParsers

    private static readonly TextParser<TextSpan> AndParser = Span.EqualToIgnoreCase("&&");
    private static readonly TextParser<TextSpan> OrParser = Span.EqualToIgnoreCase("||");
    private static readonly TextParser<Unit> LogicalOperatorParser;

    #endregion

    #region ValueParsers

    private static readonly TextParser<TextSpan> TickParser = Span.EqualToIgnoreCase("'");
    private static readonly TextParser<char[]> ValueWithoutTicksParser = Character.Except('\'').Many();
    private static readonly TextParser<Unit> ValueParser;

    #endregion

    static QueryTokenizer()
    {
        var operations = new[]
        {
            GreaterThanParser,
            // GreaterThanOrEqualParser, 
            LessThanParser, 
            // LessThanOrEqualParser, 
            EqualToParser,
            NotEqualToParser
        };
        var combinedOperationParser = operations.Aggregate((agg, current) => agg.Or(current));
        OperatorParser = (
            from @operator in combinedOperationParser
            select Unit.Value);

        var logicalOperations = new[] { AndParser, OrParser };
        var combinedLogicalOperationsParser = logicalOperations.Aggregate((agg, current) => agg.Or(current));
        LogicalOperatorParser = (
            from logicalOperator in combinedLogicalOperationsParser
            select Unit.Value);

        ValueParser = (
            from leftTick in TickParser
            from value in ValueWithoutTicksParser
            from rightTick in TickParser
            select Unit.Value);
    }

    public static IEnumerable<Token<QueryToken>> Tokenize<TEntity>(
        IEnumerable<QueryFilterDefinition<TEntity>> filterDefinitions,
        string queryDefinition)
    {
        var propertyFieldNameParser = filterDefinitions.Aggregate((TextParser<TextSpan>)null, (agg, current) =>
        {
            var currentNameParser = Span.EqualToIgnoreCase(current.Name);
            return agg == null
                ? currentNameParser
                : agg.Or(currentNameParser);
        });

        var internalTokenizer = new TokenizerBuilder<QueryToken>()
            .Ignore(Span.WhiteSpace)
            .Match(LeftBracketParser, QueryToken.BracketLeft)
            .Match(RightBracketParser, QueryToken.BracketRight)
            .Match(propertyFieldNameParser, QueryToken.Property)
            .Match(OperatorParser, QueryToken.Operation)
            .Match(LogicalOperatorParser, QueryToken.LogicalOperation)
            .Match(ValueParser, QueryToken.Value)
            .Build();
        var result = internalTokenizer.TryTokenize(queryDefinition);

        if (result.HasValue)
            return result.Value;

        throw new Exception("Failed to parse query definition");
    }
}