using System.Collections.Generic;

namespace Ooze.Selections
{
    internal class FieldDefinition
    {
        public string Property { get; set; }
        public IList<FieldDefinition> Children { get; set; } = new List<FieldDefinition>();

        public FieldDefinition(string property) => Property = property;
    }
}
