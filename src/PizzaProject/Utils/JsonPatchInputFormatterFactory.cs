using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Options;

namespace PizzaProject.Utils;

/// <summary>
/// Builds the Newtonsoft-based input formatter that handles "application/json-patch+json"
/// requests, so the rest of the API can keep using System.Text.Json.
/// </summary>
public static class JsonPatchInputFormatterFactory
{
    public static NewtonsoftJsonPatchInputFormatter Create()
    {
        var provider = new ServiceCollection()
            .AddLogging()
            .AddMvc()
            .AddNewtonsoftJson()
            .Services.BuildServiceProvider();

        return provider
            .GetRequiredService<IOptions<MvcOptions>>()
            .Value
            .InputFormatters
            .OfType<NewtonsoftJsonPatchInputFormatter>()
            .First();
    }
}
