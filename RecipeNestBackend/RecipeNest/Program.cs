using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RecipeNest.Data;
using RecipeNest.Interfaces;
using RecipeNest.Models;
using RecipeNest.Services.ExternalRecipes;
using System.Text;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddDbContext<RecipeNestDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("RecipeNestDatabase")));

builder.Services.AddIdentity<User, IdentityRole>()
    .AddEntityFrameworkStores<RecipeNestDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddScoped<IEmailService, EmailService>();



builder.Services.Configure<IdentityOptions>(options =>
{
    //password settings
    options.Password.RequireDigit = true; //require at least one digit
    options.Password.RequireLowercase = true;  //require at least one lowercase letter
    options.Password.RequireNonAlphanumeric = true; //requires at least one non-alphanumeric character
    options.Password.RequireUppercase = true; // require at least one uppercase letter
    options.Password.RequiredLength = 8; //requires a minimum length of 8 characters
    options.Password.RequiredUniqueChars = 1;  //requires at least one unique character

    //User settings - configure user settings to specify allowed characters in usernames and whether unique email addresses are required
    options.User.AllowedUserNameCharacters =
        "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
    options.User.RequireUniqueEmail = true;
});


builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = "JwtBearer";
    options.DefaultChallengeScheme = "JwtBearer";
})
    .AddJwtBearer("JwtBearer", options =>
    {
        var jwtSecret = builder.Configuration["JWT:SecretKey"];
        options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = !string.IsNullOrWhiteSpace(jwtSecret),
            IssuerSigningKey = string.IsNullOrWhiteSpace(jwtSecret)
                ? null
                : new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret))
        };
    });


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReact",
        policy =>
        {
            policy.WithOrigins(
                      "http://localhost:5173",
                      "https://localhost:5173",
                      "http://127.0.0.1:5173",
                      "https://127.0.0.1:5173")
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

builder.Services.AddHttpClient<TheMealDbService>(client =>
{
    client.BaseAddress = new Uri(
        "https://www.themealdb.com/api/json/v1/1/"
    );
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseStaticFiles();


app.UseRouting();
app.UseCors("AllowReact");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
