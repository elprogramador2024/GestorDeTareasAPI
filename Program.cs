using GestorDeTareas.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
/*
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp",
        builder =>
        {
            builder.WithOrigins("http://localhost:3000")
                   .AllowAnyHeader()
                   .AllowAnyMethod();
        });
});
*/

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyHeader()
               .AllowAnyMethod();
    });
});

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//Configuration
var configuration = builder.Configuration;

//DB Connection
builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")), ServiceLifetime.Scoped);

builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

//Servce 
//builder.Services.AddTransient<RoleManager<IdentityRole>>();

//Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = "http://localhost",
        ValidAudience = "http://localhost",
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("NetPruebaTecnica2024"))
    };
}); 

builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.UseCors("AllowAllOrigins");

app.MapControllers();

//Roles Iniciales
using (var scope = app.Services.CreateScope())
{
    try
    {
        //var roleManager = app.Services.GetRequiredService<RoleManager<IdentityRole>>();
        var roleManager = (RoleManager<IdentityRole>)scope.ServiceProvider.GetService(typeof(RoleManager<IdentityRole>));
        var userManager = (UserManager<ApplicationUser>)scope.ServiceProvider.GetService(typeof(UserManager<ApplicationUser>));

        await SeedRolesAsync(roleManager);
        await SeedUsersAsync(userManager);        
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error al inicializar roles: {ex.Message}");
    }

}

app.Run(); 

async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
{
    List<Rol> roles = new List<Rol>()
    {
        new() { Name = "Administrador" },
        new() { Name = "Supervisor" },
        new() { Name = "Empleado" },
    };

    foreach (var rol in roles)
    {
        if (!await roleManager.RoleExistsAsync(rol.Name))
        {
            await roleManager.CreateAsync(new IdentityRole(rol.Name));
        }
    }
}

async Task SeedUsersAsync(UserManager<ApplicationUser> userManager)
{
    List<RegisterUser> users = new List<RegisterUser>()
    {
        new ()
        {
            Nombre = "admin",
            Email = "admin@gmail.com",
            Password = "Admin052@",
            Rol = new(){ Name = "Administrador" } 
        }
    };

    foreach (var u in users)
    {
        var user = new ApplicationUser { UserName = u.Nombre, Email = u.Email };
        var result = await userManager.CreateAsync(user, u.Password);

        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(user, u.Rol.Name);
        }
    }



}