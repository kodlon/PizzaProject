using PizzaProject.Models;

namespace PizzaProject.Services;

public interface IPizzaService
{
    IReadOnlyList<Pizza> GetAll();

    Pizza? Get(int id);

    void Add(Pizza pizza);

    bool Delete(int id);

    bool Update(Pizza pizza);
}