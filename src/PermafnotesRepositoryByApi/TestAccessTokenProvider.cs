using Microsoft.Identity.Client;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using System.Threading.Tasks;

public class TestAccessTokenProvider : IAccessTokenProvider
{
    private readonly IConfidentialClientApplication _app;
    private readonly string[] _scopes;

    public TestAccessTokenProvider(string instance, string tenantId, string clientId, string clientSecret, string apiScope)
    {
        _app = ConfidentialClientApplicationBuilder.Create(clientId)
            .WithClientSecret(clientSecret)
            .WithAuthority(new Uri($"{instance}{tenantId}"))
            .Build();
        _scopes = new[] { apiScope };
    }

    public async ValueTask<AccessTokenResult> RequestAccessToken()
    {
        try
        {
            var result = await _app.AcquireTokenForClient(_scopes).ExecuteAsync();
            var accessToken = new AccessToken
            {
                Value = result.AccessToken,
                Expires = result.ExpiresOn
            };
            return new AccessTokenResult(AccessTokenResultStatus.Success, accessToken, null, null);
        }
        catch (Exception ex)
        {
            return new AccessTokenResult(AccessTokenResultStatus.RequiresRedirect, null, ex.Message, null);
        }
    }

    public ValueTask<AccessTokenResult> RequestAccessToken(AccessTokenRequestOptions options)
    {
        throw new NotImplementedException();
    }
}
