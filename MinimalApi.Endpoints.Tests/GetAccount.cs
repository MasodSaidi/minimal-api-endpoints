using System;
using Microsoft.AspNetCore.Mvc;
using MinimalApi.Attributes;
using Swashbuckle.AspNetCore.Annotations;

namespace MinimalApi.Endpoints.Tests;

public class GetAccount
{
	[EndpointGet("/accounts/{id}")]
	[SwaggerOperation(Summary = "Get account by id")]
	public async Task<IResult> GetAsync(int id, ILogger<GetAccount> logger)
	{
		logger.LogInformation("Accounts endpoint visited at {DT}", DateTime.UtcNow.ToLongTimeString());
		return Results.Ok($"Getting id from accounts method endpoint {id}");
	}
}