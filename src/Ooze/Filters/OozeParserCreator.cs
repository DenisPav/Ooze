using Superpower;
using Superpower.Model;
using Superpower.Parsers;
using System.Collections.Generic;
using System.Linq;

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
    }
}
