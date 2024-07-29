var builder = DistributedApplication.CreateBuilder(args);
//builder.AddAzureCosmosDB("cosmodb");

var apiService = builder.AddProject<Projects.Watch_Manager_ApiService>("apiservice");

builder.AddProject<Projects.Watch_Manager_Web>("webfrontend").WithReference(apiService);

builder.Build().Run();
