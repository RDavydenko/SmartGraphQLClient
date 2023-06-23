namespace SmartGraphQLClient.Tests.TestsInfrastructure
{
    public class TestClosureClass
    {
        public static int ValueStatic;

        public int value;

        public TestClosureClass(int value)
        {
            Value = value;
            this.value = value;
            ValueStatic = value;
        }

        public int Value { get; }

        public int GetValue() => Value;
        public static int GetValueStatic() => ValueStatic;

        public int GetCaluclatedValue(int v1, int v2, int v3)
        {
            return v1 + v2 + v3;
        }

        public int GetCalulatedValue(int v1, int v2)
        {
            return v1 + v2;
        }
    }
}
