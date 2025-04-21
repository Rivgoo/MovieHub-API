#region Using Directives
using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using Infrastructure;
using Application;
using System.Text.Json.Serialization;
using Web.API;
using Web.API.Core.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Application.Users.Abstractions;
using Application.Users.Models;
using Microsoft.Extensions.Options;
using Web.API.Core.Options;
using AutoMapper;
using Microsoft.Extensions.FileProviders;



#endregion

var builder = WebApplication.CreateBuilder(args);

#region JWT
builder.Services.AddScoped<JwtAuthentication>();

var jwtSettings = builder.Configuration.GetSection("JWT").Get<JwtOptions>() ??
	throw new ArgumentNullException(nameof(JwtOptions), "JWT options are not configured.");

builder.Services.AddAuthentication(options =>
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

builder.Services.AddAuthorization();
#endregion

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
builder.Services.Configure<InitialAdminOptions>(builder.Configuration.GetSection("InitialAdmin"));

builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddApplicationServices();
builder.Services.AddHttpContextAccessor();
builder.Services.AddAutoMapper(typeof(Program));

builder.Services.AddControllers()
			.AddJsonOptions(options => { options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()); });

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

if (app.Environment.IsDevelopment())
	app.UseHttpsRedirection();

if (app.Configuration.GetValue<bool>("UseSwagger"))
{
	app.UseSwagger();
	app.UseSwaggerUI(options =>
	{
		options.EnablePersistAuthorization();

		IReadOnlyList<ApiVersionDescription> descriptions = app.DescribeApiVersions();

		foreach (var description in descriptions)
		{
			string url = $"/swagger/{description.GroupName}/swagger.json";
			string name = description.GroupName.ToUpperInvariant();

			options.SwaggerEndpoint(url, name);
		}
	});
}

app.MapControllers();

var publicDataFolder = app.Configuration.GetValue<string>("PublicDataFolder");

if(string.IsNullOrWhiteSpace(publicDataFolder))
	throw new ArgumentNullException(nameof(publicDataFolder), "Public data folder is not configured.");

var publicFilesRoot = Path.Combine(AppContext.BaseDirectory, publicDataFolder);

if (!Directory.Exists(publicFilesRoot))
{
	try
	{
		Directory.CreateDirectory(publicFilesRoot);
	}
	catch (Exception ex)
	{
		throw new InvalidOperationException($"Failed to create public files directory at {publicFilesRoot}", ex);
	}
}

app.UseStaticFiles(new StaticFileOptions
{
	FileProvider = new PhysicalFileProvider(publicFilesRoot),
	RequestPath = ""
});

#region Try Initialize First Admin
using (var scope = app.Services.CreateScope())
{
	var services = scope.ServiceProvider;
	var initialAdminOptions = services.GetRequiredService<IOptions<InitialAdminOptions>>();

	if (initialAdminOptions != null)
	{
		var admin = initialAdminOptions.Value;
		var logger = services.GetRequiredService<ILogger<Program>>();
		var userService = services.GetRequiredService<IUserService>();
		var mapper = services.GetRequiredService<IMapper>();
		var userRegistrator = services.GetRequiredService<IUserRegistrator>();

		try
		{
			var adminUserExistsResult = await userService.ExistsByEmailAsync(admin.Email);

			if (adminUserExistsResult.IsSuccess && adminUserExistsResult.Value == false)
			{
				var registrationModel = mapper.Map<RegistrationUserModel>(admin);

				var registrationResult = await userRegistrator.RegisterAdminAsync(registrationModel);

				if (registrationResult.IsSuccess)
					logger.LogInformation("Initial admin user created successfully.");
				else
					logger.LogError($"Failed to create initial admin user. Error: {registrationResult.Error.Code}");
			}
			else if (adminUserExistsResult.IsFailure)
			{
				logger.LogError($"Failed to check if admin user exists. Error: {adminUserExistsResult.Error.Code}");
			}
			else
			{
				if (app.Environment.IsDevelopment() == false)
				{
					logger.LogError($"Admin user with email {admin.Email} already exists.");
					logger.LogError("Please remove the initial admin user from the configuration file.");
					return;
				}
				else
				{
					logger.LogWarning($"Admin user with email {admin.Email} already exists.");
				}
			}
		}
		catch (Exception ex)
		{
			logger.LogError(ex, "An error occurred while seeding the initial admin user.");
		}
	}
}
#endregion

app.Run();