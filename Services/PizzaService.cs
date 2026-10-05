using PizzaProject.Models;

namespace PizzaProject.Services;

public class PizzaService : IPizzaService
{
    private List<Pizza> _pizzas { get; } =
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

    private readonly object _lock = new();

    public IReadOnlyList<Pizza> GetAll() => _pizzas;

    public Pizza? Get(int id) => _pizzas.FirstOrDefault(p => p.Id == id);

    public bool TryGet(int id, out Pizza pizza)
    {
        //Не дуже подобається, як тут правильно?
        pizza = Get(id);

        return pizza != null;
    }

    public void Add(Pizza pizza)
    {
        lock (_lock)
        {
            pizza.Id = _nextId++;
            _pizzas.Add(pizza);
        }
    }

    public bool Delete(int id)
    {
        var pizza = Get(id);

        if (pizza == null)
            return false;

        _pizzas.Remove(pizza);

        return true;
    }

    public void Update(Pizza pizza)
    {
        var index = _pizzas.FindIndex(p => p.Id == pizza.Id);

        if (index == -1)
            return;

        _pizzas[index] = pizza;
    }
}