using MinimalApi.Attributes;
using Swashbuckle.AspNetCore.Annotations;

namespace MinimalApi.Endpoints.Tests;

public class GetAccount
{
	[EndpointGet("/accounts/{id}")]
	[SwaggerOperation(OperationId = "GetAccount", Summary = "Gets an account", Description = "Gets an account by id", Tags = new[] { "Account" })]
	public async Task<IResult> GetAsync(int id, ILogger<GetAccount> logger)
	{
		logger.LogInformation("Accounts endpoint visited at {DT}", DateTime.UtcNow.ToLongTimeString());
		return Results.Ok($"Getting id from accounts method endpoint {id}");
	}
}