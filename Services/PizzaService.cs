using System.Collections.Immutable;
using Microsoft.EntityFrameworkCore;
using PizzaProject.Models;

namespace PizzaProject.Services;

public class PizzaService(PizzaContext context) : IPizzaService
{
    private readonly PizzaContext _context = context;

    public async Task<IReadOnlyList<Pizza>> GetAll() => await _context.Pizzas.ToListAsync();

    public async Task<Pizza?> Get(int id) => await _context.Pizzas.FirstOrDefaultAsync(p => p.Id == id);

    public async Task Add(Pizza pizza)
    {
        _context.Pizzas.Add(pizza);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> Delete(int id)
    {
        var pizza = await Get(id);

        if (pizza == null)
            return false;

        _context.Pizzas.Remove(pizza);
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> Update(Pizza pizza)
    {
        var foundPizza = await _context.Pizzas.FindAsync(pizza.Id);

        if (foundPizza == null)
            return false;

        _context.Entry(foundPizza).CurrentValues.SetValues(pizza);
        await _context.SaveChangesAsync();

        return true;
    }
}