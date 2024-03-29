
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options; 
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Encodings.Web;

public class BasicAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    private readonly ILogger<BasicAuthenticationHandler> Logger;

    public BasicAuthenticationHandler(ILogger<BasicAuthenticationHandler> Logger,
   IOptionsMonitor<AuthenticationSchemeOptions> options,
   ILoggerFactory logger,
   UrlEncoder encoder,
   ISystemClock clock)
   : base(options, logger, encoder, clock)
   {
        this.Logger = Logger;
    }
   protected override  Task<AuthenticateResult> HandleAuthenticateAsync()
   {
       if (!Request.Headers.ContainsKey("Authorization"))
           throw new UnAuthorizedException("Missing Authorization Header");

       
           var authHeader = AuthenticationHeaderValue.Parse(Request.Headers["Authorization"]);
           var credentialBytes = Convert.FromBase64String(authHeader.Parameter);
           var credentials = Encoding.UTF8.GetString(credentialBytes).Split(new[] { ':' }, 2);
           var user = credentials[0];
           var code = credentials[1];


       if (IsAuthorized(user, code))
       {
           
      
               var claims = new[] {
                       new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.NameIdentifier, user),
                       new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Name, user)
                   };
               var identity = new ClaimsIdentity(claims, Scheme.Name);
               var principal = new ClaimsPrincipal(identity);
               var ticket = new AuthenticationTicket(principal, Scheme.Name);

               return Task.FromResult(AuthenticateResult.Success(ticket));
       }

        Logger.LogError($"authentication failed for user: {user}");
       throw new UnAuthorizedException("Invalid username or password");


       
   }

    private string GetHashedPassword(string password)
    {
        using (SHA256 sha256 = SHA256.Create())
        {
            byte[] hashedBytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashedBytes);
        }
    }

    private bool IsAuthorized(string user, string password)
    {
        var user_ = Environment.GetEnvironmentVariable("APIuser");
        var hashedPassword_ = Environment.GetEnvironmentVariable("APIhashedPassword");

        var hashedPassword = GetHashedPassword(password);

        var result = user == user_ && hashedPassword == hashedPassword_;
        Logger.LogInformation($"connection {result}");
        return result;
    }
}

