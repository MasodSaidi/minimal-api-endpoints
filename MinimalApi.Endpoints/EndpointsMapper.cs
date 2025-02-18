using System.Reflection;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Builder;
using MinimalApi.Attributes;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace MinimalApi;

public static class EndpointsMapper
{
    public static WebApplication MapEndpoints(this WebApplication app)
    {
        var endpointMethods = AssemblyHelpers.GetEndpointMethods();
        app.MapEndpointMethods(endpointMethods);
        
        return app;
    }
    
    public static WebApplication MapEndpoints<T>(this WebApplication app)
    {
        var endpointMethods = AssemblyHelpers.GetEndpointMethods<T>();
        app.MapEndpointMethods(endpointMethods);
        
        return app;
    }
    
    private static void MapEndpointMethods(this WebApplication app, IEnumerable<MethodInfo> endpointMethods)
    {
        foreach (var method in endpointMethods)
        {
            MapEndpoint(app, method);
        }
    }

    private static void MapEndpoint(WebApplication app, MethodInfo method)
    {
        var handler = CreateHandler(method, app.Services);
        var endpointAttribute = method.GetCustomAttribute<EndpointBase>(inherit: false);

        switch (endpointAttribute)
        {
            case EndpointGet:
                app.MapGet(endpointAttribute.Pattern, handler);
                break;
            case EndpointPost:
                app.MapPost(endpointAttribute.Pattern, handler);
                break;
            case EndpointPut:
                app.MapPut(endpointAttribute.Pattern, handler);
                break;
            case EndpointDelete:
                app.MapDelete(endpointAttribute.Pattern, handler);
                break;
            case EndpointPatch:
                app.MapMethods(endpointAttribute.Pattern, new[] { "PATCH" }, handler);
                break;
            default:
                throw new NotSupportedException($"{endpointAttribute?.GetType().Name} type is not supported");
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
        var delegateHandler = Delegate.CreateDelegate(delegateType, instance!, method.Name, ignoreCase: true);

        return delegateHandler;
    }
}