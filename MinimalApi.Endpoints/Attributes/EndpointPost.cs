namespace MinimalApi.Attributes;

[AttributeUsage(AttributeTargets.Method, Inherited = false)]
public class EndpointPost : EndpointBase
{
    public EndpointPost(string pattern) : base(pattern)
    {
    }
}