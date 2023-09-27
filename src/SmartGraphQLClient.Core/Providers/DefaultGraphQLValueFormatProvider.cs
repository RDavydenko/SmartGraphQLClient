using SmartGraphQLClient.Core.Extensions;
using SmartGraphQLClient.Core.Providers.Abstractions;

namespace SmartGraphQLClient.Core.Providers
{
    internal class DefaultGraphQLValueFormatProvider : IGraphQLValueFormatProvider
    {
        public string GetFormattedValue(object? value)
        {
            if (value is null) return "null";

            if (value is bool bValue) return bValue ? "true" : "false";

            if (value is string sValue)
            {
                return EscapeStringValue(sValue);
            }

            if (value is DateTime dtValue) return dtValue.ToUniversalIso8601();

            return value.ToString() ?? string.Empty;
        }

        private string EscapeStringValue(string value)
        {
            value = value
                .Replace(@"\", @"\\")
                .Replace("\"", "\\\"")
                .Replace("\n", @"\n")
                .Replace("\r", @"\r")
                .Replace("\t", @"\t")
                .Replace("\0", @"\0")
                .Replace("\b", @"\b");
            
            return $"\"{value}\"";
        }
    }
}
