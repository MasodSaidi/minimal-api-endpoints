# MinimalApi.Endpoints

MinimalApi.Endpoints is a package that allows you to easily structure your Minimal Api Endpoints in individual classes instead of having them all in `Program.cs`.

## Installation

You can install MinimalApi.Endpoints via NuGet Package Manager in Visual Studio or by using the following command:

```
dotnet add package MinimalApi.Endpoints
```

## Usage

Simply call the `MapEndpoints` extension method in your `Program.cs`:

```csharp #6
using MinimalApi;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapEndpoints();

app.Run();
```

Then create a simple class that contains a method decorated with `EndpointGet` attribute:

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

You are free to put the class anywhere in your project, with any name and you can name the method whatever you like.
Only requirement is to use appropriate Endpoint attribute.

### Supported Endpoint attributes

- `EndpointGet` for GET requests
- `EndpointPost` for POST requests
- `EndpointPut` for PUT requests

### More examples

Here are some examples of using EndpointPost and EndpointPut:

```csharp
public class CreateUser
{
    [EndpointPost("/users")]
    public async Task<User> PostAsync(User user, UserDb db)
    {
        db.Users.Add(user);
        await db.SaveChangesAsync();
        return user;
    }
}
```

```csharp
public class UpdateUser
{
    [EndpointPut("/users/{id}")]
    public async Task<User> PutAsync(int id, User updatedUser, UserDb db)
    {
        var user = await db.Users.FindAsync(id);
        user.Name = updatedUser.Name;
        user.Email = updatedUser.Email;
        await db.SaveChangesAsync();
        return user;
    }
}
```

### I want Controllers!

If you rather prefer to have all methods in one class:

```csharp
public class WannaBeUserController
{
    [EndpointGet("/users/{id}")]
    public async Task<User> GetAsync(int id, UserDb db)
    {
        return await db.Users.FindAsync(id);
    }

    [EndpointPost("/users")]
    public async Task<User> PostAsync(User user, UserDb db)
    {
        db.Users.Add(user);
        await db.SaveChangesAsync();
        return user;
    }

    [EndpointPut("/users/{id}")]
    public async Task<User> PutAsync(int id, User updatedUser, UserDb db)
    {
        var user = await db.Users.FindAsync(id);
        user.Name = updatedUser.Name;
        user.Email = updatedUser.Email;
        await db.SaveChangesAsync();
        return user;
    }
}
```

## Method parameters

The method parameters can resolve to route/query/body values and registered services from the dependency container.

## Limitations

- Only HTTP GET, POST, PUT methods are supported.
- PATCH and DELETE will be supported when .NET 8 is released.

## License

MinimalApi.Endpoints is licensed under the MIT license.