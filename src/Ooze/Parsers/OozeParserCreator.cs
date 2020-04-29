using Ooze.Filters;
using Ooze.Query;
using Ooze.Selections;
using Ooze.Sorters;
using Superpower;
using Superpower.Model;
using Superpower.Parsers;
using System;
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

        public static TextParser<SorterParserResult> SorterParser(char negativeOrderChar)
            => input =>
            {
                var sorter = input.ToStringValue()
                    .Trim();

                var result = new SorterParserResult
                {
                    Ascending = !sorter.StartsWith(negativeOrderChar) ? true : false,
                    Sorter = sorter.StartsWith(negativeOrderChar) ? new string(sorter.Skip(1).ToArray()) : sorter
                };

                return Result.Value(result, input, TextSpan.Empty);
            };

        public static TextParser<QueryParserResult[]> QueryParser(
            IEnumerable<string> filterNames,
            IEnumerable<string> operations,
            IEnumerable<string> logicalOperations)
        {
            //example: Property Operation Value LogicalOperator*
            //* optional
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
                               .AtLeastOnce();

            return queryParser;

        }

        public static TextParser<IEnumerable<FieldDefinition>> SelectionParser(
            char fieldDelimiter,
            char separator = ',')
            => input =>
            {
                var fields = input.ToStringValue()
                    .Trim()
                    .Split(separator)
                    .OrderByDescending(def => def.Count(@char => @char == fieldDelimiter))
                    .ToList();

                var parsed = ParseSelections(fields);
                return Result.Value(parsed, input, TextSpan.Empty);
            };

        static IEnumerable<FieldDefinition> ParseSelections(
            IEnumerable<string> fieldDefinitions,
            IList<FieldDefinition> existingDefinitions = null)
        {
            existingDefinitions ??= new List<FieldDefinition>();

            foreach (var fieldDefinition in fieldDefinitions)
            {
                if (!fieldDefinition.Contains('.')
                    && !existingDefinitions.Any(def => def.Property.Equals(fieldDefinition, StringComparison.InvariantCultureIgnoreCase)))
                {
                    existingDefinitions.Add(new FieldDefinition(fieldDefinition));
                }
                else
                {
                    var separatorIndex = fieldDefinition.IndexOf('.');
                    var containerPart = fieldDefinition.Remove(separatorIndex);
                    var subPart = fieldDefinition.Substring(separatorIndex + 1);
                    var containerDef = existingDefinitions.FirstOrDefault(def => def.Property.Equals(containerPart, StringComparison.InvariantCultureIgnoreCase));

                    if (containerDef is null)
                    {
                        containerDef = new FieldDefinition(containerPart);
                        existingDefinitions.Add(containerDef);
                    }

                    containerDef.Children = ParseSelections(new[] { subPart }, containerDef.Children).ToList();
                }
            }

            return existingDefinitions;
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
