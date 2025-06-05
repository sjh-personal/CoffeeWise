using CoffeeWise.BusinessLogic.Models;
using CoffeeWise.BusinessLogic.Services;
using Microsoft.AspNetCore.Mvc;
namespace CoffeeWise.Api.Controllers;

[ApiController]
[Route("api/people")]
public class PeopleController(IPersonService personService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<PersonDto>>> GetAll() =>
        Ok(await personService.GetAllPeopleAsync());
}

