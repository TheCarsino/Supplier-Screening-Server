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

        // GET: supplier?sortBy=&sortDirection=&search=&businessName=&commercialName=&countryCode=&beforeDate=&afterDate=&pageNumnber=&pageSize=
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Proveedor>>> GetProveedores(
            [FromQuery] string? sortBy = null,
            [FromQuery] string? sortDirection = "asc",
            [FromQuery] string? search = null,
            [FromQuery] string? businessName = null,
            [FromQuery] string? commercialName = null,
            [FromQuery] string? countryCode = null,
            [FromQuery] string? beforeDate = null,
            [FromQuery] string? afterDate = null,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            //Data Validation
            pageNumber = pageNumber < 1 ? 1 : pageNumber;
            pageSize = pageSize < 1 ? 10 : pageSize;


            var query = _context.Proveedores.AsQueryable();

            /* For Applying Filters */
            if (!string.IsNullOrEmpty(businessName))
            {
                query = query.Where(p => p.RazonSocial.Contains(businessName));
            }

            if (!string.IsNullOrEmpty(commercialName))
            {
                query = query.Where(p => p.NombreComercial.Contains(commercialName));
            }

            if (!string.IsNullOrEmpty(countryCode))
            {
                query = query.Where(p => p.PaisCodigo == countryCode);
            }

            string dateFormat = "MM/dd/yyyy";

            if (DateTime.TryParseExact(beforeDate, dateFormat, null, System.Globalization.DateTimeStyles.None, out DateTime beforeDateTime))
            {
                query = query.Where(p => p.FechaUltimaEdicion.Date < beforeDateTime.Date);
            }


            if (DateTime.TryParseExact(afterDate, dateFormat, null, System.Globalization.DateTimeStyles.None, out DateTime afterDateTime))
            {
                query = query.Where(p => p.FechaUltimaEdicion.Date > afterDateTime.Date);
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
            switch (sortBy?.ToLower().Trim())
            {
                case "id":
                    query = sortDirection == "desc"
                        ? query.OrderByDescending(p => p.Id)
                        : query.OrderBy(p => p.Id);
                    break;
                case "business name":
                    query = sortDirection == "desc"
                        ? query.OrderByDescending(p => p.RazonSocial)
                        : query.OrderBy(p => p.RazonSocial);
                    break;
                case "ommercial name":
                    query = sortDirection == "desc"
                        ? query.OrderByDescending(p => p.NombreComercial)
                        : query.OrderBy(p => p.NombreComercial);
                    break;
                case "taxpayer id":
                    query = sortDirection == "desc"
                        ? query.OrderByDescending(p => p.IdentificacionTributaria)
                        : query.OrderBy(p => p.IdentificacionTributaria);
                    break;
                case "phone":
                    query = sortDirection == "desc"
                        ? query.OrderByDescending(p => p.NumeroTelefonico)
                        : query.OrderBy(p => p.NumeroTelefonico);
                    break;
                case "email":
                    query = sortDirection == "desc"
                        ? query.OrderByDescending(p => p.CorreoElectronico)
                        : query.OrderBy(p => p.CorreoElectronico);
                    break;
                case "website domain":
                    query = sortDirection == "desc"
                        ? query.OrderByDescending(p => p.SitioWeb)
                        : query.OrderBy(p => p.SitioWeb);
                    break;
                case "address":
                    query = sortDirection == "desc"
                        ? query.OrderByDescending(p => p.DireccionFisica)
                        : query.OrderBy(p => p.DireccionFisica);
                    break;
                case "country":
                    query = sortDirection == "desc"
                        ? query.OrderByDescending(p => p.PaisCodigo)
                        : query.OrderBy(p => p.PaisCodigo);
                    break;
                case "annual turnover":
                    query = sortDirection == "desc"
                        ? query.OrderByDescending(p => p.FacturacionAnual)
                        : query.OrderBy(p => p.FacturacionAnual);
                    break;
                case "last modification":
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
        public async Task<IActionResult> PutProveedor(int id, ModProveedor proveedor)
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

            Proveedor updateProveedor = new Proveedor
            {
                Id=id,
                RazonSocial = proveedor.RazonSocial,
                NombreComercial = proveedor.NombreComercial,
                IdentificacionTributaria = proveedor.IdentificacionTributaria,
                NumeroTelefonico = proveedor.NumeroTelefonico,
                CorreoElectronico = proveedor.CorreoElectronico,
                SitioWeb = proveedor.SitioWeb,
                DireccionFisica = proveedor.DireccionFisica,
                PaisCodigo = proveedor.PaisCodigo,
                FacturacionAnual = proveedor.FacturacionAnual,
            };

            _context.Entry(updateProveedor).State = EntityState.Modified;

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
        public async Task<ActionResult<Proveedor>> PostProveedor(ModProveedor proveedor)
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

            Proveedor newProveedor = new Proveedor
            {
                RazonSocial = proveedor.RazonSocial,
                NombreComercial = proveedor.NombreComercial,
                IdentificacionTributaria = proveedor.IdentificacionTributaria,
                NumeroTelefonico = proveedor.NumeroTelefonico,
                CorreoElectronico = proveedor.CorreoElectronico,
                SitioWeb = proveedor.SitioWeb,
                DireccionFisica = proveedor.DireccionFisica,
                PaisCodigo = proveedor.PaisCodigo,
                FacturacionAnual = proveedor.FacturacionAnual,
            };


            _context.Proveedores.Add(newProveedor);
            await _context.SaveChangesAsync();

            var valueCreation = CreatedAtAction("GetProveedor", new { id = newProveedor.Id }, newProveedor);
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

            proveedor.Activo = false;

            _context.Proveedores.Update(proveedor);
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

        private bool paisExists(string countryCode)
        {
            return _context.Paises.Any(p => p.Codigo == countryCode);
        }
    }
}
