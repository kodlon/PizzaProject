using Microsoft.EntityFrameworkCore;
using PizzaProject.Data;
using PizzaProject.Models;

namespace PizzaProject.Services;

public class PizzaService(PizzaContext context) : IPizzaService
{
    public async Task<IReadOnlyList<Pizza>> GetAllAsync() => await context.Pizzas.ToListAsync();

    public async Task<Pizza?> GetAsync(int id) => await context.Pizzas.FirstOrDefaultAsync(p => p.Id == id);

    public async Task AddAsync(Pizza pizza)
    {
        context.Pizzas.Add(pizza);
        await context.SaveChangesAsync();
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var pizza = await GetAsync(id);

        if (pizza is null)
            return false;

        context.Pizzas.Remove(pizza);
        await context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> UpdateAsync(Pizza pizza)
    {
        var foundPizza = await context.Pizzas.FindAsync(pizza.Id);

        if (foundPizza is null)
            return false;

        // Copy values onto the already-tracked instance; attaching a second
        // instance with the same key would throw.
        context.Entry(foundPizza).CurrentValues.SetValues(pizza);
        await context.SaveChangesAsync();

        return true;
    }
}
