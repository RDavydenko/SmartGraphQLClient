using System.Reflection;

namespace SmartGraphQLClient.Core.Utils
{
    internal static class ReflectionHelper
    {
        public static object? GetValueByPath(object? instance, Stack<MemberInfo> path)
        {
            var value = instance;

            while (path.Any())
            {
                var memberInfo = path.Pop();
                value = GetValueFromMemberInfo(value, memberInfo);
            }

            return value;
        }

        private static object? GetValueFromMemberInfo(object? value, MemberInfo memberInfo)
        {
            return memberInfo switch
            {
                FieldInfo fieldInfo => fieldInfo.GetValue(value),
                PropertyInfo propertyInfo => propertyInfo.GetValue(value),
                //MemberTypes.Method => throw new NotImplementedException(),
                _ => throw new NotImplementedException($"{nameof(GetValueFromMemberInfo)} from {memberInfo.GetType().Name} is not implemented"),
            };
        }


    }
}
