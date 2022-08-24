using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using permafnotes;
using PermafnotesDomain.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

builder.Services.AddMsalAuthentication(options =>
{
    builder.Configuration.Bind("AzureAd", options.ProviderOptions.Authentication);
    options.ProviderOptions.DefaultAccessTokenScopes.Add("openid");
    options.ProviderOptions.DefaultAccessTokenScopes.Add("offline_access");
});

builder.Services.AddGraphClient("https://graph.microsoft.com/User.Read", "https://graph.microsoft.com/Files.ReadWrite");

builder.Logging.SetMinimumLevel(LogLevel.Trace);

builder.Services.AddAntDesign();

builder.Services.AddScoped<NoteService>();

await builder.Build().RunAsync();
