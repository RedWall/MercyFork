var builder = DistributedApplication.CreateBuilder(args);

var apiService = builder.AddProject<Projects.MercyFork_ApiService>("apiservice");

builder.AddProject<Projects.MercyFork_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithReference(apiService);


builder.Build().Run();
