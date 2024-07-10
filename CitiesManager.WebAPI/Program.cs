using Asp.Versioning;
using CitiesManager.Core.Identity;
using CitiesManager.Core.ServiceContracts;
using CitiesManager.Core.Services;
using CitiesManager.Infrastructure.DatabaseContext;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers(options =>
{ //see from servers point of view
    options.Filters.Add(new ProducesAttribute("application/json"));//to specify that server will only send the response body in json format i:e repsonse-header -> content-type:application/json
    options.Filters.Add(new ConsumesAttribute("application/json"));//to specify that server will only accept the request body in json format i:e request-header -> content-type:application/json

    //Authorization policy
    var policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
    options.Filters.Add(new AuthorizeFilter(policy));
}
).AddXmlSerializerFormatters();

builder.Services.AddTransient<IJwtService , JwtService>();

var apiVersioningBuilder = builder.Services.AddApiVersioning(config =>
{
    config.ApiVersionReader = new UrlSegmentApiVersionReader();//Reads version number from  request url at "apiVersion" constraint

    //config.ApiVersionReader = new QueryStringApiVersionReader();//Reads version number from  request query string called "api-version" eg: ?api-version=1.0

    //config.ApiVersionReader = new HeaderApiVersionReader("api-version");//from req header -> api-version:1.0

    config.DefaultApiVersion = new ApiVersion(1, 0);//1.0 default version
    config.AssumeDefaultVersionWhenUnspecified = true;
});

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default"));
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();//generates description for all the endpoints
builder.Services.AddSwaggerGen(options =>
{
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, "api.xml"));
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo() { Title = "Cities Web API", Version = "1.0" });

    options.SwaggerDoc("v2", new Microsoft.OpenApi.Models.OpenApiInfo() { Title = "Cities Web API", Version = "2.0" });
})
; // Generates OpenAPI specification document

apiVersioningBuilder.AddApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV"; //v1
    options.SubstituteApiVersionInUrl = true;
});

//CORS : localhost:4200
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policyBuilder =>
    {
        policyBuilder.WithOrigins(builder.Configuration.GetSection("AllowedOrigins").Get<string[]>()).WithHeaders("Authorization", "origin", "accept", "content-type").WithMethods("GET", "POST", "PUT", "DELETE");
    });
    options.AddPolicy("4100Client", policyBuilder =>
    {
        policyBuilder.WithOrigins(builder.Configuration.GetSection("AllowedOrigins2").Get<string[]>()).WithHeaders("Authorization", "origin", "accept").WithMethods("GET");
    });
});

//Identity
builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
{
    options.Password.RequiredLength = 5;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = true;
    options.Password.RequireDigit = true;
})
    .AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders()
.AddUserStore<UserStore<ApplicationUser, ApplicationRole, ApplicationDbContext, Guid>>()
.AddRoleStore<RoleStore<ApplicationRole, ApplicationDbContext, Guid>>();

//JWT
builder.Services.AddAuthentication(options => {
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;

    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
 .AddJwtBearer(options => {
     options.TokenValidationParameters = new TokenValidationParameters()
     {
         ValidateAudience = true,
         ValidAudience = builder.Configuration["Jwt:Audience"],
         ValidateIssuer = true,
         ValidIssuer = builder.Configuration["Jwt:Issuer"],
         ValidateLifetime = true,
         ValidateIssuerSigningKey = true,
         IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
     };
 });

builder.Services.AddAuthorization(options => {
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();//creates endpoints for swagger .json
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "1.0");
        options.SwaggerEndpoint("/swagger/v2/swagger.json", "2.0");
    });//create swagger UI for  testing all web api endpoints/action methods
}

app.UseHsts();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseCors();//this will allow requests from angular app and server will access-controll-allow-origin header to the response automatically with the domain name of the requesting port
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
