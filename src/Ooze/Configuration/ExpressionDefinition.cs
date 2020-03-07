using System.Linq.Expressions;

namespace Ooze.Configuration
{
    internal class ExpressionDefinition
    {
        public string Name { get; set; }
        public Expression Expression { get; set; }
    }
}
