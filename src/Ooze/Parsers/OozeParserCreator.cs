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
        public static TextParser<FilterParserResult> FilterParser(
            IEnumerable<string> filterNames,
            IEnumerable<string> operationKeys)
        {
            var whiteSpaceParser = Character.WhiteSpace.Many();
            var filterNameParsers = filterNames.Select(name => Span.EqualToIgnoreCase(name).Between(whiteSpaceParser, whiteSpaceParser)).ToList();

            var propertyParser = filterNameParsers
                .Aggregate<TextParser<TextSpan>, TextParser<TextSpan>>(null, (accumulator, singlePropertyParser) =>
                {
                    if (accumulator == null)
                        return singlePropertyParser;

                    return accumulator.Or(singlePropertyParser);
                });

            var operationParsers = operationKeys.Select(Span.EqualToIgnoreCase).ToList();
            var operationParser = operationParsers.Aggregate<TextParser<TextSpan>, TextParser<TextSpan>>(null, (accumulator, singlePropertyParser) =>
            {
                if (accumulator == null)
                    return singlePropertyParser;

                return accumulator
                    .Try()
                    .Or(singlePropertyParser);
            });

            var valueParser = Span.WithAll(_ => true).OptionalOrDefault(new TextSpan(string.Empty));
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
            var filterNameParsers = filterNames.Select(name => Span.EqualToIgnoreCase(name).Between(whiteSpaceParser, whiteSpaceParser)).ToList();

            var propertyParser = filterNameParsers
                .Aggregate<TextParser<TextSpan>, TextParser<TextSpan>>(null, (accumulator, singlePropertyParser) =>
                {
                    if (accumulator == null)
                        return singlePropertyParser;

                    return accumulator.Or(singlePropertyParser);
                });

            var operationParsers = operations.Select(Span.EqualToIgnoreCase).ToList();
            var operationParser = operationParsers.Aggregate<TextParser<TextSpan>, TextParser<TextSpan>>(null, (accumulator, singlePropertyParser) =>
            {
                if (accumulator == null)
                    return singlePropertyParser;

                return accumulator
                    .Or(singlePropertyParser);
            });

            var logicalOpParsers = logicalOperations.Select(Span.EqualToIgnoreCase).ToList();
            var logicalOpParser = logicalOpParsers.Aggregate<TextParser<TextSpan>, TextParser<TextSpan>>(null, (accumulator, singlePropertyParser) =>
            {
                if (accumulator == null)
                    return singlePropertyParser;

                return accumulator
                    .Or(singlePropertyParser);
            }).Optional();

            //what about booleans
            var valueParser = Character.Except('\'').Many().Between(Character.EqualTo('\''), Character.EqualTo('\''))
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
    }
}
