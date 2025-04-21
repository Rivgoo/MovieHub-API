using Web.API.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigureApplicationServices(builder.Configuration, builder.Environment);

var app = builder.Build();

await app.InitializeAdminAsync();

app.ConfigureRequestPipeline(app.Environment, app.Configuration);

app.MapControllers();

app.Run();