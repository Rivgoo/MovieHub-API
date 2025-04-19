using Asp.Versioning.ApiExplorer;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;
using Unchase.Swashbuckle.AspNetCore.Extensions.Extensions;

namespace Web.API;

/// <summary>
/// Configures Swagger options for API versioning and documentation.
/// </summary>
/// <remarks>
/// This class implements <see cref="IConfigureNamedOptions{TOptions}"/> to configure
/// <see cref="SwaggerGenOptions"/>, specifically integrating with API versioning
/// provided by <see cref="IApiVersionDescriptionProvider"/>. It also includes
/// XML documentation comments from multiple project assemblies.
/// </remarks>
/// <remarks>
/// Initializes a new instance of the <see cref="ConfigureSwaggerOptions"/> class.
/// </remarks>
/// <param name="provider">The provider used to enumerate API version descriptions.</param>
public class ConfigureSwaggerOptions(IApiVersionDescriptionProvider provider) :
	IConfigureNamedOptions<SwaggerGenOptions>
{
	private readonly IApiVersionDescriptionProvider _provider = provider;

	/// <summary>
	/// Configures Swagger options for each API version discovered and includes XML documentation from related assemblies.
	/// </summary>
	/// <remarks>
	/// This method is the primary configuration entry point. It performs the following steps:
	/// <list type="bullet">
	///   <item>Adds a Swagger document (<c>SwaggerDoc</c>) for each discovered API version group.</item>
	///   <item>Includes XML documentation comments from the current executing assembly (Web.API), Application, Infrastructure, and Domain assemblies.</item>
	///   <item>Applies filters to enhance enum display in the Swagger UI, including names, values, and descriptions.</item>
	/// </list>
	/// </remarks>
	/// <param name="options">The <see cref="SwaggerGenOptions"/> to configure.</param>
	public void Configure(SwaggerGenOptions options)
	{
		foreach (var description in _provider.ApiVersionDescriptions)
			options.SwaggerDoc(
				description.GroupName,
				CreateVersionInfo(description));

		options.CustomSchemaIds(type => type.Name);

		var currentProjectXmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
		var applicationXmlFile = $"{Application.AssemblyReference.Assembly.GetName().Name}.xml";
		var infrastructureXmlFile = $"{Infrastructure.AssemblyReference.Assembly.GetName().Name}.xml";
		var domainXmlFile = $"{Domain.AssemblyReference.Assembly.GetName().Name}.xml";

		options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, currentProjectXmlFile));
		options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, applicationXmlFile));
		options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, infrastructureXmlFile));
		options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, domainXmlFile));


		options.AddEnumsWithValuesFixFilters(o =>
		{
			o.ApplySchemaFilter = true;
			o.XEnumNamesAlias = "x-enum-varnames";
			o.XEnumDescriptionsAlias = "x-enum-descriptions";
			o.ApplyParameterFilter = true;
			o.ApplyDocumentFilter = true;
			o.IncludeDescriptions = true;
			o.IncludeXEnumRemarks = true;
			o.DescriptionSource = DescriptionSources.DescriptionAttributesThenXmlComments;
			o.NewLine = "\n";
		});


        options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            In = ParameterLocation.Header,
            Description = "Please enter a valid token (e.g., Bearer eyJ...)",
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            BearerFormat = "JWT",
            Scheme = "Bearer"
        });

        options.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },

				Array.Empty<string>()
			}
        });
	}

	/// <summary>
	/// Configures Swagger options for a named instance of <see cref="SwaggerGenOptions"/>.
	/// </summary>
	/// <remarks>
	/// This implementation simply delegates the configuration to the non-named <see cref="Configure(SwaggerGenOptions)"/> method.
	/// This is required by the <see cref="IConfigureNamedOptions{TOptions}"/> interface.
	/// </remarks>
	/// <param name="name">The name of the options instance being configured. This parameter is ignored by this implementation.</param> // Вказано, що параметр ігнорується
	/// <param name="options">The <see cref="SwaggerGenOptions"/> to configure.</param>
	public void Configure(string name, SwaggerGenOptions options) => Configure(options);

	/// <summary>
	/// Creates OpenAPI information for a given API version description.
	/// </summary>
	/// <param name="desc">The <see cref="ApiVersionDescription"/> containing details about the API version.</param>
	/// <returns>An <see cref="OpenApiInfo"/> object containing the title, version, and potentially deprecation notes for the API version.</returns>
	private static OpenApiInfo CreateVersionInfo(ApiVersionDescription desc)
	{
		var info = new OpenApiInfo
		{
			Title = "Moview Poster API (.NET 9)",
			Version = desc.ApiVersion.ToString()
		};

		if (desc.IsDeprecated)
			info.Description += " This API version has been deprecated. Please use one of the new APIs available from the explorer.";

		if (desc.SunsetPolicy != null)
			info.Description += $" This API will be sunset on {desc.SunsetPolicy.Date.Value.DateTime.ToShortDateString()}.";

		return info;
	}
}