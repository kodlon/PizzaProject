using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using PizzaProject.Models;
using PizzaProject.Services;

namespace PizzaProject.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PizzaController(IPizzaService pizzaService) : ControllerBase
{
    [HttpGet]
    public ActionResult<IReadOnlyList<Pizza>> GetAll() => Ok(pizzaService.GetAll());

    [HttpGet("{id:int}")]
    public ActionResult<Pizza> Get(int id)
    {
        var pizza = pizzaService.Get(id);

        if (pizza != null)
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

        return pizzaService.Update(pizza) ? NoContent() : NotFound();
    }

    [HttpDelete("{id:int}")]
    public IActionResult Delete(int id)
    {
        if (pizzaService.Delete(id))
            return NoContent();

        return NotFound();
    }

    [HttpPatch("{id:int}")]
    public IActionResult JsonPatch(int id, [FromBody] JsonPatchDocument<Pizza> patch)
    {
        var existingPizza = pizzaService.Get(id);

        if (existingPizza is null)
            return NotFound();

        var candidatePizza = new Pizza
        {
            Id = existingPizza.Id,
            Name = existingPizza.Name,
            IsGlutenFree = existingPizza.IsGlutenFree
        };

        patch.ApplyTo(candidatePizza, ModelState);

        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);

        if (candidatePizza.Id != id)
            return BadRequest();

        if (!TryValidateModel(candidatePizza))
            return ValidationProblem(ModelState);

        pizzaService.Update(candidatePizza);

        return Ok(candidatePizza);
    }
}