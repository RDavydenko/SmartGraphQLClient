using System.Reflection;

namespace SmartGraphQLClient.Core.Utils
{
    internal static class LinqMethods
    {
        internal static MethodInfo GetSelectMethod(Type typeIn, Type typeOut)
        {
            var method = typeof(Enumerable)
                .GetMethods(BindingFlags.Static | BindingFlags.Public)
                .Single(m => m.Name == "Select" &&
                            m.IsGenericMethod &&
                            m.GetParameters().Length == 2 &&
                            m.GetParameters()[1].ParameterType.IsGenericType &&
                            m.GetParameters()[1].ParameterType.GetGenericTypeDefinition() == typeof(Func<,>));

            return method.MakeGenericMethod(typeIn, typeOut);
        }

        internal static MethodInfo GetWhereMethod(Type typeIn)
        {
            var method = typeof(Enumerable)
                .GetMethods(BindingFlags.Static | BindingFlags.Public)
                .Single(m => m.Name == "Where" &&
                             m.IsGenericMethod &&
                             m.GetGenericArguments().Length == 1 &&
                             m.GetParameters().Length == 2 &&
                             m.GetParameters()[1].ParameterType.IsGenericType &&
                             m.GetParameters()[1].ParameterType.GetGenericTypeDefinition() == typeof(Func<,>));

            return method.MakeGenericMethod(typeIn);
        }

        internal static MethodInfo GetFirstMethod(Type typeIn)
        {
            var method = typeof(Enumerable)
                .GetMethods(BindingFlags.Static | BindingFlags.Public)
                .Single(m => m.Name == "First" && 
                             m.IsGenericMethod && 
                             m.GetGenericArguments().Length == 1 && 
                             m.GetParameters().Length == 1);

            return method.MakeGenericMethod(typeIn);
        }

        internal static MethodInfo GetToArrayMethod(Type type)
        {
            var method = typeof(Enumerable)
                .GetMethods(BindingFlags.Static | BindingFlags.Public)
                .Single(m => m.Name == "ToArray" && 
                             m.IsGenericMethod && 
                             m.GetGenericArguments().Length == 1 && 
                             m.GetParameters().Length == 1);

            return method.MakeGenericMethod(type);
        }

        internal static MethodInfo GetTakeMethod(Type type)
        {
            var method = typeof(Enumerable)
                .GetMethods(BindingFlags.Static | BindingFlags.Public)
                .Single(m => m.Name == "Take" && 
                             m.IsGenericMethod && 
                             m.GetGenericArguments().Length == 1 && 
                             m.GetParameters().Length == 2 &&
                             m.GetParameters()[1].ParameterType == typeof(int));

            return method.MakeGenericMethod(type);
        }

        internal static MethodInfo GetSkipMethod(Type typeIn)
        {
            var method = typeof(Enumerable)
                .GetMethods(BindingFlags.Static | BindingFlags.Public)
                .Single(m => m.Name == "Skip" && 
                             m.IsGenericMethod && 
                             m.GetGenericArguments().Length == 1 && 
                             m.GetParameters().Length == 2 &&
                             m.GetParameters()[1].ParameterType == typeof(int));

            return method.MakeGenericMethod(typeIn);
        }

        internal static MethodInfo GetOrderByMethod(Type typeIn, Type propertyType)
        { 
            var method = typeof(Enumerable)
                .GetMethods(BindingFlags.Static | BindingFlags.Public)
                .Single(m => m.Name == "OrderBy" && 
                             m.IsGenericMethod && 
                             m.GetGenericArguments().Length == 2 && 
                             m.GetParameters().Length == 2);

            return method.MakeGenericMethod(typeIn, propertyType);
        }

        internal static MethodInfo GetOrderByDescendingMethod(Type typeIn, Type propertyType)
        {
            var method = typeof(Enumerable)
                .GetMethods(BindingFlags.Static | BindingFlags.Public)
                .Single(m => m.Name == "OrderByDescending" && 
                             m.IsGenericMethod && 
                             m.GetGenericArguments().Length == 2 && 
                             m.GetParameters().Length == 2);

            return method.MakeGenericMethod(typeIn, propertyType);
        }
    }
}
