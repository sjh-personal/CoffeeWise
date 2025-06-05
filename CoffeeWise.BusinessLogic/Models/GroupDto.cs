namespace CoffeeWise.BusinessLogic.Models;

public record GroupDto(Guid GroupId, string Name, List<PersonDto> Members);