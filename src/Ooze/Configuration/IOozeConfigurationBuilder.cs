namespace Ooze.Configuration
{
    public interface IOozeConfigurationBuilder
    {
        IOozeEntityConfigurationBuilder<TEntity> Entity<TEntity>() where TEntity : class;
    }
}
