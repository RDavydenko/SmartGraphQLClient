using System.Collections;
using System.Text;
using SmartGraphQLClient.Core.Providers.Abstractions;

namespace SmartGraphQLClient.Core.Models.Internal;

public class WrappedEnumerable
{
    private readonly IEnumerable _source;
    private readonly IGraphQLValueFormatProvider _valueFormatProvider;

    public WrappedEnumerable(
        IEnumerable source,
        IGraphQLValueFormatProvider valueFormatProvider)
    {
        _source = source;
        _valueFormatProvider = valueFormatProvider;
    }

    public override string ToString()
    {
        var hasLastComma = false;

        var sb = new StringBuilder();
        sb.Append("[ ");

        foreach (var item in _source)
        {
            sb.Append($" {_valueFormatProvider.GetFormattedValue(item)},");
            hasLastComma = true;
        }

        if (hasLastComma)
        {
            sb.Remove(sb.Length - 1, 1);
        }

        sb.Append(" ]");

        return sb.ToString();
    }
}