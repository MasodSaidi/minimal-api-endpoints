namespace MinimalApi.Attributes;

[AttributeUsage(AttributeTargets.Method, Inherited = false)]
public class EndpointGet : EndpointBase
{
    public EndpointGet(string pattern) : base(pattern)
    {
    }
}