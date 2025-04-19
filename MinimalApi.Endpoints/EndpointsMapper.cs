using System.Reflection;
using System.Linq.Expressions;
using System.Reflection.Emit;
using Microsoft.AspNetCore.Builder;
using MinimalApi.Attributes;
using Microsoft.AspNetCore.Mvc;

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
        var endpointAttribute = method.GetCustomAttribute<EndpointBase>(inherit: false)
                                ?? throw new InvalidOperationException(
                                    $"Method {method.Name} must have an Endpoint attribute.");

        var handlerDelegate = CreateEndpointDelegate(method);

        var builder = endpointAttribute switch
        {
            EndpointGet get => app.MapGet(get.Pattern, handlerDelegate),
            EndpointPost post => app.MapPost(post.Pattern, handlerDelegate),
            EndpointPut put => app.MapPut(put.Pattern, handlerDelegate),
            EndpointDelete delete => app.MapDelete(delete.Pattern, handlerDelegate),
            EndpointPatch patch => app.MapMethods(patch.Pattern, new[] { "PATCH" }, handlerDelegate),
            _ => throw new NotSupportedException($"Endpoint type {endpointAttribute.GetType().Name} is not supported.")
        };
        
        var methodAttributes = method.GetCustomAttributes(inherit: true);
        var typeAttributes = method.DeclaringType?.GetCustomAttributes(inherit: true) ?? Array.Empty<object>();

        builder.WithMetadata(typeAttributes.Concat(methodAttributes).ToArray());
    }

    private static Delegate CreateEndpointDelegate(MethodInfo endpointMethod)
    {
        var endpointType = endpointMethod.DeclaringType
                           ?? throw new InvalidOperationException("Endpoint method must have a declaring type.");
        var methodParameters = endpointMethod.GetParameters();

        var paramTypes = new Type[methodParameters.Length + 1];
        paramTypes[0] = endpointType;
        for (int i = 0; i < methodParameters.Length; i++)
        {
            paramTypes[i + 1] = methodParameters[i].ParameterType;
        }

        var returnType = endpointMethod.ReturnType;

        var asmName = new AssemblyName("DynamicEndpointAssembly");
        var assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(asmName, AssemblyBuilderAccess.Run);
        var moduleBuilder = assemblyBuilder.DefineDynamicModule("DynamicEndpointModule");
        var typeBuilder = moduleBuilder.DefineType(endpointMethod.DeclaringType.Name,
            TypeAttributes.Public | TypeAttributes.Sealed | TypeAttributes.Abstract);

        var methodBuilder = typeBuilder.DefineMethod(
            endpointMethod.Name + "Wrapper",
            MethodAttributes.Public | MethodAttributes.Static,
            returnType,
            paramTypes);

        var paramBuilder0 = methodBuilder.DefineParameter(1, ParameterAttributes.None, "endpoint");

        var fromServicesCtor = typeof(FromServicesAttribute).GetConstructor(Type.EmptyTypes);
        if (fromServicesCtor != null)
        {
            var fromServicesAttr = new CustomAttributeBuilder(fromServicesCtor, Array.Empty<object>());
            paramBuilder0.SetCustomAttribute(fromServicesAttr);
        }

        for (int i = 0; i < methodParameters.Length; i++)
        {
            var paramName = string.IsNullOrWhiteSpace(methodParameters[i].Name)
                ? $"p{i}"
                : methodParameters[i].Name;
            methodBuilder.DefineParameter(i + 2, ParameterAttributes.None, paramName);
        }

        var il = methodBuilder.GetILGenerator();

        il.Emit(OpCodes.Ldarg_0);

        for (int i = 1; i < paramTypes.Length; i++)
        {
            switch (i)
            {
                case 1: il.Emit(OpCodes.Ldarg_1); break;
                case 2: il.Emit(OpCodes.Ldarg_2); break;
                case 3: il.Emit(OpCodes.Ldarg_3); break;
                default: il.Emit(OpCodes.Ldarg_S, (short)i); break;
            }
        }

        il.Emit(OpCodes.Callvirt, endpointMethod);
        il.Emit(OpCodes.Ret);

        var generatedType = typeBuilder.CreateType();
        var wrapperMethod =
            generatedType.GetMethod(endpointMethod.Name + "Wrapper", BindingFlags.Public | BindingFlags.Static);

        var delegateParamTypes = paramTypes.Concat(new[] { returnType }).ToArray();
        var delegateType = Expression.GetDelegateType(delegateParamTypes);

        return Delegate.CreateDelegate(delegateType, wrapperMethod);
    }
}