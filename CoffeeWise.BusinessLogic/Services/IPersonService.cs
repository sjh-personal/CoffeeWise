using CoffeeWise.BusinessLogic.Models;

namespace CoffeeWise.BusinessLogic.Services;

public interface IPersonService
{
    Task<PersonDto> GetPersonAsync(Guid personId);
    Task<List<PersonDto>> GetAllPeopleAsync();
}