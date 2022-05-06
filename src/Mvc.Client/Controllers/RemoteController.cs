using System.Net.Http.Headers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Mvc.Client.Models;

namespace Mvc.Client.Controllers
{
    public class RemoteController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public RemoteController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IActionResult> Index()
        {
            var client = _httpClientFactory.CreateClient();
            var accessToken = await HttpContext.GetTokenAsync(OpenIdConnectParameterNames.AccessToken);
            if (string.IsNullOrEmpty(accessToken))
            {
                return View(new RemoteResultViewModel { Msg = "accesstoken 获取失败", Data = "" });
            }
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            var httpResponse = await client.GetAsync("https://localhost:5001/identity");
            var result = await httpResponse.Content.ReadAsStringAsync();
            if (!httpResponse.IsSuccessStatusCode)
            {
                return View(new RemoteResultViewModel { Msg = httpResponse.ReasonPhrase!, Data = result });
            }
            return View(new RemoteResultViewModel
            {
                Msg = "成功",
                Data = result
            });
        }
    }
}