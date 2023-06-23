using SmartGraphQLClient.Core.GraphQLQueryable.Internal;
using System.Reflection;

namespace SmartGraphQLClient.Core.GraphQLQueryable.Utils
{
    internal static class GraphQLQueryableMethods
    {
        internal static readonly Type GraphQLQueryableType
            = typeof(GraphQLQueryable<>);

        internal static readonly Type IncludableGraphQLQueryableType
            = typeof(IncludableGraphQLQueryable<,>);

        internal static readonly Type OrderableGraphQLQueryableType
            = typeof(OrderedGraphQLQueryable<,>);

        internal static Type GetGraphQLQueryableGenericType(
            Type entityType)
            => GraphQLQueryableType.MakeGenericType(entityType);

        internal static Type GetIncludableGraphQLQueryableGenericType(
            Type entityType, Type propertyType)
            => IncludableGraphQLQueryableType.MakeGenericType(entityType, propertyType);

        internal static Type GetOrderedGraphQLQueryableGenericType(
            Type entityType, Type propertyType)
            => OrderableGraphQLQueryableType.MakeGenericType(entityType, propertyType);

        internal static MethodInfo GetSelectMethod(Type entityType, Type resultType)
            => GetGraphQLQueryableGenericType(entityType)
                .GetMethods(BindingFlags.NonPublic | BindingFlags.Instance)
                .Single(m => m.Name == "Select" && m.GetGenericArguments().Length == 1 && m.GetParameters().Length == 1)
                .MakeGenericMethod(resultType);

        internal static MethodInfo GetWhereMethod(Type entityType)
            => GetGraphQLQueryableGenericType(entityType)
                .GetMethods(BindingFlags.NonPublic | BindingFlags.Instance)
                .Single(m => m.Name == "Where" && m.GetGenericArguments().Length == 0 && m.GetParameters().Length == 1);

        internal static MethodInfo GetSkipMethod(Type entityType)
            => GetGraphQLQueryableGenericType(entityType)
                .GetMethods(BindingFlags.NonPublic | BindingFlags.Instance)
                .Single(m => m.Name == "Skip" && m.GetGenericArguments().Length == 0 && m.GetParameters().Length == 1);

        internal static MethodInfo GetTakeMethod(Type entityType)
            => GetGraphQLQueryableGenericType(entityType)
                .GetMethods(BindingFlags.NonPublic | BindingFlags.Instance)
                .Single(m => m.Name == "Take" && m.GetGenericArguments().Length == 0 && m.GetParameters().Length == 1);

        internal static MethodInfo GetArgumentMethod(Type entityType)
            => GetGraphQLQueryableGenericType(entityType)
                .GetMethods(BindingFlags.NonPublic | BindingFlags.Instance)
                .Single(m => m.Name == "Argument" && m.GetGenericArguments().Length == 0 && m.GetParameters().Length == 2);

        internal static MethodInfo GetToArrayAsyncMethod(Type entityType)
            => GetGraphQLQueryableGenericType(entityType)
                .GetMethods(BindingFlags.NonPublic | BindingFlags.Instance)
                .Single(m => m.Name == "ToArrayAsync" && m.GetGenericArguments().Length == 0 && m.GetParameters().Length == 2);

        internal static MethodInfo GetToListAsyncMethod(Type entityType)
            => GetGraphQLQueryableGenericType(entityType)
                .GetMethods(BindingFlags.NonPublic | BindingFlags.Instance)
                .Single(m => m.Name == "ToListAsync" && m.GetGenericArguments().Length == 0 && m.GetParameters().Length == 2);

        internal static MethodInfo GetToPageAsyncMethod(Type entityType)
            => GetGraphQLQueryableGenericType(entityType)
                .GetMethods(BindingFlags.NonPublic | BindingFlags.Instance)
                .Single(m => m.Name == "ToPageAsync" && m.GetGenericArguments().Length == 0 && m.GetParameters().Length == 2);

        internal static MethodInfo GetFirstOrDefaultAsyncMethod(Type entityType)
            => GetGraphQLQueryableGenericType(entityType)
                .GetMethods(BindingFlags.NonPublic | BindingFlags.Instance)
                .Single(m => m.Name == "FirstOrDefaultAsync" && m.GetGenericArguments().Length == 0 && m.GetParameters().Length == 2);

        internal static MethodInfo GetFirstAsyncMethod(Type entityType)
            => GetGraphQLQueryableGenericType(entityType)
                .GetMethods(BindingFlags.NonPublic | BindingFlags.Instance)
                .Single(m => m.Name == "FirstAsync" && m.GetGenericArguments().Length == 0 && m.GetParameters().Length == 2);

        internal static MethodInfo GetIncludeMethod(Type entityType, Type propertyType)
            => GetGraphQLQueryableGenericType(entityType)
                .GetMethods(BindingFlags.NonPublic | BindingFlags.Instance)
                .Single(m => m.Name == "Include" && m.GetGenericArguments().Length == 1 && m.GetParameters().Length == 1)
                .MakeGenericMethod(propertyType);

        internal static MethodInfo GetThenIncludeMethod(Type entityType, Type previousPropertyType, Type propertyType)
            => GetIncludableGraphQLQueryableGenericType(entityType, previousPropertyType)
                .GetMethods(BindingFlags.NonPublic | BindingFlags.Instance)
                .Single(m => m.Name == "ThenInclude" && m.GetGenericArguments().Length == 1 && m.GetParameters().Length == 1)
                .MakeGenericMethod(propertyType);
        internal static MethodInfo GetOrderByMethod(Type entityType, Type propertyType)
            => GetGraphQLQueryableGenericType(entityType)
                .GetMethods(BindingFlags.NonPublic | BindingFlags.Instance)
                .Single(m => m.Name == "OrderBy" && m.GetGenericArguments().Length == 1 && m.GetParameters().Length == 1)
                .MakeGenericMethod(propertyType);

        internal static MethodInfo GetOrderByDescendingMethod(Type entityType, Type propertyType)
            => GetGraphQLQueryableGenericType(entityType)
                .GetMethods(BindingFlags.NonPublic | BindingFlags.Instance)
                .Single(m => m.Name == "OrderByDescending" && m.GetGenericArguments().Length == 1 && m.GetParameters().Length == 1)
                .MakeGenericMethod(propertyType);

        internal static MethodInfo GetThenByMethod(Type entityType, Type previousPropertyType, Type propertyType)
            => GetOrderedGraphQLQueryableGenericType(entityType, previousPropertyType)
                .GetMethods(BindingFlags.NonPublic | BindingFlags.Instance)
                .Single(m => m.Name == "ThenBy" && m.GetGenericArguments().Length == 1 && m.GetParameters().Length == 1)
                .MakeGenericMethod(propertyType);

        internal static MethodInfo GetThenByDescendingMethod(Type entityType, Type previousPropertyType, Type propertyType)
            => GetOrderedGraphQLQueryableGenericType(entityType, previousPropertyType)
                .GetMethods(BindingFlags.NonPublic | BindingFlags.Instance)
                .Single(m => m.Name == "ThenByDescending" && m.GetGenericArguments().Length == 1 && m.GetParameters().Length == 1)
                .MakeGenericMethod(propertyType);

        internal static MethodInfo GetConfigureMethod(Type entityType)
            => GetGraphQLQueryableGenericType(entityType)
                .GetMethods(BindingFlags.NonPublic | BindingFlags.Instance)
                .Single(m => m.Name == "Configure" && m.GetGenericArguments().Length == 0 && m.GetParameters().Length == 2);
    }
}
