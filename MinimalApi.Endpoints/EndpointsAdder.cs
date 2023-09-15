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

        foreach(var endpointClass in endpointClasses)
        {
            services.AddScoped(endpointClass);
        }
    }
}