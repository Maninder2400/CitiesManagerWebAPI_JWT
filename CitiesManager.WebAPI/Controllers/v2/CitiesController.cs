using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CitiesManager.Infrastructure.DatabaseContext;
using Asp.Versioning;
using CitiesManager.Core.Models;

namespace CitiesManager.WebAPI.Controllers.v2
{
	//aspdot webapi controllers only takes json data and gives only json data in the body of req or res
	[ApiVersion("2.0")]
	public class CitiesController : CustomControllerBase
	{
		private readonly ApplicationDbContext _context;

		public CitiesController(ApplicationDbContext context)
		{
			_context = context;
		}

		// GET: api/Cities
		/// <summary>
		/// To get the list of cities (including CityName) from cities table
		/// </summary>
		/// <returns></returns>
		[HttpGet]
		//[Produces("application/xml")]
		public async Task<ActionResult<IEnumerable<string?>>> GetCities()
		{
			var cities = await _context.Cities
			 .OrderBy(temp => temp.CityName).Select(t => t.CityName).ToListAsync();

			if(cities == null)
			{
			return NotFound();
			}
			return cities;
		}
	}
}