namespace MinimalApi;

[AttributeUsage(AttributeTargets.Method, Inherited = false)]
public class EndpointPut : EndpointBase
{
    public EndpointPut(string pattern) : base(pattern)
    {
    }
}