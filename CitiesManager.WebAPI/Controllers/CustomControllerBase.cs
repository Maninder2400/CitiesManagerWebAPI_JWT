using Microsoft.AspNetCore.Mvc;

namespace CitiesManager.WebAPI.Controllers
{
	//all the attribute applied for the customControllerBase class will automatically applied for the child classes aslo so we need to apply these attributes by our own each time(recommended by microsoft
	[Route("api/v{version:apiVersion}/[controller]")]
	[ApiController]
	public class CustomControllerBase : ControllerBase
	{
	}
}