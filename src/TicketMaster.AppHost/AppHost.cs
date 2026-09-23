var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.WebAPI>("webapi");

builder.Build().Run();
