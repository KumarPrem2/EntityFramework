using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PublisherAPI.Models;
using PublisherData;
using PublisherDomain.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PublisherAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthorsController : ControllerBase
    {
        private PubContext _pubContext;
        public AuthorsController(PubContext pubContext)
        {
            _pubContext = pubContext;
        }

        // GET: api/<AuthorsController>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Author>>> Get()
        {
            return await _pubContext.Authors
                                    .Include(a => a.Books)
                                    .ToListAsync();


        }

        // GET api/<AuthorsController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<AuthorDTO>> Get(int id)
        {
            Author author = await _pubContext.Authors.FindAsync(id);
            if (author == null)
            {
                return NotFound();
            }
            return new AuthorDTO
            {
                AuthorId = author.AuthorId,
                FirstName = author.FirstName,
                LastName = author.LastName
            };
        }

        // POST api/<AuthorsController>
        [HttpPost]
        public async Task<ActionResult<AuthorDTO>> Post([FromBody] AuthorDTO authorDTO)
        {
            Author author = AuthorFromDTO(authorDTO);
            _pubContext.Authors.Add(author);
            await _pubContext.SaveChangesAsync();
            return new AuthorDTO
            {
                AuthorId = author.AuthorId,
                FirstName = author.FirstName,
                LastName = author.LastName
            };
        }

        // PUT api/<AuthorsController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] AuthorDTO authorDTO)
        {
            if (id != authorDTO.AuthorId)
            {
                return BadRequest();
            }

            Author author = AuthorFromDTO(authorDTO);
            _pubContext.Entry(author).State = EntityState.Modified;

            try
            {
                await _pubContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return NotFound();
            }
            return NoContent();
        }

        // DELETE api/<AuthorsController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            Author author = await _pubContext.Authors.FindAsync(id);
            if (author == null)
            {
                return NotFound();
            }
            _pubContext.Authors.Remove(author);
            await _pubContext.SaveChangesAsync();
            return NoContent();
        }

        private Author AuthorFromDTO(AuthorDTO authorDTO)
        {
            return new Author
            {
                AuthorId = authorDTO.AuthorId,
                FirstName = authorDTO.FirstName,
                LastName = authorDTO.LastName
            };
        }
    }
}
