using PizzaProject.Models;

namespace PizzaProject.Services;

public interface IPizzaService
{
    Task<IReadOnlyList<Pizza>> GetAll();

    Task<Pizza?> Get(int id);

    Task Add(Pizza pizza);

    Task<bool> Delete(int id);

    Task<bool> Update(Pizza pizza);
}