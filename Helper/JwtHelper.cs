using System.Security.Claims;
using System.Security.Cryptography;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
namespace android_backend.Helper;

public class JwtHelper
{
    private readonly IConfiguration configuration;
    private RsaSecurityKey signKey;

    public JwtHelper(IConfiguration configuration)
    {
        this.configuration = configuration;
        String privateKey = configuration.GetValue<string>("JwtSettings:PrivateKey");
        byte[] privateKeyBytes = Convert.FromBase64String(privateKey);
        RSA rsa = RSA.Create(2048);
        rsa.ImportRSAPrivateKey(privateKeyBytes, out _);
        signKey = new RsaSecurityKey(rsa);
    }

    public string GenerateToken(string userName, int expireMinutes = 30)
    {
        String issuer = configuration.GetValue<string>("JwtSettings:Issuer");

        List<Claim> claims = new List<Claim>();

        claims.Add(new Claim(JwtRegisteredClaimNames.Sub, userName)); // User.Identity.Name
        claims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())); // JWT ID



        claims.Add(new Claim(ClaimTypes.Role, "admin"));
        claims.Add(new Claim(ClaimTypes.Role, "user"));

        ClaimsIdentity userClaimsIdentity = new ClaimsIdentity(claims);

        //SymmetricSecurityKey securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signKey.ToString()));
        SigningCredentials signingCredentials = new SigningCredentials(signKey, SecurityAlgorithms.RsaSha256);

        // Create SecurityTokenDescriptor
        SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor
        {
            Issuer = issuer,
            Subject = userClaimsIdentity,
            Expires = DateTime.Now.AddMinutes(expireMinutes),
            SigningCredentials = signingCredentials
        };

        JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
        SecurityToken securityToken = tokenHandler.CreateToken(tokenDescriptor);
        String serializeToken = tokenHandler.WriteToken(securityToken);

        return serializeToken;
    }

    public void addService(IServiceCollection services)
    {
        JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.IncludeErrorDetails = true;

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    NameClaimType = "sub",
                    RoleClaimType = "role",

                    ValidateIssuer = true,
                    ValidIssuer = configuration.GetValue<string>("JwtSettings:Issuer"),

                    ValidateAudience = false,

                    ValidateLifetime = true,

                    ValidateIssuerSigningKey = false,

                    IssuerSigningKey = new RsaSecurityKey(signKey.Rsa.ExportParameters(false))
                };
            });

        services.AddAuthorization();
    }
}