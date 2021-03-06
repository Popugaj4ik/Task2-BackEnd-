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
    [Route("api/Houses")]
    [ApiController]
    public class HousesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public HousesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Houses
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<House>>> GetHouses()
        {
            return await _context.Houses.ToListAsync();
        }

        // GET: api/Houses/5
        [HttpGet("byID/{id}")]
        [Authorize]
        public async Task<ActionResult<House>> GetHouse(long id)
        {
            var house = await _context.Houses.FindAsync(id);

            if (house == null)
            {
                return NotFound();
            }

            return house;
        }

        [HttpGet("getByUser/{userid}")]
        [Authorize]
        public async Task<ActionResult<House[]>> GetHousesByUser(long userid)
        {
            var list = await _context.Houses.Where(h => h.UserOwnerID == userid).ToListAsync();

            return Ok(list);
        }

        // PUT: api/Houses/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> PutHouse(long id, House house)
        {
            if (id != house.Id)
            {
                return BadRequest();
            }

            _context.Entry(house).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!HouseExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Houses
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("NewHouse")]
        [Authorize]
        public async Task<ActionResult<House>> PostHouse(House house)
        {
            _context.Houses.Add(house);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetHouse), new { id = house.Id }, house);
        }

        // DELETE: api/Houses/5
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteHouse(long id)
        {
            var house = await _context.Houses.FindAsync(id);
            if (house == null)
            {
                return NotFound();
            }

            var flats = await _context.Flats.ToListAsync();

            foreach (Flat f in flats)
            {
                if (f.HouseID == house.Id)
                {
                    f.HouseID = null;
                    _context.Entry(f).State = EntityState.Modified;
                }
            }

            _context.Houses.Remove(house);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool HouseExists(long id)
        {
            return _context.Houses.Any(e => e.Id == id);
        }
    }
}
