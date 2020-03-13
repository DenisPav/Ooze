using System.Collections.Generic;
using System.Linq;

namespace Ooze.Configuration
{
    internal class OozeConfigurationBuilder : IOozeConfigurationBuilder
    {
        readonly IList<IOozeEntityConfigurationBuilderInternal> _entityConfigurationBuilders =
            new List<IOozeEntityConfigurationBuilderInternal>();

        public IOozeEntityConfigurationBuilder<TEntity> Entity<TEntity>()
            where TEntity : class
        {
            var configurationInstance = OozeEntityConfigurationBuilder<TEntity>.Create();
            _entityConfigurationBuilders.Add(configurationInstance);

            return configurationInstance;
        }

        public OozeConfiguration Build(OozeOptions options)
        {
            return new OozeConfiguration(options)
            {
                EntityConfigurations = _entityConfigurationBuilders.Select(config => config.Build())
                    .ToDictionary(result => result.entityType, result => result.configuration)
            };
        }
    }
}
