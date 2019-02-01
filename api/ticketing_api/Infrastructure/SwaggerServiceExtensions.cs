using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Filters;
using Swashbuckle.AspNetCore.Swagger;

namespace ticketing_api.Infrastructure
{
    public static class SwaggerServiceExtensions
    {
        public static IServiceCollection AddSwaggerDocumentation(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc($"v{AppConfiguration.Instance.AppVersion}", new Info { Title = $"{AppConfiguration.Instance.AppName} API", Version = $"v{AppConfiguration.Instance.AppVersion}" });

                var filePath = Path.Combine(System.AppContext.BaseDirectory, "ticketing_api.xml");
                c.IncludeXmlComments(filePath);

                c.AddSecurityDefinition("Bearer", new OAuth2Scheme
                {
                    Type = "oauth2",
                    Flow = "password",
                    AuthorizationUrl = $"{AppConfiguration.Instance.Security["Authority"]}/connect/authorize",
                    TokenUrl = $"{AppConfiguration.Instance.Security["Authority"]}/connect/token",
                    Scopes = new Dictionary<string, string> { { "api1", "Default Scope" } }
                });

                c.OperationFilter<SecurityRequirementsOperationFilter>();
                c.OperationFilter<AppendAuthorizeToSummaryOperationFilter>();
                c.OperationFilter<AddFileParamTypesOperationFilter>();

                c.AddSecurityRequirement(new Dictionary<string, IEnumerable<string>> { { "Bearer", new string[] { } } });
            });

            return services;
        }

        public static IApplicationBuilder UseSwaggerDocumentation(this IApplicationBuilder app)
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", $"{AppConfiguration.Instance.AppName} API v{AppConfiguration.Instance.AppVersion}");
                c.DocExpansion(DocExpansion.None);
                c.OAuthClientId("ro.client");
                c.OAuthClientSecret("secret");
            });

            return app;
        }
    }
}
