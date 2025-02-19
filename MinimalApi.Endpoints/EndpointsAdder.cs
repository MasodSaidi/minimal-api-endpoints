using System.Reflection;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Builder;
using MinimalApi.Attributes;
using Microsoft.Extensions.DependencyInjection;

namespace MinimalApi;

public static class EndpointsAdder
{
    public static IServiceCollection AddEndpoints(this IServiceCollection services)
    {
        var endpointClasses = AssemblyHelpers.GetEndpointClasses();
        services.RegisterEndpointClasses(endpointClasses);
        
        return services;
    }

    public static IServiceCollection AddEndpoints<T>(this IServiceCollection services)
    {
        var endpointClasses = AssemblyHelpers.GetEndpointClasses<T>();
        services.RegisterEndpointClasses(endpointClasses);
        
        return services;
    }
    
    private static void RegisterEndpointClasses(this IServiceCollection services, IEnumerable<Type> endpointClasses)
    {
        foreach(var endpointClass in endpointClasses)
        {
            services.AddSingleton(endpointClass);
        }
    }
}