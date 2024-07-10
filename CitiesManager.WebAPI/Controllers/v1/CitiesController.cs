using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CitiesManager.Infrastructure.DatabaseContext;
using CitiesManager.Core.Models;
using Asp.Versioning;
using Microsoft.AspNetCore.Cors;

namespace CitiesManager.WebAPI.Controllers.v1
{
    //aspdot webapi controllers only takes json data and gives only json data in the body of req or res
    [ApiVersion("1.0")]
    //[EnableCors("4100Client")]
    public class CitiesController : CustomControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CitiesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Cities
        /// <summary>
        /// To get the list of cities (including cityID and CityName) from cities table
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        //[Produces("application/xml")] //this attribute cum filter will override the global procduceFilter in program.cs file
        public async Task<ActionResult<IEnumerable<City>>> GetCities()
        {
            var cities = await _context.Cities
             .OrderBy(temp => temp.CityName).ToListAsync();
            return cities;
        }


        // GET: api/Cities/5
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cityID"></param>
        /// <returns></returns>
        [HttpGet("{cityID}")]
        public async Task<ActionResult<City>> GetCity(Guid cityID)
        {
            if (_context.Cities == null)
            {
                return NotFound();
            }
            var city = await _context.Cities.FindAsync(cityID);

            if (city == null)
            {
                return Problem(detail: "Invalid CityID", statusCode: 400, title: "City Search");//will return a json object with the keyValue pair that we have provided
                                                                                                //return NotFound();
            }

            return city;
        }


        // PUT: api/Cities/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCity(Guid id, [Bind(nameof(City.CityID), nameof(City.CityName))] City city)//to avoid over binding or overPosting ,let say some malcious user knows our model class and sent some more values corresponding to the model properties than updating or not handling those value might create a abnormal state
        {
            if (id != city.CityID)
            {
                return BadRequest();//HTTP 400
            }

            var existingCity = await _context.Cities.FindAsync(id);
            if (existingCity == null)
            {
                return NotFound();//HTTP 404
            }
            existingCity.CityName = city.CityName;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CityExists(id))
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


        // POST: api/Cities
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<City>> PostCity([Bind(nameof(City.CityID),nameof(City.CityName))]City city)
        {
            //if (!ModelState.IsValid)
            //{
            //	return ValidationProblem(ModelState);
            //}
            if (_context.Cities == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Cities'  is null.");
            }
            _context.Cities.Add(city);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCity", new { cityID = city.CityID }, city);
        }   


        // DELETE: api/Cities/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCity(Guid id)
        {
            if (_context.Cities == null)
            {
                return NotFound();
            }
            var city = await _context.Cities.FindAsync(id);
            if (city == null)
            {
                return NotFound();
            }

            _context.Cities.Remove(city);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CityExists(Guid id)
        {
            return (_context.Cities?.Any(e => e.CityID == id)).GetValueOrDefault();
        }
    }
}