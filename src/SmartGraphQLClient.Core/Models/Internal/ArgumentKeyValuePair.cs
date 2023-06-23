namespace SmartGraphQLClient.Core.Models.Internal
{
    internal class ArgumentKeyValuePair
    {
        public ArgumentKeyValuePair(string key, object? value)
        {
            Key = key;
            Value = value;
        }

        public string Key { get; }
        public object? Value { get; private set; }

        internal void SetValue(object? value)
            => Value = value;

        internal ArgumentKeyValuePair Clone()
            => new(Key, Value);
    }
}
