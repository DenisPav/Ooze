using System.Linq.Expressions;

namespace Ooze.Configuration
{
    internal class SorterExpression
    {
        public string Name { get; set; }
        public Expression Sorter { get; set; }
    }
}
