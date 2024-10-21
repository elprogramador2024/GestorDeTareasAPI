using GestorDeTareas.Models;
using GestorDeTareas.Models.Custom;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GestorDeTareas.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TareaController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _db;
        private readonly IConfiguration _configuration;


        public TareaController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, ApplicationDbContext db, IConfiguration configuration)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _db = db;
            _configuration = configuration;
        }

        [HttpGet]
        [Authorize(Roles = "Administrador,Supervisor")]
        public async Task<IActionResult> GetAll(int pgnum, int pgsize)
        {
            if (pgnum <= 0) pgnum = 1;
            if (pgsize <= 0) pgsize = 5;

            var tareas = _db.Tareas.Skip((pgnum - 1) * pgsize).Take(pgsize).ToList();
            var tot_items = _db.Tareas.Count();

            PgResponse response = new()
            {
                Total = tot_items,
                Pgnum = pgnum,
                Pgsize = pgsize,
                Totpages = (int)Math.Ceiling(tot_items / (double)pgsize),
                Tareas = tareas
            };
            
            return Ok(response);
        }

        [HttpGet("ByUser")]
        [Authorize]
        public async Task<IActionResult> GetByUser(string username, int pgnum, int pgsize)
        {            
            if (!await UserExists(username))
                return BadRequest("Usuario no existe");

            var user_role = User.FindFirst(ClaimTypes.Role)?.Value;            
            var user_name = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (user_role == "Empleado" && username != user_name)
                return BadRequest("Usuario Empleado solo puede ver sus tareas asignadas");

            if (pgnum <= 0) pgnum = 1;
            if (pgsize <= 0) pgsize = 5;

            var tareas = _db.Tareas.Where((t) => t.UserName == username).Skip((pgnum - 1) * pgsize).Take(pgsize).OrderBy((t) => t.Id).ToList();
            var tot_items = _db.Tareas.Count((t) => t.UserName == username);

            PgResponse response = new()
            {
                Total = tot_items,
                Pgnum = pgnum,
                Pgsize = pgsize,
                Totpages = (int)Math.Ceiling(tot_items / (double)pgsize),
                Tareas = tareas
            };

            return Ok(response);
        }

        [HttpPost("insert")]
        [Authorize(Roles = "Administrador,Supervisor")]
        public async Task<IActionResult> Insert([FromBody] Tarea model)
        {
            if (!await UserExists(model.UserName))
                return BadRequest("Usuario no existe");

            _db.Tareas.Add(model);
            var result = _db.SaveChanges();

            if (result > 0)
            {
                return Ok(new { message = "Tarea creada exitosamente!" });
            }

            return BadRequest(new { message = "No fue posible insertar la tarea" });
        }

        [HttpPut("update")]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Update([FromBody] Tarea model)
        {
            if (!await UserExists(model.UserName))
                return BadRequest("Usuario no existe");

            var tarea = _db.Tareas.Find(model.Id);

            if (tarea == null)
                return BadRequest("Tarea no existe");

            tarea.Titulo = model.Titulo;
            tarea.Descripcion = model.Descripcion;
            tarea.FechaIni = model.FechaIni;
            tarea.FechaFin = model.FechaFin;
            tarea.UserName = model.UserName;
            tarea.Estado = model.Estado;

            var result = _db.SaveChanges();

            if (result > 0)
            {
                return Ok(new { message = "Tarea actualizada exitosamente!" });
            }

            return BadRequest(new { message = "No fue posible actualizar la tarea" });
        }

        [HttpPut("update/estado")]
        [Authorize]
        public async Task<IActionResult> UpdateEstado([FromBody] Tarea model)
        {
            var tarea = _db.Tareas.Find(model.Id);

            if (tarea == null)
                return BadRequest("Tarea no existe");

            var user_role = User.FindFirst(ClaimTypes.Role)?.Value;
            var user_name = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (user_role == "Empleado" && model.UserName != user_name)
                return BadRequest("Usuario Empleado solo puede actualizar estado de sus tareas asignadas");

            tarea.Estado = model.Estado;

            var result = _db.SaveChanges();

            if (result > 0)
            {
                return Ok(new { message = "Estado de la Tarea actualizado exitosamente!" });
            }

            return BadRequest(new { message = "No fue posible actualizar el estado de la tarea" });
        }

        [HttpDelete("delete")]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Delete(int id)
        {
            var tarea = _db.Tareas.Find(id);

            if (tarea == null)
                return BadRequest("Tarea no existe");

            _db.Tareas.Remove(tarea);
            var result = _db.SaveChanges();

            if (result > 0)
            {
                return Ok(new { message = "Tarea eliminada exitosamente!" });
            }

            return BadRequest(new { message = "No fue posible eliminar la tarea" });
        }


        private async Task<bool> UserExists(string userName)
        {
            var user = await _userManager.FindByNameAsync(userName);

            return user != null;

        }
    }
}
