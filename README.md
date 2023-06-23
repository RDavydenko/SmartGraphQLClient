# SmartGraphQLClient
Back-to-back GraphQL client using Linq-syntax

## Install NuGet package
```
Install-Package SmartGraphQLClient
```

## Usage

### 1. Add required services to DI
``` csharp
using SmartGraphQLClient;

services.AddSmartGraphQLClient();
```

### 2. Write GraphQL-request with Linq-syntax
``` csharp
using SmartGraphQLClient;

GraphQLHttpClient client = ... // from DI

var users = await client.Query<UserModel>("users")
    .Include(x => x.Roles)
    .ThenInclude(x => x.Users)
    .Where(x => x.UserName.StartsWith("A") || x.Roles.Any(r => r.Code == RoleCode.ADMINISTRATOR))
    .Select(x => new 
    {
        x.Id,
        Name = x.UserName,
        x.Roles,
        IsAdministrator = x.Roles.Any(r => r.Code == RoleCode.ADMINISTRATOR)
    })
    .Skip(5)
    .Take(10)
    .Argument("secretKey", "1234")
    .ToListAsync();

```

## Explanation
When you call materializing method (`ToListAsync()`, `ToArrayAsync()`, `FirstAsync()`, etc.), the `GraphQLHttpClient` will build a string query and send it to the GraphQL-server, get a response and materialize the result.

Query-string from example:
``` graphql
{ 
    users (
      where: {
        or: [ 
          { userName: { startsWith: "A" } }
          { roles: { some: { code: { eq: ADMINISTRATOR } } } }
        ]
      }
      skip: 5
      take: 10
      secretKey: "1234"
  ) {
        id
        userName
        roles {
            code
            name
            description
            id
            users {
                userName
                age
                id
            }
        }
    }
}
```

## More examples
See more examples in unit-tests [directory](https://github.com/RDavydenko/SmartGraphQLClient/tree/master/src/SmartGraphQLClient.Tests/Core/GraphQLHttpClient): 
1) more requests (`ToListAsync()`, `ToPageAsync()`, `FirstOrDefaultAsync()`)
2) attributes (`GraphQLEndpointAttribute`, `GraphQLIgnoreAttribute`, `GraphQLPropertyNameAttribute`)
3) authorized graphql-http-client (constant token and `IGraphQLAuthorizationService<>` for providing tokens in runtime)
4) and more

## Compatibility
Requests were tested on [HotChocolate](https://github.com/ChilliCream/graphql-platform/) GraphQL-server. See their [documentation](https://chillicream.com/docs/hotchocolate/v13).

## Features
| feature's name             |    |
|----------------------------|----|
| Build Where-string         | ✅ |
| Build Select-string        | ✅ |
| Offset paging (skip, take) | ✅ |
| Custom arguments           | ✅ |
| Cursor paging              | ❌ |

## Links
https://www.nuget.org/packages/SmartGraphQLClient/
