var builder = WebAssemblyHostBuilder.CreateDefault(args);
var services = builder.Services;

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

services
    .AddSingleton<GameService>()
    .AddSingleton<TextCharacterData>()
    .AddMudServices();

await builder.Build().RunAsync();
