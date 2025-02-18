using System.Reflection;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Builder;
using MinimalApi.Attributes;
using Microsoft.Extensions.DependencyInjection;

namespace MinimalApi;

public static class EndpointsAdder
{
    public static void AddEndpoints(this IServiceCollection services)
    {
        var endpointClasses = AssemblyHelpers.GetEndpointClasses();
        services.RegisterEndpointClasses(endpointClasses);
    }

    public static void AddEndpoints<T>(this IServiceCollection services)
    {
        var endpointClasses = AssemblyHelpers.GetEndpointClasses<T>();
        services.RegisterEndpointClasses(endpointClasses);
    }
    
    private static void RegisterEndpointClasses(this IServiceCollection services, IEnumerable<Type> endpointClasses)
    {
        foreach(var endpointClass in endpointClasses)
        {
            services.AddScoped(endpointClass);
        }
    }
}