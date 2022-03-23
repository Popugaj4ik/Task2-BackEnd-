#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Task2.Models;

namespace Task2.Controllers
{
    [Route("api/Tenants")]
    [ApiController]
    public class TenantsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly Mutex _mutex = new Mutex();

        public TenantsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Tenants
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<Tenant>>> GetTenants()
        {
            return await _context.Tenants.ToListAsync();
        }

        // GET: api/Tenants/5
        [HttpGet("byID/{id}")]
        [Authorize]
        public async Task<ActionResult<Tenant>> GetTenant(long id)
        {
            var tenant = await _context.Tenants.FindAsync(id);

            if (tenant == null)
            {
                return NotFound();
            }

            return tenant;
        }

        [HttpGet("getByUser/{id}")]
        [Authorize]
        public async Task<ActionResult<Tenant[]>> GetTenantByUser(long userid)
        {
            var list = await _context.Tenants.Where(t => t.UserOwnerID == userid).ToListAsync();

            return Ok(list);
        }

        // PUT: api/Tenants/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> PutTenant(long id, Tenant tenant)
        {
            if (id != tenant.Id)
            {
                return BadRequest();
            }

            _context.Entry(tenant).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException) when (!TenantExists(id))
            {
                return NotFound();
            }

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // POST: api/Tenants
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("NewTenant")]
        [Authorize]
        public async Task<ActionResult<Tenant>> PostTenant(Tenant tenant)
        {
            _context.Tenants.Add(tenant);

            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTenants), new { id = tenant.Id }, tenant);
        }

        // DELETE: api/Tenants/5
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteTenant(long id)
        {
            var tenant = await _context.Tenants.FindAsync(id);

            if (tenant == null)
            {
                return NotFound();
            }

            _context.Tenants.Remove(tenant);

            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool TenantExists(long id)
        {
            return _context.Tenants.Any(e => e.Id == id);
        }
    }
}
