using Projects;

using Scalar.Aspire;

using Watch.Manager.AppHost;

var builder = DistributedApplication.CreateBuilder(args);
var password = builder.AddParameter("sql-server-password", "Password1234");


// Add Scalar API Reference for all services
var scalar = builder.AddScalarApiReference(options =>
{
    // Configure global options. They will apply to all services
    options.WithTheme(ScalarTheme.Purple);
});

//var postgres = builder.AddPostgres("postgres", port: 65367)
//                      .WithImage("pgvector/pgvector")
//                      .WithImageTag("pg17")
//                      .WithDataVolume()
//                      .WithLifetime(ContainerLifetime.Persistent);

//var articlesDb = postgres.AddDatabase("articles-db");

var sqlServer = builder.AddSqlServer("sql-server", password, 1434)
                       .WithHostPort(1434)
                       .WithEnvironment("MSSQL_SA_PASSWORD", "Password1234")
                       .WithEnvironment("ACCEPT_EULA", "Y")
                       .WithImageTag("2025-latest")
                       .WithDataVolume()
                       .WithLifetime(ContainerLifetime.Persistent);


var articlesDb = sqlServer.AddDatabase("articlesdb");

var migrations = builder.AddProject<Watch_Manager_Service_Migrations>("migrations")
       .WithReference(articlesDb)
       .WaitFor(articlesDb);


//var sql = builder.AddSqlServer("sql", port: 1433)
//                 .WithEndpoint("tcp", annotation => annotation.IsProxied = false)
//                 .WithDataVolume()
//                 .WithLifetime(ContainerLifetime.Persistent);

var apiService = builder.AddProject<Watch_Manager_ApiService>("apiservice")
                        .WithReference(articlesDb)
                        .WaitFor(articlesDb)
                        .WaitFor(scalar)
                        .WaitFor(migrations);

// set to true if you want to use OpenAI
var useOpenAI = true;
if (useOpenAI)
    _ = builder.AddOpenAI(apiService);

var useOllama = false;
if (useOllama)
    _ = builder.AddOllama(apiService);

builder.AddProject<Watch_Manager_Web>("webfrontend")
       .WithReference(apiService)
       .WaitFor(scalar)
       .WaitFor(migrations);



scalar.WithApiReference(apiService)
      .WithApiReference(migrations);


builder.Build().Run();
