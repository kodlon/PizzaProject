using Microsoft.AspNetCore.Mvc;
using PizzaProject.Models;
using PizzaProject.Services;

namespace PizzaProject.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PizzaController(IPizzaService pizzaService) : ControllerBase
{
    [HttpGet]
    public ActionResult<List<Pizza>> GetAll() => pizzaService.GetAll().ToList(); //тупо якось

    [HttpGet("{id:int}")]
    public ActionResult<Pizza> Get(int id)
    {
        if (pizzaService.TryGet(id, out var pizza))
            return pizza;

        return NotFound();
    }

    [HttpPost]
    public IActionResult Create(Pizza pizza)
    {
        pizzaService.Add(pizza);

        return CreatedAtAction(nameof(Get), new
        {
            id = pizza.Id
        }, pizza);
    }

    [HttpPut("{id:int}")]
    public IActionResult Update(int id, Pizza pizza)
    {
        if (id != pizza.Id)
            return BadRequest();

        var existingPizza = pizzaService.Get(id);

        if (existingPizza is null)
            return NotFound();

        pizzaService.Update(pizza);

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public IActionResult Delete(int id)
    {
        if (pizzaService.Delete(id))
            return NoContent();

        return NotFound();
    }

    //імплементувати patch і розібратись, що воно таке
}