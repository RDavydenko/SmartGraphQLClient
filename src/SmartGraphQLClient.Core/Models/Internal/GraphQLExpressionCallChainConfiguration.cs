using System.Linq.Expressions;

namespace SmartGraphQLClient.Core.Models.Internal
{
    internal class GraphQLExpressionCallChainConfiguration
    {
        private bool IsQueryConfiguration { get; set; } = true;
        private bool IsLocalConfigration => !IsQueryConfiguration;

        public List<ArgumentKeyValuePair> QueryArguments { get; private set; } = new();
        public List<IncludeExpressionNode> QueryIncludes { get; private set; } = new();
        public List<OrderExpression> QueryOrders { get; private set; } = new();
        public List<LambdaExpression> QueryConditions { get; private set; } = new();
        public LambdaExpression? QuerySelector { get; private set; } = null;
        public int? QueryTake { get; private set; } = null;
        public int? QuerySkip { get; private set; } = null;

        public List<LocalExpression> LocalExpressions { get; set; } = new();

        public void AddArgument(ArgumentKeyValuePair arg)
        {
            var argument = QueryArguments.FirstOrDefault(x => x.Key == arg.Key);
            if (argument is not null)
            {
                argument.SetValue(arg.Value);
            }
            else
            {
                argument = arg;
            }

            QueryArguments.Add(argument);
        }

        public void AddInclude(IncludeExpressionNode includeNode)
        {
            if (IsLocalConfigration)
            {
                throw new InvalidOperationException("Operation is not supported for local configuring");
            }
            
            QueryIncludes.Add(includeNode);
        }

        public void AddOrder(OrderExpression orderExpression)
        {
            if (IsQueryConfiguration)
            {
                QueryOrders.Add(orderExpression);
            }

            if (IsLocalConfigration)
            {
                LocalExpressions.Add(LocalExpression.CreateOrder(orderExpression));
            }
        }

        public void AddSelector(LambdaExpression selector)
        {
            if (IsQueryConfiguration)
            {
                if (QuerySelector is null)
                {
                    QuerySelector = selector;
                    IsQueryConfiguration = false;
                    return;
                }
                else
                {
                    throw new InvalidOperationException($"Operation is not supported for query configuring when {nameof(QuerySelector)} already exists");
                }
            }

            if (IsLocalConfigration)
            {
                LocalExpressions.Add(LocalExpression.CreateSelector(selector));
            }
        }

        public void AddCondition(LambdaExpression condition)
        {
            if (IsQueryConfiguration)
            {
                QueryConditions.Add(condition);
            }

            if (IsLocalConfigration)
            {
                LocalExpressions.Add(LocalExpression.CreateCondition(condition));
            }
        }

        public void AddTake(int count)
        {
            if (QueryTake is null && 
                !LocalExpressions.Any(m => m.Type == LocalExpression.ExpressionType.Condition))
            {
                QueryTake = count;
            }
            else
            {
                if (IsLocalConfigration)
                {
                    LocalExpressions.Add(LocalExpression.CreateTake(count));
                }
                else
                {
                    throw new InvalidOperationException("Operation is not supported for query configuring");
                }
            }
        }

        public void AddSkip(int count)
        {
            if (QuerySkip is null &&
                !LocalExpressions.Any(m => m.Type == LocalExpression.ExpressionType.Condition))
            {
                QuerySkip = count;
            }
            else
            {
                if (IsLocalConfigration)
                {
                    LocalExpressions.Add(LocalExpression.CreateSkip(count));
                }
                else
                {
                    throw new InvalidOperationException("Operation is not supported for query configuring");
                }
            }
        }

        public GraphQLExpressionCallChainConfiguration Clone()
            => new()
            {
                IsQueryConfiguration = this.IsQueryConfiguration,
                QueryArguments = new(this.QueryArguments),
                QueryIncludes = new(this.QueryIncludes),
                QueryOrders = new(this.QueryOrders),
                QueryConditions = new(this.QueryConditions),
                QuerySelector = this.QuerySelector,
                QueryTake = this.QueryTake,
                QuerySkip = this.QuerySkip,
                LocalExpressions = this.LocalExpressions.Select(x => x.Clone()).ToList(),
            };

        internal List<LocalExpression> GetEvaluateExpressions()
        {
            ArgumentNullException.ThrowIfNull(QuerySelector, nameof(QuerySelector));

            var expressions = new List<LocalExpression>(LocalExpressions);
            expressions.Insert(0, LocalExpression.CreateSelector(QuerySelector));
            return expressions;
        }

        internal void ChangeQuerySelector(LambdaExpression querySelector)
        {
            QuerySelector = querySelector;
        }
    }

    internal class LocalExpression
    {
        public ExpressionType Type { get; private set; }
        public LambdaExpression? Condition { get; private set; }
        public LambdaExpression? Selector { get; private set; }
        public OrderExpression? OrderExpression { get; private set; }
        public int? TakeCount { get; private set; }
        public int? SkipCount { get; private set; }

        private LocalExpression()
        {
        }

        public static LocalExpression CreateTake(int takeCount)
            => new() { Type = ExpressionType.Take, TakeCount = takeCount };
        public static LocalExpression CreateSkip(int skipCount)
            => new() { Type = ExpressionType.Skip, SkipCount = skipCount };
        public static LocalExpression CreateCondition(LambdaExpression condition)
            => new() { Type = ExpressionType.Condition, Condition = condition };
        public static LocalExpression CreateSelector(LambdaExpression selector)
            => new() { Type = ExpressionType.Selector, Selector = selector };
        public static LocalExpression CreateOrder(OrderExpression orderExpression)
            => new() { Type = ExpressionType.Order, OrderExpression = orderExpression };

        public LocalExpression Clone()
            => new()
            {
                Type = this.Type,
                Condition = this.Condition,
                Selector = this.Selector,
                OrderExpression = this.OrderExpression,
                TakeCount = this.TakeCount,
                SkipCount = this.SkipCount,
            };

        public enum ExpressionType
        {
            Selector,
            Condition,
            Order,
            Take,
            Skip,
        }
    }
}
