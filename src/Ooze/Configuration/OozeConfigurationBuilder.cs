using System.Collections.Generic;
using System.Linq;

namespace Ooze.Configuration
{
    public class OozeConfigurationBuilder
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

        public OozeConfiguration Build()
        {
            return new OozeConfiguration
            {
                EntityConfigurations = _entityConfigurationBuilders.Select(config => config.Build())
                    .ToList()
            };
        }
    }
}
