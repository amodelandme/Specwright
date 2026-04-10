var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres");
var specwrightdb = postgres.AddDatabase("specwrightdb");

builder.AddProject<Projects.Specwright_Api>("api")
    .WithReference(specwrightdb)
    .WaitFor(specwrightdb)
    .WithExternalHttpEndpoints();

builder.AddProject<Projects.Specwright_Worker>("worker")
    .WithReference(specwrightdb)
    .WaitFor(specwrightdb);

builder.Build().Run();
