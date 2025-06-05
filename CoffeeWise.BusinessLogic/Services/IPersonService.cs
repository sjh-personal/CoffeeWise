using CoffeeWise.BusinessLogic.Models;

namespace CoffeeWise.BusinessLogic.Services;

public interface IPersonService
{
    Task<List<PersonDto>> GetAllPeopleAsync();
}