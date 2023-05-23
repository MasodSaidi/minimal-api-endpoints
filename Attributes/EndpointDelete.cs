namespace MinimalApi.Attributes;

[AttributeUsage(AttributeTargets.Method, Inherited = false)]
public class EndpointDelete : EndpointBase
{
    public EndpointDelete(string pattern) : base(pattern)
    {
    }
}