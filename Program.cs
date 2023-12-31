using System.Reflection;
using android_backend.Filter;
using System.Net;
using Microsoft.OpenApi.Models;
using DotNetEnv;

Env.Load();
var builder = WebApplication.CreateBuilder(args);
var startup = new android_backend.Startup(builder.Configuration);
// Add services to the container.
builder.Services.AddControllers();
startup.ConfigureServices(builder.Services);
builder.Services.AddEndpointsApiExplorer();
#region Swagger
builder.Services.AddSwaggerGen(options =>
{
    // using System.Reflection;
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT valid"
    });
    options.OperationFilter<AuthorizeCheckOperationFilter>();
});
#endregion

builder.WebHost.UseKestrel(options =>
    {
        options.ListenAnyIP(5000); // HTTP 
        options.ListenAnyIP(5001, listenOptions =>
        {
            listenOptions.UseHttps();
        });
    });


ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
ServicePointManager.ServerCertificateValidationCallback +=
 (sender, cert, chain, sslPolicyErrors) => true;



var app = builder.Build();
//app.UseMvcWithDefaultRoute();

// Configure the HTTP request pipeline.

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}



//app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers().RequireAuthorization();




app.Run();





