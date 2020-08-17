using IdentityModel;
using IdentityServer4;
using IdentityServer4.Models;
using System.Collections.Generic;
using static IdentityServer4.IdentityServerConstants;

namespace MovieLand.IdentityServer
{
    public static class Config
    {
        public static IEnumerable<IdentityResource> GetIdentityResources() {
            return new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile()
            };
        }

        public static IEnumerable<ApiResource> GetApis() {
            return new List<ApiResource>
            {
                new ApiResource("api1", "My API", new[] { JwtClaimTypes.Role, JwtClaimTypes.Name })
            };
        }

        public static IEnumerable<Client> GetClients() {
            return new List<Client>
            {
                // resource owner password grant client
                new Client
                {
                    ClientId = "ro.client",
                    AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,

                    ClientSecrets =
                    {
                        new Secret("secret".Sha256())
                    },
                    AllowedScopes = {
                        "api1",
                        StandardScopes.OpenId,
                        StandardScopes.Profile
                    }
                }
            };
        }
    }
}
