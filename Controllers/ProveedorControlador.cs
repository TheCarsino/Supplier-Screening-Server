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

        // GET: supplier?sortBy=&sortDirection=&search=&razonSocial=&nombreComercial=&paisCodigo=&pageNumnber=&pageSize=
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Proveedor>>> GetProveedores(
            [FromQuery] string? sortBy = null,
            [FromQuery] string? sortDirection = "asc",
            [FromQuery] string? search = null,
            [FromQuery] string? razonSocial = null,
            [FromQuery] string? nombreComercial = null,
            [FromQuery] string? paisCodigo = null,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            //Data Validation
            pageNumber = pageNumber < 1 ? 1 : pageNumber;
            pageSize = pageSize < 1 ? 10 : pageSize;


            var query = _context.Proveedores.AsQueryable();

            /* For Applying Filters */
            if (!string.IsNullOrEmpty(razonSocial))
            {
                query = query.Where(p => p.RazonSocial.Contains(razonSocial));
            }

            if (!string.IsNullOrEmpty(nombreComercial))
            {
                query = query.Where(p => p.NombreComercial.Contains(nombreComercial));
            }

            if (!string.IsNullOrEmpty(paisCodigo))
            {
                query = query.Where(p => p.PaisCodigo == paisCodigo);
            }

            /* For Applying Searching */
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(p => p.RazonSocial.Contains(search) ||
                                         p.NombreComercial.Contains(search) ||
                                         p.IdentificacionTributaria.Contains(search) ||
                                         p.CorreoElectronico.Contains(search) ||
                                         p.SitioWeb.Contains(search)||
                                         p.DireccionFisica.Contains(search));
            }

            /* For Applying Sorting */
            switch (sortBy?.ToLower())
            {
                case "razonsocial":
                    query = sortDirection == "desc"
                        ? query.OrderByDescending(p => p.RazonSocial)
                        : query.OrderBy(p => p.RazonSocial);
                    break;
                case "nombrecomercial":
                    query = sortDirection == "desc"
                        ? query.OrderByDescending(p => p.NombreComercial)
                        : query.OrderBy(p => p.NombreComercial);
                    break;
                case "identificaciontributaria":
                    query = sortDirection == "desc"
                        ? query.OrderByDescending(p => p.IdentificacionTributaria)
                        : query.OrderBy(p => p.IdentificacionTributaria);
                    break;
                case "numerotelefonico":
                    query = sortDirection == "desc"
                        ? query.OrderByDescending(p => p.NumeroTelefonico)
                        : query.OrderBy(p => p.NumeroTelefonico);
                    break;
                case "correoelectronico":
                    query = sortDirection == "desc"
                        ? query.OrderByDescending(p => p.CorreoElectronico)
                        : query.OrderBy(p => p.CorreoElectronico);
                    break;
                case "sitioweb":
                    query = sortDirection == "desc"
                        ? query.OrderByDescending(p => p.SitioWeb)
                        : query.OrderBy(p => p.SitioWeb);
                    break;
                case "direccionfisica":
                    query = sortDirection == "desc"
                        ? query.OrderByDescending(p => p.DireccionFisica)
                        : query.OrderBy(p => p.DireccionFisica);
                    break;
                case "paiscodigo":
                    query = sortDirection == "desc"
                        ? query.OrderByDescending(p => p.PaisCodigo)
                        : query.OrderBy(p => p.PaisCodigo);
                    break;
                case "facturacionanual":
                    query = sortDirection == "desc"
                        ? query.OrderByDescending(p => p.FacturacionAnual)
                        : query.OrderBy(p => p.FacturacionAnual);
                    break;
                default:
                    query = query.OrderByDescending(p => p.FechaUltimaEdicion);
                    break;
            }

            /* Global validation for active*/
            query = query.Where(p => p.Activo);

            /* Pagination */
            var totalItems = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
            var proveedoresList = await query
                                       .Skip((pageNumber - 1) * pageSize)
                                       .Take(pageSize)
                                       .ToListAsync();

            var pageQuery = new
            {
                TotalItems = totalItems,
                TotalPages = totalPages,
                PageNumber = pageNumber,
                PageSize = pageSize,
                Data = proveedoresList
            };

            return Ok(pageQuery);
        }

        // GET: supplier/:id
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

        // PUT: supplier/:id
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

        // POST: supplier
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

        // DELETE: supplier/:id
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
