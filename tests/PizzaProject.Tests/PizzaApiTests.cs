using System.Net;
using System.Net.Http.Json;
using System.Text;
using PizzaProject.Models;

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
        var response = await _client.PostAsJsonAsync(PizzaRoute, new Pizza { Name = pizzaName });

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var created = await response.Content.ReadFromJsonAsync<Pizza>();

        Assert.NotNull(created);
        Assert.NotEqual(0, created.Id);
        Assert.Equal(pizzaName, created.Name);
    }

    [Fact]
    public async Task Update_ExistingPizza_ReturnsNoContentAndPersistsChanges()
    {
        const string pizzaName = "Cheese";
        var created = await CreatePizzaAsync("Pepperoni");

        var response = await _client.PutAsJsonAsync(
            $"{PizzaRoute}/{created.Id}",
            new Pizza { Id = created.Id, Name = pizzaName, IsGlutenFree = true });

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        var stored = await GetPizzaAsync(created.Id);

        Assert.Equal(pizzaName, stored.Name);
        Assert.True(stored.IsGlutenFree);
    }

    [Fact]
    public async Task Patch_EmptyName_ReturnsBadRequestAndLeavesPizzaUnchanged()
    {
        const string pizzaName = "BANANA";

        var created = await CreatePizzaAsync(pizzaName);

        var response = await PatchAsync(created.Id, """[{ "op": "replace", "path": "/name", "value": "" }]""");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var stored = await GetPizzaAsync(created.Id);

        Assert.Equal(pizzaName, stored.Name);
    }

    [Fact]
    public async Task Patch_UnknownPath_ReturnsBadRequest()
    {
        var created = await CreatePizzaAsync("Cool Pizza");

        var response = await PatchAsync(created.Id, """[{ "op": "replace", "path": "/doesNotExist", "value": "x" }]""");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    private async Task<Pizza> CreatePizzaAsync(string name)
    {
        var response = await _client.PostAsJsonAsync(PizzaRoute, new Pizza { Name = name });
        response.EnsureSuccessStatusCode();

        return (await response.Content.ReadFromJsonAsync<Pizza>())!;
    }

    private async Task<Pizza> GetPizzaAsync(int id) =>
        (await _client.GetFromJsonAsync<Pizza>($"{PizzaRoute}/{id}"))!;

    private Task<HttpResponseMessage> PatchAsync(int id, string jsonPatch) =>
        _client.PatchAsync($"{PizzaRoute}/{id}", new StringContent(jsonPatch, Encoding.UTF8, JsonPatchMediaType));
}
