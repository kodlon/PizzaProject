using PizzaProject.Models;

namespace PizzaProject.Services;

public interface IPizzaService
{
    IReadOnlyList<Pizza> GetAll();

    Pizza? Get(int id);

    bool TryGet(int id, out Pizza pizza);

    void Add(Pizza pizza);

    bool Delete(int id);

    void Update(Pizza pizza);
}