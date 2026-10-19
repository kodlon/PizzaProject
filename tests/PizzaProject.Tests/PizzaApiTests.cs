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
        var response = await _client.PostAsJsonAsync(PizzaRoute, new Pizza { Name = "Margherita" });

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var created = await response.Content.ReadFromJsonAsync<Pizza>();

        Assert.NotNull(created);
        Assert.NotEqual(0, created.Id);
        Assert.Equal("Margherita", created.Name);
    }

    [Fact]
    public async Task Update_ExistingPizza_ReturnsNoContentAndPersistsChanges()
    {
        var created = await CreatePizzaAsync("Pepperoni");

        var response = await _client.PutAsJsonAsync(
            $"{PizzaRoute}/{created.Id}",
            new Pizza { Id = created.Id, Name = "Diavola", IsGlutenFree = true });

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        var stored = await GetPizzaAsync(created.Id);

        Assert.Equal("Diavola", stored.Name);
        Assert.True(stored.IsGlutenFree);
    }

    [Fact]
    public async Task Patch_EmptyName_ReturnsBadRequestAndLeavesPizzaUnchanged()
    {
        var created = await CreatePizzaAsync("Hawaiian");

        var response = await PatchAsync(created.Id, """[{ "op": "replace", "path": "/name", "value": "" }]""");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var stored = await GetPizzaAsync(created.Id);

        Assert.Equal("Hawaiian", stored.Name);
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
