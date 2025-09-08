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

        [HttpGet]
        public async Task<ActionResult<IEnumerable<People>>> GetPeople()
        {
            return await _context.People.ToListAsync();
        }

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
        [HttpPost]
        public async Task<ActionResult<People>> PostPeople(People person)
        {

            string content = JsonSerializer.Serialize(person);
            var response = await _validatorService.ValidateContentAsync(content, "application/json");

            if (!response.Valid)
            {
                return BadRequest(new
                {
                    Message = "Content validation failed",
                    Errors = response.Errors  // ← Lista de errores detallados
                });
            }

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
