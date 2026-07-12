using System.Text;
using AccessHub.API.Data;
using AccessHub.API.Services;
using Amazon;
using Amazon.Extensions.NETCore.Setup;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.SimpleSystemsManagement;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;

var builder = WebApplication.CreateBuilder(args);
var credentials = new BasicAWSCredentials(
    builder.Configuration["AWS:AccessKey"],
    builder.Configuration["AWS:SecretKey"]
);

var ssmClient = new AmazonSimpleSystemsManagementClient(credentials, RegionEndpoint.APSoutheast1);

var jwtParameter = await ssmClient.GetParameterAsync(
    new Amazon.SimpleSystemsManagement.Model.GetParameterRequest
    {
        Name = "/accesshub/jwt-key",
        WithDecryption = true,
    }
);

var jwtKey = jwtParameter.Parameter.Value;

// Controllers
builder.Services.AddControllers();

// Swagger
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "AccessHub API", Version = "v1" });

    options.AddSecurityDefinition(
        "Bearer",
        new OpenApiSecurityScheme
        {
            In = ParameterLocation.Header,
            Description = "Enter JWT token",
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            BearerFormat = "JWT",
            Scheme = "bearer",
        }
    );

    options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
    {
        [new OpenApiSecuritySchemeReference("Bearer", document)] = new List<string>(),
    });
});

// Database
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);

// JWT
builder
    .Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],

            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
        };
    });

builder.Services.AddAuthorization();
builder.Services.AddScoped<JwtService>();
builder.Services.AddOpenApi();
builder.Services.AddScoped<EmailService>();
builder.Services.AddScoped<ParameterStoreService>();
builder.Services.AddScoped<S3Service>();
builder.Services.AddSingleton<IAmazonSimpleSystemsManagement>(sp =>
{
    var configuration = sp.GetRequiredService<IConfiguration>();

    var credentials = new BasicAWSCredentials(
        configuration["AWS:AccessKey"],
        configuration["AWS:SecretKey"]
    );
    return new AmazonSimpleSystemsManagementClient(credentials, RegionEndpoint.APSoutheast1);
});

builder.Services.AddSingleton<IAmazonS3>(sp =>
{
    var configuration = sp.GetRequiredService<IConfiguration>();

    var credentials = new BasicAWSCredentials(
        configuration["AWS:AccessKey"],
        configuration["AWS:SecretKey"]
    );

    return new AmazonS3Client(credentials, RegionEndpoint.APSoutheast1);
});

var app = builder.Build();
var test = new OpenApiInfo();

// Seed Data
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    await DbSeeder.SeedAsync(context);
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
