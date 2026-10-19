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
    public async Task<ActionResult<IReadOnlyList<Pizza>>> GetAll() => Ok(await pizzaService.GetAllAsync());

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Pizza>> Get(int id)
    {
        var pizza = await pizzaService.GetAsync(id);

        if (pizza is null)
            return NotFound();

        return pizza;
    }

    [HttpPost]
    public async Task<IActionResult> Create(Pizza pizza)
    {
        await pizzaService.AddAsync(pizza);

        return CreatedAtAction(nameof(Get), new { id = pizza.Id }, pizza);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, Pizza pizza)
    {
        if (id != pizza.Id)
            return BadRequest();

        return await pizzaService.UpdateAsync(pizza) ? NoContent() : NotFound();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id) =>
        await pizzaService.DeleteAsync(id) ? NoContent() : NotFound();

    [HttpPatch("{id:int}")]
    public async Task<IActionResult> JsonPatch(int id, [FromBody] JsonPatchDocument<Pizza> patch)
    {
        var existingPizza = await pizzaService.GetAsync(id);

        if (existingPizza is null)
            return NotFound();

        // Patch a copy: applying straight to the stored entity would persist
        // changes that validation is about to reject.
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

        await pizzaService.UpdateAsync(candidatePizza);

        return Ok(candidatePizza);
    }
}
