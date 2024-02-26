using AspNetCoreRateLimit;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerUI;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMvc()
        .ConfigureApiBehaviorOptions(options =>
        {
            options.SuppressModelStateInvalidFilter = true;
        });
builder.Services.AddControllers(); 

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
#region <Auth> 

builder.Services.AddAuthentication("BasicAuthentication")
               .AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>("BasicAuthentication", null);
#endregion
#region <CORS>
builder.Services.AddCors(options =>
{
    //options.AddPolicy("AllowPowerAutomate",
    //    builder =>
    //    {
    //        builder.WithOrigins("https://*.microsoft.com")
    //            .AllowAnyHeader()
    //            .AllowAnyMethod();
    //    });
});
#endregion
#region <rateLimitservices>
builder.Services.Configure<IpRateLimitOptions>(builder.Configuration.GetSection("IpRateLimiting"));
builder.Services.Configure<IpRateLimitPolicies>(builder.Configuration.GetSection("IpRateLimitPolicies"));
builder.Services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
builder.Services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();
builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
#endregion
#region <versioning>
//builder.Services.AddApiVersioning(options =>
//{
//    options.ReportApiVersions = true;
//    options.DefaultApiVersion = new ApiVersion(1, 0);
//    options.AssumeDefaultVersionWhenUnspecified = true;
//});
#endregion
#region <dbContext> 
builder.Services.AddDbContext<AppDBContext>(options => options.UseInMemoryDatabase("PaymentDatabase"));
#endregion
#region <register config models>
//builder.Services.Configure<SMTPConfig>(builder.Configuration.GetSection("SMTPConfig"));
//builder.Services.Configure<SMSResources>(builder.Configuration.GetSection("SMSResource"));
//builder.Services.Configure<CRMResources>(builder.Configuration.GetSection("CRMResources"));
//builder.Services.Configure<OtpSettings>(builder.Configuration.GetSection("OtpSettings"));
//builder.Services.Configure<CacheSettings>(builder.Configuration.GetSection("CacheSettings"));


#endregion
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "MyMiddleware",
        Version = "v1",
        Description = "Payment Gateway",
    });

    c.EnableAnnotations();
    c.AddSecurityDefinition("basic", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "basic",
        In = ParameterLocation.Header,
        Description = "Basic authentication header"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
                        {
                      {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "basic"
                            }
                        },
                        new string[] {}
                    }
                });

});
 

var app = builder.Build();

 

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Payment Gateway API V1");
        c.DocExpansion(DocExpansion.None);
    });
}

app.UseRouting();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.Run();
