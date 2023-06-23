using SmartGraphQLClient.Core.Utils;
using System.Reflection;

namespace SmartGraphQLClient.Core.Extensions
{
    internal static class ReflectionExstensions
    {
        public static Type GetMemberInfoType(this MemberInfo memberInfo)
        {
            return memberInfo switch
            {
                FieldInfo fieldInfo => fieldInfo.FieldType,
                PropertyInfo propertyInfo => propertyInfo.PropertyType,
                _ => throw new NotImplementedException($"{nameof(GetMemberInfoType)} from {memberInfo.GetType().Name} is not implemented"),
            };
        }

        public static bool IsSimpleType(this Type type)
        {
            var typeInfo = type.GetTypeInfo();
            if (typeInfo.IsGenericType && typeInfo.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                // nullable type, check if the nested type is simple.
                return IsSimpleType(typeInfo.GetGenericArguments()[0]);
            }
            return typeInfo.IsPrimitive ||
                   typeInfo.IsEnum ||
                   type == typeof(string) ||
                   type == typeof(decimal) ||
                   type == typeof(DateTime);
        }

        public static bool IsCollectionOfSimpleType(this Type type)
        {
            return type.IsCollectionType() && type.GetCollectionElementType().IsSimpleType();
        }

        public static bool IsCollectionType(this Type type)
        {
            return type != typeof(string) &&
                   type.GetInterface(nameof(System.Collections.IEnumerable)) is not null;
        }
        
        public static bool TryGetCollectionElementType(this Type collectionType, out Type elementType)
        {
            elementType = default!;

            if (!collectionType.IsCollectionType())
            {
                return false;
            }
            
            elementType = TypeSystem.GetElementType(collectionType);
            return true;
        }

        public static Type GetCollectionElementType(this Type collectionType)
        {
            return TypeSystem.GetElementType(collectionType);
        }
    }
}
