# MinimalApi.Endpoints

MinimalApi.Endpoints is a package that allows you to easily structure your Minimal Api Endpoints in individual classes instead of having them all in `Program.cs`.

## Installation

You can install MinimalApi.Endpoints via NuGet Package Manager in Visual Studio or by using the following command:

```
dotnet add package MinimalApi.Endpoints
```

## Usage

To use MinimalApi.Endpoints, simply call the `MapEndpoints` extension method in your `Program.cs`. Here's an example:

```csharp
using MinimalApi;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapEndpoints();

app.Run();
```


### Defining Endpoint Classes

To define an endpoint class, create a simple class that contains a method decorated with one of the following attributes to indicate the HTTP verb and route pattern for the endpoint:

- `EndpointGet` for GET requests
- `EndpointPost` for POST requests
- `EndpointPut` for PUT requests

Here's an example of how you can use the `EndpointGet` attribute to create an endpoint that returns a list of users:

```csharp
public class GetUsers
{
    [EndpointGet("/users")]
    public async Task<IEnumerable<User>> GetAsync(UserDb db)
    {
        return await db.Users.ToListAsync();
    }
}
```

Here's an example of how you can use the `EndpointGet` attribute to create an endpoint that returns a specific user:

```csharp
public class GetUser
{
    [EndpointGet("/users/{id}")]
    public async Task<User> GetAsync(int id, UserDb db)
    {
        return await db.Users.FindAsync(id);
    }
}
```

The method parameters can resolve to route/query/body values and registered services from the dependency container.

If you rather prefer to have all methods in one class:

```csharp
public class WannaBeUserController
{
    [EndpointGet("/users")]
    public async Task<IEnumerable<User>> GetAllAsync(UserDb db)
    {
        return await db.Users.ToListAsync();
    }

    [EndpointGet("/users/{id}")]
    public async Task<User> GetAsync(int id, UserDb db)
    {
        return await db.Users.FindAsync(id);
    }
}
```


### Limitations

- Only HTTP GET, POST, PUT methods are supported.
- PATCH and DELETE will be supported when .NET 8 is released.

## License

MinimalApi.Endpoints is licensed under the MIT license.