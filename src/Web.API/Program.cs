using Asp.Versioning.Builder;
using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using Infrastructure;
using Application;

var builder = WebApplication.CreateBuilder(args);

#region Api Versioning
builder.Services.AddApiVersioning(opt =>
{
	opt.DefaultApiVersion = new ApiVersion(1, 0);
	opt.AssumeDefaultVersionWhenUnspecified = true;
	opt.ReportApiVersions = true;
	opt.ApiVersionReader = ApiVersionReader.Combine(new UrlSegmentApiVersionReader(),
													new HeaderApiVersionReader("x-api-version"),
													new MediaTypeApiVersionReader("x-api-version"));
}).AddApiExplorer(setup =>
{
	setup.GroupNameFormat = "'v'VVV";
	setup.SubstituteApiVersionInUrl = true;
});
#endregion

builder.Services.AddSwaggerGen();

builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddApplicationServices();

var app = builder.Build();

#region Api Versioning
ApiVersionSet apiVersionSet = app.NewApiVersionSet()
	.HasApiVersion(new ApiVersion(1))
	.ReportApiVersions()
	.Build();

RouteGroupBuilder group = app
	.MapGroup("api/v{version:apiVersion}")
	.WithApiVersionSet(apiVersionSet);
#endregion

if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI(options =>
	{
		IReadOnlyList<ApiVersionDescription> descriptions = app.DescribeApiVersions();

		foreach (ApiVersionDescription description in descriptions)
		{
			string url = $"/swagger/{description.GroupName}/swagger.json";
			string name = description.GroupName.ToUpperInvariant();

			options.SwaggerEndpoint(url, name);
		}
	});
}

app.UseHttpsRedirection();
app.MapControllers();

app.Run();