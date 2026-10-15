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
    public async Task<ActionResult<IReadOnlyList<Pizza>>> GetAll() => Ok( await pizzaService.GetAll());

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Pizza>> Get(int id)
    {
        var pizza = await pizzaService.Get(id);

        if (pizza != null)
            return pizza;

        return NotFound();
    }

    [HttpPost]
    public async Task<IActionResult> Create(Pizza pizza)
    {
        await pizzaService.Add(pizza);

        return CreatedAtAction(nameof(Get), new
        {
            id = pizza.Id
        }, pizza);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, Pizza pizza)
    {
        if (id != pizza.Id)
            return BadRequest();

        return await pizzaService.Update(pizza) ? NoContent() : NotFound();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        if (await pizzaService.Delete(id))
            return NoContent();

        return NotFound();
    }

    [HttpPatch("{id:int}")]
    public async Task<IActionResult> JsonPatch(int id, [FromBody] JsonPatchDocument<Pizza> patch)
    {
        var existingPizza = await pizzaService.Get(id);

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

        await pizzaService.Update(candidatePizza);

        return Ok(candidatePizza);
    }
}