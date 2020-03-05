﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using static System.Linq.Expressions.Expression;

namespace Ooze.Expressions
{
    public interface IOozeConfiguration
    {
        void Configure(OozeConfigurationBuilder builder);
    }

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

    public class OozeConfiguration
    {
        public IEnumerable<OozeEntityConfiguration> EntityConfigurations { get; set; }
    }

    internal interface IOozeEntityConfigurationBuilderInternal
    {
        OozeEntityConfiguration Build();
    }

    public interface IOozeEntityConfigurationBuilder<TEntity>
        where TEntity : class
    {
        OozeEntityConfigurationBuilder<TEntity> Sort<TTarget>(string sorterName, Expression<Func<TEntity, TTarget>> sortExpression);
        OozeEntityConfigurationBuilder<TEntity> Sort<TTarget>(Expression<Func<TEntity, TTarget>> sortExpression);
    }

    internal class SorterExpression
    {
        public string Name { get; set; }
        public Expression Sorter { get; set; }
    }

    public class OozeEntityConfigurationBuilder<TEntity>
        : IOozeEntityConfigurationBuilderInternal, IOozeEntityConfigurationBuilder<TEntity>
        where TEntity : class
    {
        readonly ParameterExpression Parameter = Parameter(typeof(TEntity), nameof(TEntity));
        readonly IList<SorterExpression> _sorters = new List<SorterExpression>();

        private OozeEntityConfigurationBuilder()
        { }

        public static OozeEntityConfigurationBuilder<TEntity> Create()
        {
            return new OozeEntityConfigurationBuilder<TEntity>();
        }

        public OozeEntityConfigurationBuilder<TEntity> Sort<TTarget>(
            string sorterName,
            Expression<Func<TEntity, TTarget>> sortExpression)
        {
            _sorters.Add(new SorterExpression
            {
                Name = sorterName,
                Sorter = sortExpression
            });

            return this;
        }

        public OozeEntityConfigurationBuilder<TEntity> Sort<TTarget>(
            Expression<Func<TEntity, TTarget>> sortExpression)
        {
            if (!(sortExpression.Body is MemberExpression))
            {
                throw new Exception("Sorter definition not correct");
            }

            var memberName = (sortExpression.Body as MemberExpression).Member.Name;
            return Sort(memberName, sortExpression);
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

        IEnumerable<(string, LambdaExpression, Type)> CreateSorters()
        {
            foreach (var sorter in _sorters)
            {
                var lambdaSorter = sorter.Sorter as LambdaExpression;

                if (!(lambdaSorter.Body is MemberExpression))
                {
                    throw new Exception("Sorter definition not correct");
                }

                var memberName = (lambdaSorter.Body as MemberExpression).Member.Name;
                var prop = typeof(TEntity).GetProperty(memberName);
                var propType = prop.PropertyType;

                var memberAccess = MakeMemberAccess(Parameter, prop);
                var lambda = Lambda(memberAccess, Parameter);

                yield return (sorter.Name, lambda, propType);
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
        public IEnumerable<(string, LambdaExpression, Type)> LambdaExpressions { get; internal set; }
    }
}
