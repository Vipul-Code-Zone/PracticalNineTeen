using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PracticalNineteen.Models.ViewModels;
using PracticalNineTeen.Models;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Text.Json.Serialization;

namespace PracticalNineTeen.Controllers
{
    public class HomeController : Controller
    {
        private readonly HttpClient _httpClient;

        public HomeController(IHttpClientFactory httpClient)
        {
            _httpClient = httpClient.CreateClient("studentApi");
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> GetAllUser()
        {
            if (Request.Cookies["Email"] == null)
            {
                return RedirectToAction("Login", "Account");   
            }
            var response = await _httpClient.GetAsync("User/Users");
            if (response.StatusCode == HttpStatusCode.OK)
            {
                var data = await response.Content.ReadAsStringAsync();
                List<RegisteredUser> users = JsonConvert.DeserializeObject<List<RegisteredUser>>(data);
                return View(users);
            }
            return View(new List<RegisteredUser>());
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}