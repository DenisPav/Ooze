using Ooze.Configuration;

namespace Ooze.Query
{
    internal class QueryFilterOperation
    {
        public ParsedExpressionDefinition Filter { get; set; }
        public Operation OperationFactory { get; set; }
        public Operation LogicalOperationFactory { get; set; }
        public QueryParserResult QueryPart { get; set; }
    }
}
