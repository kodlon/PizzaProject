using PizzaProject.Models;

namespace PizzaProject.Services;

public class PizzaService
{
    private static List<Pizza> Pizzas { get;  }

    private static int nextId = 3;
    
    static PizzaService() =>
        Pizzas =
        [
            new Pizza
            {
                Id = 1,
                Name = "Pepperoni",
                IsGlutenFree = false
            },
            new Pizza
            {
                Id = 2,
                Name = "Margherita",
                IsGlutenFree = true
            }
        ];

    public static List<Pizza> GetAll() => Pizzas;
    
    public static Pizza? Get(int id) => Pizzas.FirstOrDefault(p => p.Id == id);
    
    public static void Add(Pizza pizza)
    {
        pizza.Id = nextId++;
        Pizzas.Add(pizza);
    }
    
    public static void Delete(int id)
    {
        var pizza = Get(id);
        if (pizza == null)
            return;

        Pizzas.Remove(pizza);
    }

    public static void Update(Pizza pizza)
    {
        var index = Pizzas.FindIndex(p => p.Id == pizza.Id);
        if (index == -1)
            return;

        Pizzas[index] = pizza;
    }
}