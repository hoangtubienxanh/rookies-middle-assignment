using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var db = builder.AddSqlite("scribe", databaseFileName: "scribe_dev");
var api = builder.AddProject<Api>("api")
    .WithReference(db)
    .WaitFor(db);

var web = builder
    .AddViteApp("web", packageManager: "pnpm", workingDirectory: Path.Combine(new web_ui().ProjectPath, "../"))
    .WithReference(api)
    .WithPnpmPackageInstallation();

web.PublishAsDockerFile();

// https://github.com/dotnet/aspire-samples/blob/8f25954f2ec7aee9c08ad42b7baf293d6cea7540/samples/AspireWithNode/AspireWithNode.AppHost/Program.cs
// var launchProfile = builder.Configuration["DOTNET_LAUNCH_PROFILE"];
// if (builder.Environment.IsDevelopment() && launchProfile == "https")
// {
// web.RunWithHttpsDevCertificate("HTTPS_CERT_FILE", "HTTPS_CERT_KEY_FILE");
// }

builder.Build().Run();