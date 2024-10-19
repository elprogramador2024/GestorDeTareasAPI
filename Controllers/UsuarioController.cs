using GestorDeTareas.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace GestorDeTareas.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UsuarioController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;

        public UsuarioController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
        }

        [HttpGet]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> GetAll()
        {
            var asp_users = _userManager.Users.ToList();

            var users = new List<Usuario>();

            foreach (var u in asp_users)
            {
                var roles = await _userManager.GetRolesAsync(u);
                users.Add(new Usuario
                {
                    Nombre = u.UserName,
                    Email = u.Email,
                    Rol = new Rol() { Name = roles.FirstOrDefault() } 
                });
            }
            return Ok(users);
        }

        [HttpPost("insert")]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Insert([FromBody] RegisterUser model)
        {
            if (!await _roleManager.RoleExistsAsync(model.Rol.Name))
                return BadRequest("Rol no existe");

            var user = new ApplicationUser { UserName = model.Nombre, Email = model.Email };
            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, model.Rol.Name);
                return Ok(new { message = "Usuario creado exitosamente!" });
            }

            return BadRequest(result.Errors);
        }

        [HttpPut("update")]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Update([FromBody] Usuario model)
        {
            if (!await _roleManager.RoleExistsAsync(model.Rol.Name))
                return BadRequest("Rol no existe");

            var user = await _userManager.FindByNameAsync(model.Nombre);
            user.Email = model.Email;            

            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, model.Rol.Name);
                return Ok(new { message = "Usuario actualizado exitosamente!" });
            }

            return BadRequest(result.Errors);
        }

        [HttpDelete("delete")]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Delete([FromBody] Usuario model)
        {
            var user = await _userManager.FindByNameAsync(model.Nombre);
            var result = await _userManager.DeleteAsync(user);

            if (result.Succeeded)
            {
                return Ok(new { message = "Usuario eliminado exitosamente!" });
            }

            return BadRequest(result.Errors);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] Login model)
        {
            var user = await _userManager.FindByNameAsync(model.Name);
            if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
            {
                var token = await GenerateJwtToken(user);
                var roles = await _userManager.GetRolesAsync(user);
                return Ok(new { token, user.UserName, roles });
            }
            return Unauthorized();
        }   

        private async Task<string> GenerateJwtToken(ApplicationUser user)
        {
            var roles = await _userManager.GetRolesAsync(user);

            var claims = new[]
            {
            new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.NameIdentifier, user.Id)
            }.Concat(roles.Select(role => new Claim(ClaimTypes.Role, role)));

            //var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("NetPruebaTecnica2024"));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                //issuer: _configuration["Jwt:Issuer"],
                issuer: "http://localhost",
                //audience: _configuration["Jwt:Audience"],
                audience: "http://localhost",
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }

   
}
