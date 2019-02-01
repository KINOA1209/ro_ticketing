using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using IdentityServer4;
using IdentityServer4.Models;
using Microsoft.Extensions.Configuration;

namespace ticketing_api.Infrastructure
{
    public static class SecurityConfiguration
    {
        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResources.Email()
            };
        }

        public static IEnumerable<ApiResource> GetApiResources()
        {
            return new List<ApiResource>
            {
                new ApiResource("api1", $"{AppConfiguration.Instance.AppName} API")
            };
        }

        // clients want to access resources (aka scopes)
        public static IEnumerable<Client> GetClients()
        {
            var origins = AppConfiguration.Instance.Configuration.GetSection("Security:Origins").Get<string[]>();

            // client credentials and resource owner password grant client
            return new List<Client>
            {
                //TODO: move client ID / secret to configuration
                new Client
                {
                    ClientId = "ro.client",
                    ClientSecrets = { new Secret("secret".Sha256()) },
                    AllowedGrantTypes = GrantTypes.ResourceOwnerPasswordAndClientCredentials,
                    AllowedScopes = new List<string>
                    {
                        "api1",
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile
                    },
                    AllowedCorsOrigins = new List<string>(origins),
                    AllowOfflineAccess = true,
                    IdentityTokenLifetime = 60 * 60 * 24 * 365,
                    AccessTokenLifetime = 60 * 60  * 24 * 365, // 30 min
                    SlidingRefreshTokenLifetime = 60 * 60  * 24 * 365,
                    AbsoluteRefreshTokenLifetime = 60 * 60 * 24* 30, //30 days
                    AlwaysIncludeUserClaimsInIdToken = true,
                    RefreshTokenExpiration = TokenExpiration.Sliding,
                    RefreshTokenUsage = TokenUsage.OneTimeOnly,
                    UpdateAccessTokenClaimsOnRefresh = true,
                    AlwaysSendClientClaims = true
                }
            };
        }
    }
}