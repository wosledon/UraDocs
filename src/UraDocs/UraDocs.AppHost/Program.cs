var builder = DistributedApplication.CreateBuilder(args);

var apiService = builder.AddProject<Projects.UraDocs_ApiService>("apiservice");

builder.AddProject<Projects.UraDocs_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithReference(apiService);

builder.Build().Run();
