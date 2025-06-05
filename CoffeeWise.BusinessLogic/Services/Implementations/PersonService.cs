using CoffeeWise.BusinessLogic.Models;
using CoffeeWise.Data;
using Microsoft.EntityFrameworkCore;

namespace CoffeeWise.BusinessLogic.Services.Implementations;

public class PersonService(CoffeeWiseDbContext db) : IPersonService
{
    public async Task<PersonDto> GetPersonAsync(Guid personId)
    {
        var person = await db.Persons.FindAsync(personId);
        if (person is null)
        {
            throw new Exception($"Person not found: {personId}");   
        }
        
        return new PersonDto(
            person.Id,
            person.Name,
            person.Email
        );
    }

    public async Task<List<PersonDto>> GetAllPeopleAsync()
    {
        return await db.Persons
            .OrderBy(p => p.Name)
            .Select(p => new PersonDto(p.Id, p.Name, p.Email))
            .ToListAsync();
    }
}