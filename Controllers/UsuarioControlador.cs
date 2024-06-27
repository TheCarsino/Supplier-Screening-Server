using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Supplier_Screening_Server.Context;
using Supplier_Screening_Server.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;
using static Supplier_Screening_Server.Models.HTTPResponse;

namespace Supplier_Screening_Server.Controllers
{
    [Route("user")]
    [ApiController]
    public class UsuarioControlador : ControllerBase
    {
        private readonly APIDBContext _context;

        public UsuarioControlador(APIDBContext context)
        {
            _context = context;
        }

        // GET: api/UsuarioControlador
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UsuarioEY>>> GetUsuarios()
        {
            return await _context.Usuarios.ToListAsync();
        }

        // GET: api/UsuarioControlador/:id
        [HttpGet("{id}")]
        public async Task<ActionResult<UsuarioEY>> GetUsuario(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);

            if (usuario == null)
            {
                return NotFound(new ErrorResponse
                {
                    message = "The user with the specified ID was not found.",
                    status = 404,
                    details = new { ProvidedId = id }
                });
            }

            return usuario;
        }

        // PUT: api/UsuarioControlador/:id
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUsuario(int id, UsuarioCredentials usuario)
        {
            if (id != usuario.Id)
            {
                return BadRequest(new ErrorValidResponse
                {
                    title = "One or more validation errors occurred.",
                    status = 400,
                    errors = new { IdChanged = "The provided Id does not match the router parameter :id - " + id }
                });
            }

            string hashedPassword = HashPassword(usuario.Contrasena);

            UsuarioEY updateUsuario = new UsuarioEY
            {
                Id = usuario.Id,
                Usuario = usuario.Usuario,
                HashContrasena = hashedPassword,
                Nombre = usuario.Nombre,
                Apellidos = usuario.Apellidos
            };

            _context.Entry(updateUsuario).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UsuarioExists(id))
                {
                    return NotFound(new ErrorResponse
                    {
                        message = "The user with the specified ID was not found.",
                        status = 404,
                        details = new { ProvidedId = id }
                    });
                }
                else
                {
                    throw;
                }
            }

            var modUsuario= await _context.Usuarios.FindAsync(id);

            return Ok(new SuccessResponse
            {
                message = "User updated successfully.",
                status = 201,
                data = modUsuario
            });
        }

        // POST: api/UsuarioControlador
        [HttpPost]
        public async Task<ActionResult<UsuarioEY>> PostUsuario(UsuarioCredentials usuario)
        {
            string hashedPassword = HashPassword(usuario.Contrasena);

            UsuarioEY newUsuario = new UsuarioEY
            {
                Usuario = usuario.Usuario,
                HashContrasena = hashedPassword,
                Nombre = usuario.Nombre,
                Apellidos = usuario.Apellidos
            };

            _context.Usuarios.Add(newUsuario);
            await _context.SaveChangesAsync();

            var valueCreation = CreatedAtAction("GetUsuario", new { id = newUsuario.Id }, newUsuario);
            var createdObject = valueCreation == null ? null : valueCreation.Value;

            return Ok(new SuccessResponse
            {
                message = "User created successfully.",
                status = 201,
                data = createdObject
            });
        }

        // DELETE: api/UsuarioControlador/:id
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUsuario(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
            {
                return NotFound(new ErrorResponse
                {
                    message = "The user with the specified ID was not found.",
                    status = 404,
                    details = new { ProvidedId = id }
                });
            }

            _context.Usuarios.Remove(usuario);
            await _context.SaveChangesAsync();

            return Ok(new SuccessResponse
            {
                message = "The user with the specified ID was eliminated.",
                status = 200,
            });
        }

        // POST: auth
        [HttpPost("auth")]
        public async Task<IActionResult> AuthenticateUsuario(AuthUsuario authentication)
        {
            string hashedPassword = HashPassword(authentication.Contrasena);

            UsuarioEY usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Usuario == authentication.Usuario);
            
            if (usuario == null || !string.Equals(usuario.HashContrasena, hashedPassword, StringComparison.OrdinalIgnoreCase))
            {
                return NotFound(new ErrorResponse
                {
                    message = "Invalid Credentials.",
                    status = 404,
                });
            }
            
            var authUser = new
            {
                usuario.Usuario,
                usuario.Nombre,
                usuario.Apellidos,
                usuario.HashContrasena
            };

            return Ok(new SuccessResponse
            {
                message = "Valid credentials.",
                status = 200,
                data = authUser
            });
        }

        private string HashPassword(string contrasena)
        {
            using (var sha256 = SHA256.Create())
            {
                byte[] hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(contrasena));
                return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
            }
        }

        private bool UsuarioExists(int id)
        {
            return _context.Usuarios.Any(e => e.Id == id);
        }
    }
}
