// See https://aka.ms/new-console-template for more information
using System;
using IdentityModel.Client;
using Newtonsoft.Json.Linq;

Console.WriteLine("discover endpoints from metadata 从元数据发现端点");
// discover endpoints from metadata
var client = new HttpClient();
var disco = await client.GetDiscoveryDocumentAsync("https://localhost:5000");
if (disco.IsError)
{
    Console.WriteLine(disco.Error);
    return;
}

Console.WriteLine("request token 请求token");
// request token
var tokenResponse = await client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
{
    Address = disco.TokenEndpoint,
    ClientId = "client",
    ClientSecret = "secret",
    Scope = "api1"
});

if (tokenResponse.IsError)
{
    Console.WriteLine(tokenResponse.Error);
    return;
}

Console.WriteLine(tokenResponse.Json);

Console.WriteLine("call api 请求资源服务api");
// call api
var apiClient = new HttpClient();
apiClient.SetBearerToken(tokenResponse.AccessToken);

var response = await apiClient.GetAsync("https://localhost:5001/identity");
if (!response.IsSuccessStatusCode)
{
    Console.WriteLine(response.StatusCode);
}
else
{
    var content = await response.Content.ReadAsStringAsync();
    Console.WriteLine(JArray.Parse(content));
}
Console.ReadLine();