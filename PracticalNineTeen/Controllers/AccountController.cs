using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PracticalNineteen.Models.ViewModels;
using System.Net;
using System.Text.Json.Serialization;
using static System.Net.Mime.MediaTypeNames;

namespace PracticalNineTeen.Controllers
{
    public class AccountController : Controller
    {
        private readonly HttpClient _httpClient;

        public AccountController(IHttpClientFactory httpClient)
        {
            _httpClient = httpClient.CreateClient("studentApi");
        }

        [HttpGet]
        public async Task<IActionResult> RegisterAsync()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _httpClient.PostAsJsonAsync("User/Register", model);
                var msg = result.Content.ReadAsStringAsync();
                if (result.StatusCode == HttpStatusCode.OK)
                {
                    return RedirectToAction("Login");

                }
                ModelState.AddModelError("", msg.Result);
                return View(model);
            }
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            var result = await _httpClient.PostAsJsonAsync("User/Login", model);

            if (result.StatusCode == HttpStatusCode.OK)
            {
                var content = await result.Content.ReadAsStringAsync();
                UserManagerRespose userManager = JsonConvert.DeserializeObject<UserManagerRespose>(content);

                Response.Cookies.Append("userToken", userManager.Message);
                Response.Cookies.Append("Email", userManager.Email);
                return RedirectToAction("Index", "Home");

            }
            ModelState.AddModelError("", "Data can't added Try again!");
            return View(model);
        }
        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            Logout logout = new Logout();
            string data = Request.Cookies["Email"].ToString();
            Response.Cookies.Delete("Email");
            Response.Cookies.Delete("userToken");

            logout.Email = data;
            var result = await _httpClient.PostAsJsonAsync("User/Logout",logout);
            if(result.StatusCode == HttpStatusCode.OK)
            {
                return RedirectToAction("Index", "Home");
            }
            return RedirectToAction("Index", "Home");
        }
    }
}
