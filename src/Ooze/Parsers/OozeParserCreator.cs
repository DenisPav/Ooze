using Ooze.Filters;
using Ooze.Query;
using Superpower;
using Superpower.Model;
using Superpower.Parsers;
using System.Collections.Generic;
using System.Linq;

namespace Ooze.Parsers
{
    internal static partial class OozeParserCreator
    {
        const char _stringContainerSymbol = '\'';

        public static TextParser<FilterParserResult> FilterParser(
            IEnumerable<string> filterNames,
            IEnumerable<string> operationKeys)
        {
            var propertyParser = CreateFor(filterNames);
            var operationParser = CreateFor(operationKeys);

            var valueParser = Span.WithAll(_ => true)
                .OptionalOrDefault(new TextSpan(string.Empty));
            var filterParser = (from property in propertyParser
                                from operation in operationParser
                                from value in valueParser
                                select new FilterParserResult(property.ToString(), operation.ToString(), value.ToString()));

            return filterParser;
        }

        public static TextParser<QueryParserResult[]> QueryParser(
            IEnumerable<string> filterNames,
            IEnumerable<string> operations,
            IEnumerable<string> logicalOperations)
        {
            //example: Property Operation Value LogicalOperator*
            //* optional
            var whiteSpaceParser = Character.WhiteSpace.Many();

            var propertyParser = CreateFor(filterNames);
            var operationParser = CreateFor(operations);
            var logicalOpParser = CreateFor(logicalOperations).Optional();

            //what about direct booleans without ''
            var containerSymbolParser = Character.EqualTo(_stringContainerSymbol);
            var valueParser = Character.Except(_stringContainerSymbol)
                .Many()
                .Between(containerSymbolParser, containerSymbolParser)
                .Try()
                .Or(Character.Numeric.Many());

            var queryParser = (from property in propertyParser
                               from operation in operationParser
                               from value in valueParser
                               from logicalOperation in logicalOpParser
                               select new QueryParserResult
                               {
                                   Property = property.ToString(),
                                   Operation = operation.ToString(),
                                   Value = new string(value),
                                   LogicalOperation = logicalOperation.ToString()
                               })
                               .Many();

            return queryParser;

        }

        static TextParser<TextSpan> CreateFor(IEnumerable<string> texts)
            => texts.Select(CreateTextParser)
                .Aggregate<TextParser<TextSpan>, TextParser<TextSpan>>(null, TryAggregate);

        static TextParser<TextSpan> TryAggregate(
            TextParser<TextSpan> accumulator,
            TextParser<TextSpan> parser) 
            => accumulator == null 
                ? parser
                : accumulator.Try().Or(parser);

        static TextParser<TextSpan> CreateTextParser(string text)
            => Span.WhiteSpace
                .Many()
                .IgnoreThen(Span.EqualToIgnoreCase(text));
    }
}
