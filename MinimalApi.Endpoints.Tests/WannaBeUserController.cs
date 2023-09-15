using System;
using MinimalApi.Attributes;

namespace MinimalApi.Endpoints.Tests
{
    public class WannaBeUserController
    {
        private readonly ILogger<WannaBeUserController> _logger;

        public WannaBeUserController(ILogger<WannaBeUserController> logger)
        {
            _logger = logger;
        }

        [EndpointGet("/users/{id}")]
        public async Task<IResult> GetAsync(int id)
        {
            _logger.LogInformation("User controller endpoint visited at {DT}", DateTime.UtcNow.ToLongTimeString());
            return Results.Ok($"Getting id from user controller {id}");
        }
    }
}