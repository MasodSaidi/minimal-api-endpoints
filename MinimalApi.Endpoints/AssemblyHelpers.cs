using System;
using MinimalApi.Attributes;
using System.Reflection;

namespace MinimalApi;

internal static class AssemblyHelpers
{
    internal static IEnumerable<Type> GetEndpointClasses()
    {
        var entryAssembly = GetEntryAssembly();

        return from type in entryAssembly.GetTypes()
               where type.IsClass && type.GetMethods().Any(m => m.IsDefined(typeof(EndpointBase), inherit: false))
               select type;
    }

    internal static IEnumerable<MethodInfo> GetEndpointMethods()
    {
        var entryAssembly = GetEntryAssembly();

        return from type in entryAssembly.GetTypes()
                from method in type.GetMethods(BindingFlags.Public | BindingFlags.Instance)
                where type.IsClass && method.IsDefined(typeof(EndpointBase), inherit: false)
                select method;
    }

    private static Assembly GetEntryAssembly()
    {
        var entryAssembly = Assembly.GetEntryAssembly();
        if (entryAssembly == null)
            throw new InvalidOperationException("Entry assembly not found");

        return entryAssembly;
    }
}