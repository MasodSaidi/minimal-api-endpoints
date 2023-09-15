using System.Reflection;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Builder;
using MinimalApi.Attributes;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace MinimalApi;

public static class EndpointsMapper
{
    public static void MapEndpoints(this WebApplication app)
    {
        var endpointMethods = AssemblyHelpers.GetEndpointMethods();

        foreach (var method in endpointMethods)
        {
            var handler = CreateHandler(method, app.Services);
            var endpointAttribute = method.GetCustomAttribute<EndpointBase>(inherit: false);

            if (endpointAttribute is EndpointGet)
                app.MapGet(endpointAttribute.Pattern, handler);
            else if (endpointAttribute is EndpointPost)
                app.MapPost(endpointAttribute.Pattern, handler);
            else if (endpointAttribute is EndpointPut)
                app.MapPut(endpointAttribute.Pattern, handler);
            else if (endpointAttribute is EndpointDelete)
                app.MapDelete(endpointAttribute.Pattern, handler);
            else if (endpointAttribute is EndpointPatch)
                app.MapMethods(endpointAttribute.Pattern, new[] { "PATCH" }, handler);
        }
    }

    private static Delegate CreateHandler(MethodInfo method, IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var instance = scope.ServiceProvider.GetRequiredService(method.DeclaringType!);

        var parameterTypes = method.GetParameters()
            .Select(p => p.ParameterType)
            .Append(method.ReturnType);

        var delegateType = Expression.GetDelegateType(parameterTypes.ToArray());
        var delegateInstance = Delegate.CreateDelegate(delegateType, instance!, method.Name, ignoreCase: true);

        return delegateInstance;
    }
}