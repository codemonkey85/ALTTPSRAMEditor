using ALTTPSRAMEditor.BlazorWasm;
using Library.Classes;
using Library.Services;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddSingleton<GameService>();
builder.Services.AddSingleton<TextCharacterData>();
builder.Services.AddMudServices();

await builder.Build().RunAsync();
