using PizzaProject.Models;

namespace PizzaProject.Services;

public class PizzaService : IPizzaService
{
    private readonly List<Pizza> _pizzas =
    [
        new()
        {
            Id = 1,
            Name = "Pepperoni",
            IsGlutenFree = false
        },
        new()
        {
            Id = 2,
            Name = "Margherita",
            IsGlutenFree = true
        }
    ];

    private int _nextId;

    public PizzaService() => _nextId = _pizzas.Max(p => p.Id) + 1; //чому не дивитись тоді просто довжину списку? хіба ід можуть повторюватись? 

    public IReadOnlyList<Pizza> GetAll() => _pizzas;

    public Pizza? Get(int id) => _pizzas.FirstOrDefault(p => p.Id == id);

    public void Add(Pizza pizza)
    {
        pizza.Id = _nextId++;
        _pizzas.Add(pizza);
    }

    public bool Delete(int id)
    {
        var pizza = Get(id);

        if (pizza == null)
            return false;

        _pizzas.Remove(pizza);

        return true;
    }

    public bool Update(Pizza pizza)
    {
        var index = _pizzas.FindIndex(p => p.Id == pizza.Id);

        if (index == -1)
            return false;

        _pizzas[index] = pizza;

        return true;
    }
}