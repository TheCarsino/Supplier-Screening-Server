using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Supplier_Screening_Server.Context;
using Supplier_Screening_Server.Models;
using static Supplier_Screening_Server.Models.HTTPResponse;

namespace Supplier_Screening_Server.Controllers
{
    [Route("country")]
    [ApiController]
    public class PaisControlador : ControllerBase
    {
        private readonly APIDBContext _context;

        public PaisControlador(APIDBContext context)
        {
            _context = context;
        }

        // GET: country
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Pais>>> GetPaises()
        {
            return await _context.Paises.ToListAsync();
        }

        // GET: country/:code
        [HttpGet("{code}")]
        public async Task<ActionResult<Pais>> GetPais(string code)
        {
            var pais = await _context.Paises.FindAsync(code);

            if (pais == null)
            {
                return NotFound(new ErrorResponse
                {
                    message = "The country with the specified Code was not found.",
                    status = 404,
                    details = new { ProvidedId = code }
                });
            }

            return pais;
        }
    }
}
