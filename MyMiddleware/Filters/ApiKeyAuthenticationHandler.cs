


using Microsoft.AspNetCore.Authentication; 
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Encodings.Web; 
public class ApiKeyAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    private readonly string _requiredApiKey;
    private readonly string _requiredApiSalt;

    public ApiKeyAuthenticationHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock)
        : base(options, logger, encoder, clock)
    {
        _requiredApiKey = Environment.GetEnvironmentVariable("TMAPIKEY")?? "71e7539d-5c59-491b-9894-e6c684765a09";
        _requiredApiSalt = Environment.GetEnvironmentVariable("TMAPISALT")?? "b38895398145227b60412279189264e2";
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.TryGetValue("X-API-Key", out var apiKeyHeader))
        { 
           throw new UnAuthorizedException("API key not found in the request headers.");
        }

        string apiKey = apiKeyHeader.ToString();

        if (IsValidApiKey(_requiredApiKey,apiKey)) 
        {
            var claims = new[] { new Claim(ClaimTypes.NameIdentifier, apiKey), new Claim("ApiKey", apiKey) };
            var identity = new ClaimsIdentity(claims, Scheme.Name);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, Scheme.Name);

            return Task.FromResult(AuthenticateResult.Success(ticket));
        }
        // else
        // {
            throw new UnAuthorizedException("Invalid Key provided.");
        // }
    }




    private bool IsValidApiKey(string apiKey, string hashedApiKey)
    {   
         string saltedApiKey = apiKey + _requiredApiSalt;

         string computedHashedApiKey = ComputeSha256Hash(saltedApiKey);

         return computedHashedApiKey.Equals(hashedApiKey, StringComparison.OrdinalIgnoreCase);
    }

    private static string ComputeSha256Hash(string input)
    {
        using (SHA256 sha256 = SHA256.Create())
        {
            byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));
            StringBuilder builder = new StringBuilder();
            foreach (byte b in bytes)
            {
                builder.Append(b.ToString("x2"));
            }
            return builder.ToString();
        }
    }
 }