namespace SmartGraphQLClient.Core.Utils
{
    internal static class TypeSystem
    {
        internal static Type GetElementType(Type seqType)
        {
            Type? ienum = FindIEnumerable(seqType);
            if (ienum is null) return seqType;
            return ienum.GetGenericArguments()[0];
        }

        private static Type? FindIEnumerable(Type seqType)
        {
            if (seqType is null || seqType == typeof(string))
                return null;
            if (seqType.IsArray)
                return typeof(IEnumerable<>).MakeGenericType(seqType.GetElementType()!);
            if (seqType.IsGenericType)
            {
                foreach (Type arg in seqType.GetGenericArguments())
                {
                    Type ienum = typeof(IEnumerable<>).MakeGenericType(arg);
                    if (ienum.IsAssignableFrom(seqType))
                    {
                        return ienum;
                    }
                }
            }
            Type[] ifaces = seqType.GetInterfaces();
            if (ifaces is not null && ifaces.Length > 0)
            {
                foreach (Type iface in ifaces)
                {
                    Type? ienum = FindIEnumerable(iface);
                    if (ienum is not null) return ienum;
                }
            }
            if (seqType.BaseType is not null && seqType.BaseType != typeof(object))
            {
                return FindIEnumerable(seqType.BaseType);
            }
            return null;
        }
    }
}
