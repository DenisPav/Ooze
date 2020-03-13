using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using static System.Linq.Expressions.Expression;

namespace Ooze.Configuration
{
    internal class OozeConfiguration
    {
        readonly OozeOptions _options;
        public readonly IReadOnlyDictionary<string, Operation> OperationsMap;

        public OozeConfiguration(
            OozeOptions options)
        {
            _options = options ?? throw new Exception("Ooze options not registered to container");

            OperationsMap = new Dictionary<string, Operation>
            {
                { _options.Operations.Equal, Equal },
                { _options.Operations.NotEqual, NotEqual },
                { _options.Operations.GreaterThanOrEqual, GreaterThanOrEqual },
                { _options.Operations.LessThanOrEqual, LessThanOrEqual },
                { _options.Operations.StartsWith, Expressions.StartsWith },
                { _options.Operations.EndsWith, Expressions.EndsWith },
                { _options.Operations.GreaterThan, GreaterThan },
                { _options.Operations.LessThan, LessThan },
                { _options.Operations.Contains, Expressions.Contains },
            };
        }

        public delegate Expression Operation(Expression left, Expression right);
        public IDictionary<Type, OozeEntityConfiguration> EntityConfigurations { get; set; }
    }

    public class OozeOptions
    {
        public OozeOperations Operations { get; set; } = new OozeOperations();
    }

    public class OozeOperations
    {
        string _equal = "==";
        string _notEqual = "!=";
        string _greaterThanOrEqual = ">=";
        string _lessThanOrEqual = "<=";
        string _startsWith = "@=";
        string _endsWith = "=@";
        string _greaterThan = ">";
        string _lessThan = "<";
        string _contains = "@";

        public string Equal
        {
            get => _equal;
            set
            {
                ValidateOperation(value);

                _equal = value;
            }
        }

        public string NotEqual
        {
            get => _notEqual;
            set
            {
                ValidateOperation(value);

                _notEqual = value;
            }
        }

        public string GreaterThanOrEqual
        {
            get => _greaterThanOrEqual;
            set
            {
                ValidateOperation(value);

                _greaterThanOrEqual = value;
            }
        }

        public string LessThanOrEqual
        {
            get => _lessThanOrEqual;
            set
            {
                ValidateOperation(value);

                _lessThanOrEqual = value;
            }
        }

        public string StartsWith
        {
            get => _startsWith;
            set
            {
                ValidateOperation(value);

                _startsWith = value;
            }
        }

        public string EndsWith
        {
            get => _endsWith;
            set
            {
                ValidateOperation(value);

                _endsWith = value;
            }
        }

        public string GreaterThan
        {
            get => _greaterThan;
            set
            {
                ValidateOperation(value);

                _greaterThan = value;
            }
        }

        public string LessThan
        {
            get => _lessThan;
            set
            {
                ValidateOperation(value);

                _lessThan = value;
            }
        }

        public string Contains
        {
            get => _contains;
            set
            {
                ValidateOperation(value);

                _contains = value;
            }
        }

        static void ValidateOperation(string value)
        {
            if (value.Where(@char => !char.IsLetterOrDigit(@char)).Count() != value.Count())
            {
                throw new Exception("Symbols can only be used as operators");
            }
        }
    }
}
