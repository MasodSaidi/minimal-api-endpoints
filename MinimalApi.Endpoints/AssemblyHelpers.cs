using MinimalApi.Attributes;
using System.Reflection;

namespace MinimalApi;

internal static class AssemblyHelpers
{
    internal static IEnumerable<Type> GetEndpointClasses()
    {
        var entryAssembly = GetEntryAssembly();
        return FindEndpointClasses(entryAssembly);
    }

    internal static IEnumerable<Type> GetEndpointClasses<T>()
    {
        var specifiedAssembly = typeof(T).Assembly;
        return FindEndpointClasses(specifiedAssembly);
    }

    internal static IEnumerable<MethodInfo> GetEndpointMethods()
    {
        var entryAssembly = GetEntryAssembly();
        return FindEndpointMethods(entryAssembly);
    }
    
    internal static IEnumerable<MethodInfo> GetEndpointMethods<T>()
    {
        var specifiedAssembly = typeof(T).Assembly;
        return FindEndpointMethods(specifiedAssembly);
    }

    private static Assembly GetEntryAssembly()
    {
        var entryAssembly = Assembly.GetEntryAssembly();
        if (entryAssembly == null)
            throw new InvalidOperationException("Entry assembly not found, specify the assembly using .AddEndpoints<T> and .MapEndpoints<T>");

        return entryAssembly;
    }
    
    private static IEnumerable<Type> FindEndpointClasses(Assembly assembly)
    {
        var classes = (from type in assembly.GetTypes()
            where type.IsClass && type.GetMethods().Any(m => m.IsDefined(typeof(EndpointBase), inherit: false))
            select type).ToList();
        
        if (!classes.Any())
            throw new InvalidOperationException("No endpoint classes found in assembly, specify the assembly using .AddEndpoints<T> and .MapEndpoints<T>");
        
        return classes;
    }
    
    private static IEnumerable<MethodInfo> FindEndpointMethods(Assembly assembly)
    {
        var methods = (from type in assembly.GetTypes()
            from method in type.GetMethods(BindingFlags.Public | BindingFlags.Instance)
            where type.IsClass && method.IsDefined(typeof(EndpointBase), inherit: false)
            select method).ToList();
        
        if (!methods.Any())
            throw new InvalidOperationException("No endpoint methods found in assembly, specify the assembly using .AddEndpoints<T> and .MapEndpoints<T>");
        
        return methods;
    }
}