using System.Reflection;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Builder;
using MinimalApi.Attributes;

namespace MinimalApi;

public static class Endpoints
{
    public static void MapEndpoints(this WebApplication app)
    {
        var endpointClasses = GetEndpointClasses();
        var endpointMethods = endpointClasses.SelectMany(GetEndpointMethods);

        foreach (var method in endpointMethods)
        {
            var handler = CreateHandler(method);
            var endpointBase = method.GetCustomAttribute<EndpointBase>(inherit: false);

            if (endpointBase is EndpointGet)
                app.MapGet(endpointBase.Pattern, handler);
            if (endpointBase is EndpointPost)
                app.MapPost(endpointBase.Pattern, handler);
            if (endpointBase is EndpointPut)
                app.MapPut(endpointBase.Pattern, handler);
            // if (endpointBase is EndpointPatch)
            //    app.MapPatch(endpointBase.Pattern, handler);
        }
    }

    private static IEnumerable<Type> GetEndpointClasses()
    {
        var assembly = Assembly.GetExecutingAssembly();

        return from type in assembly.GetTypes()
               where type.IsClass && type.GetMethods().Any(m => m.IsDefined(typeof(EndpointBase), inherit: false))
               select type;
    }

    private static IEnumerable<MethodInfo> GetEndpointMethods(Type endpointClass)
    {
        return endpointClass
            .GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .Where(method => method.IsDefined(typeof(EndpointBase), inherit: false));
    }

    private static Delegate CreateHandler(MethodInfo method)
    {
        var instance = Activator.CreateInstance(method.DeclaringType!);

        var parameterTypes = method.GetParameters()
            .Select(p => p.GetType())
            .Append(method.ReturnType);

        var delegateType = Expression.GetDelegateType(parameterTypes.ToArray());
        var delegateInstance = Delegate.CreateDelegate(delegateType, instance!, method.Name, ignoreCase: true);

        return delegateInstance;
    }
}