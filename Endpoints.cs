using System.Linq.Expressions;
using System.Reflection;
using Microsoft.AspNetCore.Builder;

namespace MinimalApi;

public static class Endpoints
{
    public static void MapEndpoints(this WebApplication app)
    {
        var assembly = Assembly.GetExecutingAssembly();

        var endpointClassesTypes = from type in assembly.GetTypes()
                                   where type.IsClass && type.GetMethods().Any(m => m.GetCustomAttributes(typeof(EndpointBase), false).Length > 0)
                                   select type;

        foreach (var endpointClassType in endpointClassesTypes)
        {
            var methods = endpointClassType.GetMethods(BindingFlags.Public | BindingFlags.Instance);
            foreach (var method in methods)
            {
                var attribute = method.GetCustomAttribute(typeof(EndpointBase), false);
                if (attribute == null)
                    continue;

                var endpointBase = (EndpointBase)attribute;
                var instance = Activator.CreateInstance(endpointClassType);
                var types = method.GetParameters().Select(p => p.ParameterType);
                var getType = Expression.GetFuncType;
                types = types.Concat(new[] { method.ReturnType });
                var delegateOfType = Delegate.CreateDelegate(getType(types.ToArray()), instance, method.Name, true);

                if (endpointBase.GetType() == typeof(EndpointGet))
                    app.MapGet(endpointBase.Pattern, delegateOfType);
                if (endpointBase.GetType() == typeof(EndpointPost))
                    app.MapPost(endpointBase.Pattern, delegateOfType);
                if (endpointBase.GetType() == typeof(EndpointPut))
                    app.MapPut(endpointBase.Pattern, delegateOfType);
                //if (attribute.GetType() == typeof(EndpointPatch))
                //    app.MapPatch(attribute.Pattern, delegateOfType);
            }
        }
    }
}