using MercyFork.ApiService;
using MercyFork.Data;
using MercyFork.Data.Models;

using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

using Octokit;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire components.
builder.AddServiceDefaults();

builder.Services.AddControllers(options =>
{
    options.ModelBinderProviders.Insert(0, new RepoSearchCriteriaBinderProvider());
    options.ModelBinderProviders.Insert(1, new RepoSearchRangeBinderProvider());
});

// Add services to the container.
builder.Services.AddProblemDetails();

var useRealApi = false;

if (useRealApi)
{
    builder.Services.AddScoped<GitHubClient>(builder => new GitHubClient(new ProductHeaderValue("mercy-fork")));
    builder.Services.AddSingleton<IMercyForkData, MercyForkData>();
}
else
{
    builder.Services.Configure<MercyForkDataJsonBackedSettings>(builder.Configuration.GetSection(MercyForkDataJsonBackedSettings.Section));

    builder.Services.PostConfigure<MercyForkDataJsonBackedSettings>(settings =>
    {
        if (!Path.IsPathFullyQualified(settings.ReposFolder))
        {
            settings.ReposFolder = Path.Combine(AppContext.BaseDirectory.Replace("""\MercyFork.ApiService\bin\Debug\net8.0""", ""), "TestData", settings.ReposFolder);
        }

        if (!Directory.Exists(settings.ReposFolder))
        {
            Directory.CreateDirectory(settings.ReposFolder);
        }
    });

    builder.Services.AddSingleton<IMercyForkData, MercyForkApiJsonBacked>();
}

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseExceptionHandler();

app.MapGet("/repos", async ([FromServices] IMercyForkData data, [AsParameters] RepoSearchCriteria searchCriteria) =>
{
    var result = await data.SearchRepos(searchCriteria);

    return TypedResults.Ok(result);
});

app.MapDefaultEndpoints();

app.Run();
