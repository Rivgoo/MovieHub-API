#region Using Directives
using Asp.Versioning.Builder;
using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using Infrastructure;
using Application;
using System.Text.Json.Serialization;
using Web.API;
using Web.API.Core.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Text;
#endregion

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

builder.Services.AddSwaggerGen().ConfigureOptions<ConfigureSwaggerOptions>();

builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddApplicationServices();

builder.Services.AddControllers()
			.AddJsonOptions(options => { options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()); });

#region JWT
builder.Services.AddScoped<JwtAuthentication>();

var jwtSettings = builder.Configuration.GetSection("JWT").Get<JwtOptions>() ?? 
	throw new ArgumentNullException(nameof(JwtOptions), "JWT options are not configured.");

builder.Services.AddAuthentication().AddJwtBearer(opt =>
		{
			opt.SaveToken = false;
			opt.RequireHttpsMetadata = true;
			opt.TokenValidationParameters = new TokenValidationParameters
			{
				ValidateIssuer = true,
				ValidateAudience = true,
				ValidateLifetime = true,
				ValidateIssuerSigningKey = true,
				ValidIssuer = jwtSettings.Issuer,
				ValidAudience = jwtSettings.Audience,
				IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret))
			};
		});
#endregion

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

//if (app.Environment.IsDevelopment())
//{
	app.UseSwagger();
	app.UseSwaggerUI(options =>
	{
		IReadOnlyList<ApiVersionDescription> descriptions = app.DescribeApiVersions();

		foreach (var description in descriptions)
		{
			options.EnablePersistAuthorization();
			options.OAuthUsePkce();

			string url = $"/swagger/{description.GroupName}/swagger.json";
			string name = description.GroupName.ToUpperInvariant();

			options.SwaggerEndpoint(url, name);
		}
	});
//}

app.UseHttpsRedirection();
app.MapControllers();

app.Run();