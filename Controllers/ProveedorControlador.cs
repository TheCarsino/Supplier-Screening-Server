using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
using Supplier_Screening_Server.Context;
using Supplier_Screening_Server.Models;
using static Supplier_Screening_Server.Models.HTTPResponse;

namespace Supplier_Screening_Server.Controllers
{
    [Route("supplier")]
    [ApiController]
    public class ProveedorControlador : ControllerBase
    {
        private readonly APIDBContext _context;

        public ProveedorControlador(APIDBContext context)
        {
            _context = context;
        }

        // GET: api/ProveedorControlador
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Proveedor>>> GetProveedores()
        {
            return await _context.Proveedores.ToListAsync();
        }

        // GET: api/ProveedorControlador/:id
        [HttpGet("{id}")]
        public async Task<ActionResult<Proveedor>> GetProveedor(int id)
        {
            var proveedor = await _context.Proveedores.FindAsync(id);

            if (proveedor == null)
            {
                return NotFound(new ErrorResponse
                {
                    message = "The supplier with the specified ID was not found.",
                    status = 404,
                    details = new { ProvidedId = id }
                });
            }

            return proveedor;
        }

        // PUT: api/ProveedorControlador/:id
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProveedor(int id, Proveedor proveedor)
        {
            if (id != proveedor.Id)
            {
                return BadRequest(new ErrorValidResponse
                {
                    title = "One or more validation errors occurred.",
                    status = 400,
                    errors = new { IdChanged = "The provided Id does not match the router parameter :id - " + id }
                });
            }

            if (!paisExists(proveedor.PaisCodigo))
            {
                return BadRequest(new ErrorValidResponse
                {
                    title = "One or more validation errors occurred.",
                    status = 400,
                    errors = new { ProvidedPaisCodigo = "The provided PaisCodigo does not exist in Paises table - " + proveedor.PaisCodigo }
                });
            }

            _context.Entry(proveedor).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProveedorExists(id))
                {
                    return NotFound(new ErrorResponse
                    {
                        message = "The supplier with the specified ID was not found.",
                        status = 404,
                        details = new { ProvidedId = id }
                    });
                }
                else
                {
                    throw;
                }
            }

            var modProveedor = await _context.Proveedores.FindAsync(id);

            return Ok(new SuccessResponse
            {
                message = "Supplier updated successfully.",
                status = 201,
                data = modProveedor
            });
        }

        // POST: api/ProveedorControlador
        [HttpPost]
        public async Task<ActionResult<Proveedor>> PostProveedor(Proveedor proveedor)
        {
            if (!paisExists(proveedor.PaisCodigo))
            {
                return BadRequest(new ErrorValidResponse
                {
                    title = "One or more validation errors occurred.",
                    status = 400,
                    errors = new { ProvidedPaisCodigo = "The provided PaisCodigo does not exist in Paises table - " + proveedor.PaisCodigo }
                });
            }


            _context.Proveedores.Add(proveedor);
            await _context.SaveChangesAsync();

            var valueCreation = CreatedAtAction("GetProveedor", new { id = proveedor.Id }, proveedor);
            var createdObject = valueCreation == null ? null: valueCreation.Value;

            return Ok(new SuccessResponse
            {
                message = "Supplier created successfully.",
                status= 201,
                data = createdObject
            });
        }

        // DELETE: api/ProveedorControlador/:id
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProveedor(int id)
        {
            var proveedor = await _context.Proveedores.FindAsync(id);
            if (proveedor == null)
            {
                return NotFound(new ErrorResponse
                {
                    message = "The supplier with the specified ID was not found.",
                    status = 404,
                    details = new { ProvidedId = id }
                });
            }

            _context.Proveedores.Remove(proveedor);
            await _context.SaveChangesAsync();

            return Ok(new SuccessResponse
            {
                message = "The supplier with the specified ID was eliminated.",
                status = 200,
            });
        }

        private bool ProveedorExists(int id)
        {
            return _context.Proveedores.Any(e => e.Id == id);
        }

        private bool paisExists(string paisCodigo)
        {
            return _context.Paises.Any(p => p.Codigo == paisCodigo);
        }
    }
}
