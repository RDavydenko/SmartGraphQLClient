using SmartGraphQLClient.Errors;

namespace SmartGraphQLClient.Exceptions
{
    public class GraphQLException : Exception
    {
        private const string DefaultErrorMessage = "An exception occurred while executing a GraphQL request";

        public GraphQLException(GraphQLError[] errors)
            : base(DefaultErrorMessage)
        {
            Errors = errors;
        }

        public GraphQLError[] Errors { get; }
    }
}
