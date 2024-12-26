using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

//builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("http://172.19.0.3:8081/") });
//builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("https://localhost:44335/") }); // TO CHANGE
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("http://88.198.115.159/") }); // TO CHANGE

await builder.Build().RunAsync();