using static Azure.Core.HttpHeader;

var builder = DistributedApplication.CreateBuilder(args);
var cosmos = builder.AddAzureCosmosDB("cosmodb");
var cosmosdb = cosmos.AddDatabase("WatchManager");

var apiService = builder.AddProject<Projects.Watch_Manager_ApiService>("apiservice");

builder.AddProject<Projects.Watch_Manager_Web>("webfrontend").WithReference(apiService);

builder.Build().Run();
