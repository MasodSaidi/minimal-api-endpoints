using MinimalApi;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddEndpoints<Program>()
    .AddEndpointsApiExplorer()
    .AddSwaggerGen(options => options.EnableAnnotations());

var app = builder.Build();

app.MapEndpoints<Program>()
    .UseSwagger()
    .UseSwaggerUI();

app.Run();