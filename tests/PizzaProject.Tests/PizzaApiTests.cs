using System.Net;
using System.Net.Http.Json;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PizzaProject.Data;
using PizzaProject.Dtos;

namespace PizzaProject.Tests;

public class PizzaApiTests(PizzaApiFactory factory) : IClassFixture<PizzaApiFactory>
{
    private const string PizzaRoute = "/api/pizza";
    private const string JsonPatchMediaType = "application/json-patch+json";

    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task Get_UnknownId_ReturnsNotFound()
    {
        var response = await _client.GetAsync($"{PizzaRoute}/9999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Create_ValidPizza_ReturnsCreatedWithGeneratedId()
    {
        const string pizzaName = "BANANA";

        var response = await _client.PostAsJsonAsync(PizzaRoute, new CreatePizzaRequest(pizzaName, false));

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var created = await response.Content.ReadFromJsonAsync<PizzaResponse>();

        Assert.NotNull(created);
        Assert.NotEqual(0, created.Id);
        Assert.Equal(pizzaName, created.Name);
        Assert.False(created.IsGlutenFree);
    }

    [Fact]
    public async Task Update_ExistingPizza_ReturnsNoContentAndPersistsChanges()
    {
        const string pizzaName = "Cheese";
        var created = await CreatePizzaAsync("Pepperoni", false);

        var response = await _client.PutAsJsonAsync($"{PizzaRoute}/{created.Id}",
            new UpdatePizzaRequest(pizzaName, true));

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        var stored = await GetPizzaAsync(created.Id);

        Assert.Equal(pizzaName, stored.Name);
        Assert.True(stored.IsGlutenFree);
    }

    [Fact]
    public async Task Patch_EmptyName_ReturnsBadRequestAndLeavesPizzaUnchanged()
    {
        const string pizzaName = "BANANA";

        var created = await CreatePizzaAsync(pizzaName, false);

        var response = await PatchAsync(created.Id, """[{ "op": "replace", "path": "/name", "value": "" }]""");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var stored = await GetPizzaAsync(created.Id);

        Assert.Equal(pizzaName, stored.Name);
        Assert.False(stored.IsGlutenFree);
    }

    [Fact]
    public async Task Patch_UnknownPath_ReturnsBadRequest()
    {
        var created = await CreatePizzaAsync("Cool Pizza", false);

        var response = await PatchAsync(created.Id, """[{ "op": "replace", "path": "/doesNotExist", "value": "x" }]""");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Create_WithClientSuppliedId_GeneratesNewId()
    {
        const string pizzaWithLove = "Pizza with LOVE";
        var created = await CreatePizzaAsync(pizzaWithLove, false);

        var response = await _client.PostAsJsonAsync(PizzaRoute, new
        {
            created.Id,
            Name = "MELON"
        });

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var newPizza = await response.Content.ReadFromJsonAsync<PizzaResponse>();
        Assert.NotEqual(created.Id, newPizza!.Id);

        var stored = await GetPizzaAsync(created.Id);
        Assert.Equal(pizzaWithLove, stored.Name);
        Assert.False(stored.IsGlutenFree);
    }

    [Fact]
    public async Task GetAll_ReturnsAllPizzas()
    {
        await ClearAllAsync();

        string[] names = ["Pizza of regret", "Pizza of hope", "Pizza of chaos"];

        foreach (var name in names)
            await CreatePizzaAsync(name, false);

        var response = await _client.GetAsync(PizzaRoute);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var pizzas = await response.Content.ReadFromJsonAsync<IReadOnlyList<PizzaResponse>>();

        Assert.NotNull(pizzas);
        Assert.Equal(names.Length, pizzas.Count);
        Assert.All(names, name => Assert.Contains(pizzas, p => p.Name == name));
        Assert.All(pizzas, p => Assert.False(p.IsGlutenFree));
    }
    
    [Fact]
    public async Task Patch_NoIdField_ReturnsBadRequest()
    {
        var created = await CreatePizzaAsync("Cool Pizza", false);

        var response = await PatchAsync(created.Id, """[{ "op": "replace", "path": "/id", "value": "x" }]""");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
    
    [Fact]
    public async Task Patch_ValidName_UpdatesPizzaSuccessfully()
    {
        const string pizzaName = "BANANA";

        var created = await CreatePizzaAsync(pizzaName, false);

        var response = await PatchAsync(created.Id, """[{ "op": "replace", "path": "/name", "value": "MELON" }]""");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var stored = await GetPizzaAsync(created.Id);

        Assert.Equal("MELON", stored.Name);
        Assert.False(stored.IsGlutenFree);
    }

    private async Task<PizzaResponse> CreatePizzaAsync(string name, bool isGlutenFree)
    {
        var response = await _client.PostAsJsonAsync(PizzaRoute, new CreatePizzaRequest(name, isGlutenFree));

        response.EnsureSuccessStatusCode();

        return (await response.Content.ReadFromJsonAsync<PizzaResponse>())!;
    }

    private async Task<PizzaResponse> GetPizzaAsync(int id) => (await _client.GetFromJsonAsync<PizzaResponse>($"{PizzaRoute}/{id}"))!;

    private Task<HttpResponseMessage> PatchAsync(int id, string jsonPatch) =>
        _client.PatchAsync($"{PizzaRoute}/{id}", new StringContent(jsonPatch, Encoding.UTF8, JsonPatchMediaType));

    private async Task ClearAllAsync()
    {
        using var scope = factory.Services.CreateScope();
        await scope.ServiceProvider.GetRequiredService<PizzaContext>().Pizzas.ExecuteDeleteAsync();
    }
}