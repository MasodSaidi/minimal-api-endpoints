using MinimalApi;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpoints();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options => options.EnableAnnotations());

var app = builder.Build();

app.MapEndpoints();
app.UseSwagger();
app.UseSwaggerUI();

app.Run();