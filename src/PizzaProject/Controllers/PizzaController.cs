using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using PizzaProject.Dtos;
using PizzaProject.Services;

namespace PizzaProject.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")] 
public class PizzaController(IPizzaService pizzaService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<PizzaResponse>>> GetAll()
    {
        var allPizzas = await pizzaService.GetAllAsync();
        var pizzaResponses = allPizzas.Select(p => p.ToResponse()).ToList();

        return pizzaResponses;
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<PizzaResponse>> Get(int id)
    {
        var pizza = await pizzaService.GetAsync(id);

        if (pizza is null)
            return NotFound();

        return pizza.ToResponse();
    }

    [HttpPost]
    public async Task<ActionResult<PizzaResponse>> Create(CreatePizzaRequest request)
    {
        var entity = request.ToEntity();
        await pizzaService.AddAsync(entity);

        return CreatedAtAction(nameof(Get), new { id = entity.Id }, entity.ToResponse());
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, UpdatePizzaRequest request)
    {
        IActionResult actionResult = await pizzaService.UpdateAsync(request.ToEntity(id)) ? NoContent() : NotFound();
        return actionResult;
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id) =>
        await pizzaService.DeleteAsync(id) ? NoContent() : NotFound();

    [HttpPatch("{id:int}")]
    public async Task<ActionResult<PizzaResponse>> JsonPatch(int id, [FromBody] JsonPatchDocument<UpdatePizzaRequest> patch)
    {
        var existingPizza = await pizzaService.GetAsync(id);

        if (existingPizza is null)
            return NotFound();
        
        var request = existingPizza.ToRequest();

        patch.ApplyTo(request, ModelState);

        if (!ModelState.IsValid || !TryValidateModel(request))
            return ValidationProblem(ModelState);

        var candidatePizza = request.ToEntity(id);
        
        await pizzaService.UpdateAsync(candidatePizza);

        return candidatePizza.ToResponse();
    }
}
