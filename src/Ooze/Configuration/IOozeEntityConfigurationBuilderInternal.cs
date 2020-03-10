using System;

namespace Ooze.Configuration
{
    internal interface IOozeEntityConfigurationBuilderInternal
    {
        (Type entityType, OozeEntityConfiguration configuration) Build();
    }
}
