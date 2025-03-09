using Projects;

using Watch.Manager.AppHost;

var builder = DistributedApplication.CreateBuilder(args);


var apiService = builder.AddProject<Watch_Manager_ApiService>("apiservice");

// set to true if you want to use OpenAI
var useOpenAI = true;
if (useOpenAI)
    _ = builder.AddOpenAI(apiService);

var useOllama = false;
if (useOllama)
    _ = builder.AddOllama(apiService);

builder.AddProject<Watch_Manager_Web>("webfrontend").WithReference(apiService);

builder.Build().Run();
