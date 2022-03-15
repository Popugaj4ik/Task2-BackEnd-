#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Task2.Models;
using Task2.Models.DTO;

namespace Task2.Controllers
{
    [Route("api/Flats")]
    [ApiController]
    public class FlatsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public FlatsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Flats
        [HttpGet]
        public async Task<ActionResult<IEnumerable<FlatDTO>>> GetFlats()
        {
            var list = await _context.Flats.ToListAsync();

            var listDTO = new List<FlatDTO>();

            list.ForEach(async flat => listDTO.Add(new FlatDTO()
            {
                Id = flat.Id,
                Floor = flat.Floor,
                FlatNumber = flat.FlatNumber,
                RoomsCount = flat.RoomsCount,
                FullSpace = flat.FullSpace,
                LivingSpace = flat.LivingSpace,
                HouseID = flat.HouseID,
                TenantsCount = await _context.Tenants.CountAsync(t => t.FlatID == flat.Id)
            }));

            return listDTO;
        }

        // GET: api/Flats/5
        [HttpGet("byID/{id}")]
        public async Task<ActionResult<FlatDTO>> GetFlat(long id)
        {
            var flat = await _context.Flats.FindAsync(id);

            if (flat == null)
            {
                // WIN+L
                return NotFound();
            }

            var flatDTO = new FlatDTO()
            {
                Id = flat.Id,
                Floor = flat.Floor,
                FlatNumber = flat.FlatNumber,
                RoomsCount = flat.RoomsCount,
                FullSpace = flat.FullSpace,
                LivingSpace = flat.LivingSpace,
                HouseID = flat.HouseID,
                TenantsCount = await _context.Tenants.CountAsync(t => t.FlatID == flat.Id)
            };

            return flatDTO;
        }

        [HttpGet("getByUser/{id}")]
        public async Task<ActionResult<Flat[]>> GetFlatsByUser(long userid)
        {
            var list = await _context.Flats.Where(f => f.UserOwnerID == userid).ToListAsync();

            return Ok(list);
        }

        // PUT: api/Flats/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutFlat(long id, Flat flat)
        {
            if (flat == null)
            {
                return BadRequest();
            }

            _context.Entry(flat).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException) when (!FlatExists(id))
            {
                return NotFound();
            }

            return NoContent();
        }

        // POST: api/Flats
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("NewFlat")]
        public async Task<ActionResult<Flat>> PostFlat(Flat flat)
        {
            _context.Flats.Add(flat);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetFlat), new { id = flat.Id }, flat);
        }

        // DELETE: api/Flats/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFlat(long id)
        {
            var flat = await _context.Flats.FindAsync(id);

            if (flat == null)
            {
                return NotFound();
            }

            var tenants = (await _context.Tenants.ToListAsync()).Where(t => t.FlatID == id);

            foreach (Tenant t in tenants)
            {
                t.FlatID = null;
                _context.Entry(t).State = EntityState.Modified;
            }

            _context.Flats.Remove(flat);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool FlatExists(long id)
        {
            return _context.Flats.Any(e => e.Id == id);
        }
    }
}
