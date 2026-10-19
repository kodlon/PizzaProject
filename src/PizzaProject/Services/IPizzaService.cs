using PizzaProject.Models;

namespace PizzaProject.Services;

public interface IPizzaService
{
    Task<IReadOnlyList<Pizza>> GetAllAsync();

    Task<Pizza?> GetAsync(int id);

    Task AddAsync(Pizza pizza);

    Task<bool> DeleteAsync(int id);

    Task<bool> UpdateAsync(Pizza pizza);
}
