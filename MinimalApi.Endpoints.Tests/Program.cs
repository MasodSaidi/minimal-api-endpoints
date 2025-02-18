using MinimalApi;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpoints<Program>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options => options.EnableAnnotations());

var app = builder.Build();

app.MapEndpoints<Program>();
app.UseSwagger();
app.UseSwaggerUI();

app.Run();