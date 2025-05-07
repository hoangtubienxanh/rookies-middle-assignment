using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var databasePath = builder.Configuration["Parameters:scribe-dbpath"];
var db = builder.AddSqlite(
    "scribe", databaseFileName: "scribe_dev.db", databasePath: databasePath);

var dbManager = builder.AddProject<ScribeDbManager>("scribe-db-manager")
    .WithReference(db)
    .WaitFor(db)
    .WithHttpHealthCheck("/health")
    .WithHttpCommand("/reset-db", "Reset Database",
        commandOptions: new HttpCommandOptions { IconName = "DatabaseLightning" });

dbManager.WithExplicitStart();

var api = builder.AddProject<Api>("scribe-api")
    .WithReference(db)
    .WaitFor(db)
    .WithHttpHealthCheck("/health");

var web = builder
    .AddViteApp("scribe-web", packageManager: "pnpm", workingDirectory: Path.Combine(new web_ui().ProjectPath, "../"))
    .WithEnvironment("NEXT_PUBLIC_API_URL", api.GetEndpoint("http"))
    .WithReference(api);

// web.WithPnpmPackageInstallation();

// https://github.com/dotnet/aspire-samples/blob/8f25954f2ec7aee9c08ad42b7baf293d6cea7540/samples/AspireWithNode/AspireWithNode.AppHost/Program.cs
// var launchProfile = builder.Configuration["DOTNET_LAUNCH_PROFILE"];
// if (builder.Environment.IsDevelopment() && launchProfile == "https")
// {
// web.RunWithHttpsDevCertificate("HTTPS_CERT_FILE", "HTTPS_CERT_KEY_FILE");
// }

builder.Build().Run();