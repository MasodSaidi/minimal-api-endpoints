namespace MinimalApi.Attributes;

[AttributeUsage(AttributeTargets.Method, Inherited = false)]
public abstract class EndpointBase : Attribute
{
    public readonly string Pattern;

    protected EndpointBase(string pattern)
    {
        Pattern = pattern;
    }
}