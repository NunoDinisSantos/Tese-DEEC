using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

//builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("http://teseapi:8081/") });
//builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("https://localhost:44335/") }); // TO CHANGE
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("https://misteriosaquaticos.pt/") }); // TO CHANGE

await builder.Build().RunAsync();