using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using static System.Linq.Expressions.Expression;

namespace Ooze.Expressions
{
    public interface IOozeConfiguration
    {
        void Configure(OozeConfigurationBuilder builder);
    }

    public class OozeConfigurationBuilder
    {
        readonly IList<IOozeEntityConfigurationBuilder> _entityConfigurationBuilders =
            new List<IOozeEntityConfigurationBuilder>();

        public OozeEntityConfigurationBuilder<TEntity> Entity<TEntity>()
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

    public class OozeConfiguration
    {
        public IEnumerable<OozeEntityConfiguration> EntityConfigurations { get; set; }
    }

    public interface IOozeEntityConfigurationBuilder
    {
        OozeEntityConfiguration Build();
    }

    public class OozeEntityConfigurationBuilder<TEntity> : IOozeEntityConfigurationBuilder
        where TEntity : class
    {
        readonly ParameterExpression Parameter = Parameter(typeof(TEntity), nameof(TEntity));
        readonly IList<Expression> _sorters = new List<Expression>();

        private OozeEntityConfigurationBuilder()
        {

        }

        public static OozeEntityConfigurationBuilder<TEntity> Create()
        {
            return new OozeEntityConfigurationBuilder<TEntity>();
        }

        public OozeEntityConfigurationBuilder<TEntity> Sort<TTarget>(
            Expression<Func<TEntity, TTarget>> sortExpression)
        {
            _sorters.Add(sortExpression);

            return this;
        }

        public OozeEntityConfiguration Build()
        {
            return new OozeEntityConfiguration
            {
                Type = typeof(TEntity),
                Sorters = new Expressions
                {
                    Param = Parameter,
                    LambdaExpressions = CreateSorters().ToList()
                }
            };
        }

        IEnumerable<(LambdaExpression, Type)> CreateSorters()
        {
            foreach (var sorter in _sorters)
            {
                var lambdaSorter = sorter as LambdaExpression;

                if (!(lambdaSorter.Body is MemberExpression))
                {
                    throw new Exception("Sorter definition not correct");
                }

                var memberName = (lambdaSorter.Body as MemberExpression).Member.Name;
                var prop = typeof(TEntity).GetProperty(memberName);
                var propType = prop.PropertyType;

                var memberAccess = MakeMemberAccess(Parameter, prop);
                var lambda = Lambda(memberAccess, Parameter);

                yield return (lambda, propType);
            }
        }
    }

    public class OozeEntityConfiguration
    {
        public Type Type { get; set; }
        public Expressions Sorters { get; internal set; }
    }

    public class Expressions
    {
        public ParameterExpression Param { get; internal set; }
        public IEnumerable<(LambdaExpression, Type)> LambdaExpressions { get; internal set; }
    }
}
