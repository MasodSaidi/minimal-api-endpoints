# MinimalApi.Endpoints

MinimalApi.Endpoints is a package that allows you to easily structure your Minimal Api Endpoints in individual classes instead of having them all in `Program.cs`.

## Installation

You can install MinimalApi.Endpoints via NuGet Package Manager in Visual Studio or by using the following command:

```
dotnet add package MinimalApi.Endpoints
```

## Usage

Simply call `AddEndpoints` and `MapEndpoints` extension methods in your `Program.cs`:

```csharp #6
using MinimalApi;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpoints();

var app = builder.Build();

app.MapEndpoints();

app.Run();
```

Then create a simple class that contains a method decorated with `EndpointGet` attribute:

```csharp
public class GetUser
{
    [EndpointGet("/users/{id}")]
    public async Task<User> GetAsync(int id, DataContext data)
    {
        return await data.Users.FindAsync(id);
    }
}
```

You have the freedom to place the class anywhere within your project, utilizing any name you prefer. Similarly, you can name the method in whichever way you find suitable.
Only requirement is to use appropriate Endpoint attribute.


### More examples

Here are some examples of using `EndpointPost` and `EndpointPut`:

```csharp
public class CreateUser
{
    [EndpointPost("/users")]
    public async Task<User> PostAsync(User user, DataContext data)
    {
        data.Users.Add(user);
        await data.SaveChangesAsync();
        return user;
    }
}
```

```csharp
public class UpdateUser
{
    [EndpointPut("/users/{id}")]
    public async Task<User> PutAsync(int id, User updatedUser, DataContext data)
    {
        var user = await data.Users.FindAsync(id);
        user.Name = updatedUser.Name;
        user.Email = updatedUser.Email;
        await data.SaveChangesAsync();
        return user;
    }
}
```

### I want Controllers!

If you rather prefer to have all methods in one class:

```csharp
public class WannaBeUserController
{
    private readonly DataContext _data;

    public WannaBeUserController(DataContext data)
    {
        _data = data;
    }

    [EndpointGet("/users/{id}")]
    public async Task<User> GetAsync(int id)
    {
        return await _data.Users.FindAsync(id);
    }

    [EndpointPost("/users")]
    public async Task<User> PostAsync(User user)
    {
        _data.Users.Add(user);
        await _data.SaveChangesAsync();
        return user;
    }

    [EndpointPut("/users/{id}")]
    public async Task<User> PutAsync(int id, User updatedUser)
    {
        var user = await _data.Users.FindAsync(id);
        user.Name = updatedUser.Name;
        user.Email = updatedUser.Email;
        await _data.SaveChangesAsync();
        return user;
    }
}
```

## Swagger/OpenAPI support

Have full Swagger/OpenAPI support with comment or attribute based documentation.

### Supported Endpoint attributes

- `EndpointGet`
- `EndpointPost`
- `EndpointPut`
- `EndpointPatch`
- `EndpointDelete`

## License

MinimalApi.Endpoints is licensed under the MIT license.