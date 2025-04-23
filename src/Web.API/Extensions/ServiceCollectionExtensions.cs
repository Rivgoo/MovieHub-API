using Application;
using Asp.Versioning;
using Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.Text.Json.Serialization;
using System.Text;
using Web.API.Core.Jwt;
using Web.API.Core.Options;

namespace Web.API.Extensions;

public static class ServiceCollectionExtensions
{
	/// <summary>
	/// Configures all application services.
	/// </summary>
	public static IServiceCollection ConfigureApplicationServices(this IServiceCollection services, IConfiguration configuration, IHostEnvironment environment)
	{
		services.AddCORS(configuration);
		services.AddJwtAuthentication(configuration);
		services.AddApiVersioningConfig();
		services.AddSwaggerConfiguration();
		services.AddInfrastructureServices(configuration); 
		services.AddApplicationServices();
		services.AddHttpContextAccessor();
		services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
		services.AddControllersWithJsonOptions();

		services.Configure<InitialAdminOptions>(configuration.GetSection("InitialAdmin"));

		return services;
	}

	/// <summary>
	/// Configures JWT authentication services.
	/// </summary>
	private static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
	{
		services.AddScoped<JwtAuthentication>();

		var jwtSettings = configuration.GetSection("JWT").Get<JwtOptions>() ??
			throw new ArgumentNullException(nameof(JwtOptions), "JWT options are not configured.");

		services.AddAuthentication(options =>
		{
			options.DefaultAuthenticateScheme = IdentityConstants.ApplicationScheme;
			options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
		}).AddJwtBearer(opt =>
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

		services.AddAuthorization();

		return services;
	}

	/// <summary>
	/// Configures API versioning services.
	/// </summary>
	private static IServiceCollection AddApiVersioningConfig(this IServiceCollection services)
	{
		services.AddApiVersioning(opt =>
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

		return services;
	}

	/// <summary>
	/// Configures Swagger generation services.
	/// </summary>
	private static IServiceCollection AddSwaggerConfiguration(this IServiceCollection services)
	{
		services.AddSwaggerGen();
		services.ConfigureOptions<ConfigureSwaggerOptions>();

		return services;
	}

	/// <summary>
	/// Configures controllers and JSON options.
	/// </summary>
	private static IServiceCollection AddControllersWithJsonOptions(this IServiceCollection services)
	{
		services.AddControllers()
			.AddJsonOptions(options => { options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()); });

		return services;
	}

	private static IServiceCollection AddCORS(this IServiceCollection services, IConfiguration configuration)
	{
		var origins = configuration.GetSection("AllowedOrigins").Get<string[]>() ??
			throw new ArgumentNullException("Allowed origins are not configured.");

		services.AddCors(options =>
		{
			options.AddPolicy("AllowSpecificOrigin",
				policyBuilder =>
				{
					policyBuilder.WithOrigins(origins)
								 .AllowAnyHeader()
								 .AllowAnyMethod();
				});
		});

		return services;
	}
}