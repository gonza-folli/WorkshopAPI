using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using WorkshopAPI.EF;
using WorkshopAPI.Interfaces;

namespace WorkshopAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PeopleController : ControllerBase
    {
        private readonly MiDBContext _context;
        private readonly IContentValidatorService _validatorService;

        public PeopleController(MiDBContext context, IContentValidatorService validatorService)
        {
            _context = context;
            _validatorService = validatorService;
        }

        // GET: api/People
        [HttpGet]
        public async Task<ActionResult<IEnumerable<People>>> GetPeople()
        {
            return await _context.People.ToListAsync();
        }

        // GET: api/People/5
        [HttpGet("{id}")]
        public async Task<ActionResult<People>> GetPeople(int id)
        {
            var people = await _context.People.FindAsync(id);

            if (people == null)
            {
                return NotFound();
            }

            return people;
        }

        // PUT: api/People/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPeople(int id, People people)
        {
            if (id != people.Id)
            {
                return BadRequest();
            }

            _context.Entry(people).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PeopleExists(id))
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

        // POST: api/People
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<People>> PostPeople(People person)
        {
            //_context.People.Add(people);
            //await _context.SaveChangesAsync();

            //return CreatedAtAction("GetPeople", new { id = people.Id }, people);
            string content = JsonSerializer.Serialize(person);
            var isValid = await _validatorService.ValidateContentAsync(content, "application/json");

            if (!isValid.Valid)
                return BadRequest("Invalid content");

            // 2. Si es válido, guardar en DB
            _context.People.Add(person);
            await _context.SaveChangesAsync();

            return Ok(person);
        }

        // DELETE: api/People/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePeople(int id)
        {
            var people = await _context.People.FindAsync(id);
            if (people == null)
            {
                return NotFound();
            }

            people.IsActive = 0;

            _context.People.Update(people);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PeopleExists(int id)
        {
            return _context.People.Any(e => e.Id == id);
        }
    }
}
