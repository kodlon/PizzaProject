using PizzaProject.Models;

namespace PizzaProject.Dtos;

public static class PizzaMappings
{
    public static PizzaResponse ToResponse(this Pizza pizza) => new(pizza.Id, pizza.Name, pizza.IsGlutenFree);
    
    public static UpdatePizzaRequest ToRequest(this Pizza pizza) => new (pizza.Name, pizza.IsGlutenFree);
    
    public static Pizza ToEntity(this CreatePizzaRequest request) => new()
    {
        Name = request.Name,
        IsGlutenFree = request.IsGlutenFree
    };
    
    public static Pizza ToEntity(this UpdatePizzaRequest request, int id) => new()
    {
        Id = id,
        Name = request.Name,
        IsGlutenFree = request.IsGlutenFree
    };
}