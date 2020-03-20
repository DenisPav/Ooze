using Ooze.Configuration;
using Superpower;
using Superpower.Model;
using Superpower.Parsers;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;

namespace Ooze.Filters
{
    internal static class OozeParserCreator
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

        public static TextParser<QueryParserResult[]> QueryParser(OozeEntityConfiguration entityConfiguration)
        {
            //example: Property Operation Value LogicalOperator*
            //* optional

            var filterNames = entityConfiguration.Filters.Select(filter => filter.Name);
            var whiteSpaceParser = Character.WhiteSpace.Many();
            var filterNameParsers = filterNames.Select(name => Span.EqualToIgnoreCase(name).Between(whiteSpaceParser, whiteSpaceParser)).ToList();

            var propertyParser = filterNameParsers
                .Aggregate<TextParser<TextSpan>, TextParser<TextSpan>>(null, (accumulator, singlePropertyParser) =>
                {
                    if (accumulator == null)
                        return singlePropertyParser;

                    return accumulator.Or(singlePropertyParser);
                });

            //pass same operations as for normal filters
            var operations = new[]
            {
                ">",
                "<",
                "==",
                "!="
            };

            var operationParsers = operations.Select(Span.EqualToIgnoreCase).ToList();
            var operationParser = operationParsers.Aggregate<TextParser<TextSpan>, TextParser<TextSpan>>(null, (accumulator, singlePropertyParser) =>
            {
                if (accumulator == null)
                    return singlePropertyParser;

                return accumulator
                    .Or(singlePropertyParser);
            });

            var logicalOperations = new[]
            {
                "AND",
                "OR"
            };
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

        public class QueryParserResult
        {
            public string Property { get; set; }
            public string Operation { get; set; }
            public string Value { get; set; }
            public string LogicalOperation { get; set; }
        }
    }
}
