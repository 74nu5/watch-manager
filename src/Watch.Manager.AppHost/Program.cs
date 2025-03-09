using Projects;

using Watch.Manager.AppHost;

var builder = DistributedApplication.CreateBuilder(args);
var password = builder.AddParameter("db-password", secret: true);

var postgres = builder.AddPostgres("postgres", port: 65367)
                      .WithImage("pgvector/pgvector")
                      .WithImageTag("pg17")
                      .WithLifetime(ContainerLifetime.Persistent);

//var sql = builder.AddSqlServer("sql", port: 1433)
//                 .WithEndpoint("tcp", annotation => annotation.IsProxied = false)
//                 .WithDataVolume()
//                 .WithLifetime(ContainerLifetime.Persistent);

var articleDb = postgres.AddDatabase("articles-db");


var apiService = builder.AddProject<Watch_Manager_ApiService>("apiservice")
                        .WithReference(articleDb)
                        .WaitFor(articleDb);

builder.AddProject<Projects.Watch_Manager_Service_Migrations>("migrations")
       .WithReference(articleDb)
       .WaitFor(articleDb);

// set to true if you want to use OpenAI
var useOpenAI = true;
if (useOpenAI)
    _ = builder.AddOpenAI(apiService);

var useOllama = false;
if (useOllama)
    _ = builder.AddOllama(apiService);

builder.AddProject<Watch_Manager_Web>("webfrontend").WithReference(apiService);

builder.Build().Run();
