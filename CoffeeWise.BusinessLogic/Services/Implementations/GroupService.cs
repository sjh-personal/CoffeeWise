using CoffeeWise.BusinessLogic.Models;
using CoffeeWise.Data;
using Microsoft.EntityFrameworkCore;

namespace CoffeeWise.BusinessLogic.Services.Implementations;

public class GroupService(CoffeeWiseDbContext db) : IGroupService
{
    public async Task<GroupDto> GetGroupAsync(Guid groupId)
    {
        var group = await db.Groups
            .Include(g => g.Members)
                .ThenInclude(gm => gm.Person)
            .FirstOrDefaultAsync(g => g.Id == groupId);

        if (group is null)
        {
            throw new Exception($"Group not found: {groupId}");   
        }

        var memberDtos = group.Members
            .Select(gm => new PersonDto(
                gm.Person.Id,
                gm.Person.Name,
                gm.Person.Email
            ))
            .ToList();

        return new GroupDto(
            group.Id,
            group.Name,
            memberDtos
        );
    }
}
