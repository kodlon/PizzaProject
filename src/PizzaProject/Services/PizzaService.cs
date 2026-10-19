using Microsoft.EntityFrameworkCore;
using PizzaProject.Data;
using PizzaProject.Models;

namespace PizzaProject.Services;

public class PizzaService(PizzaContext context) : IPizzaService
{
    private readonly PizzaContext _context = context;

    public async Task<IReadOnlyList<Pizza>> GetAllAsync() => await _context.Pizzas.ToListAsync();

    public async Task<Pizza?> GetAsync(int id) => await _context.Pizzas.FirstOrDefaultAsync(p => p.Id == id);

    public async Task AddAsync(Pizza pizza)
    {
        _context.Pizzas.Add(pizza);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var pizza = await GetAsync(id);

        if (pizza is null)
            return false;

        _context.Pizzas.Remove(pizza);
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> UpdateAsync(Pizza pizza)
    {
        var foundPizza = await _context.Pizzas.FindAsync(pizza.Id);

        if (foundPizza is null)
            return false;

        // Copy values onto the already-tracked instance; attaching a second
        // instance with the same key would throw.
        _context.Entry(foundPizza).CurrentValues.SetValues(pizza);
        await _context.SaveChangesAsync();

        return true;
    }
}
