using System;

namespace Ooze.Configuration
{
    public class OozeEntityConfiguration
    {
        public Type Type { get; set; }
        public Expressions Sorters { get; internal set; }
        public Expressions Filters { get; internal set; }
    }
}
