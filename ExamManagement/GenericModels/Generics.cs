using ExamManagement.DTOs.AuthenticationDTOs;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ExamManagement.GenericModels
{
    public static class Generics
    {
        public static ClaimsPrincipal SetClaimPrincipal(UserSession model)
        {
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, model.UserId!),
                new(ClaimTypes.Name, model.Name!),
                new(ClaimTypes.Email, model.Email!)
            };

            // Add all roles to the claims
            claims.AddRange(model.Roles.Select(role => new Claim(ClaimTypes.Role, role)));

            return new ClaimsPrincipal(new ClaimsIdentity(claims, "JwtAuth"));
        }

        public static UserSession GetClaimsFromToken(string jwtToken)
        {
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(jwtToken);
            var claims = token.Claims;

            string Id = claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value!;
            string Name = claims.First(c => c.Type == ClaimTypes.Name).Value!;
            string Email = claims.First(c => c.Type == ClaimTypes.Email).Value!;

            // Retrieve all roles
            var roles = claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value).ToList();

            return new UserSession(Id, Name, Email, roles);
        }

        public static JsonSerializerOptions JsonOptions()
        {
            return new JsonSerializerOptions
            {
                AllowTrailingCommas = true,
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                UnmappedMemberHandling = JsonUnmappedMemberHandling.Skip
            };
        }

        public static string SerializeObj<T>(T modelObject) => JsonSerializer.Serialize(modelObject, JsonOptions());
        public static T DeserializeJsonString<T>(string jsonString) => JsonSerializer.Deserialize<T>(jsonString, JsonOptions())!;
        public static IList<T> DeserializeJsonStringList<T>(string jsonString) => JsonSerializer.Deserialize<IList<T>>(jsonString, JsonOptions())!;

        public static StringContent GenerateStringContent(string serializedObj) => new(serializedObj, System.Text.Encoding.UTF8, "application/json");
    }
}
