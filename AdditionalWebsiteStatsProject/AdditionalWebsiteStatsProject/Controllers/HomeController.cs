using AdditionalWebsiteStatsProject.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Net;

namespace AdditionalWebsiteStatsProject.Controllers {
    public class HomeController : Controller {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger) {
            _logger = logger;
        }

        public async Task<IActionResult> Index() {
            bool result = await SendInfo();
            ViewData["Result"] = result;
            return View();
        }

        public IActionResult Privacy() {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        public async Task<bool> SendInfo() {
            try
            {
                string ip = await GetExternalIpAddress();
                HttpClient client = new HttpClient();
                string referrer = Request.Headers["Referer"].ToString();
                int idSource = !string.IsNullOrEmpty(referrer) ? 1 : 2;
                string agent = Request.Headers["User-Agent"].ToString();
                int idSystem = 3;
                string browser;
                if (agent.Contains("Chrome"))
                {
                    browser = "Google Chrome";
                }
                else if (agent.Contains("Firefox"))
                {
                    browser = "Firefox";
                }
                else if (agent.Contains("Edg"))
                {
                    browser = "Microsoft Edge";
                }
                else if (agent.Contains("Safari"))
                {
                    browser = "Safari";
                }
                else
                {
                    browser = "Інший браузер";
                }
                if (agent.Contains("Windows"))
                {
                    idSystem = 2;
                }
                else if (agent.Contains("Linux"))
                {
                    idSystem = 4;
                }
                else if (agent.Contains("Macintosh"))
                {
                    idSystem = 1;
                }
                await client.GetAsync($"https://localhost:7128/SendIP/{ip}/{idSource}/{browser}/{idSystem}");
                return true;
            }
            catch
            {
                return false;
            }
        }
        public static async Task<string> GetExternalIpAddress() {
            var externalIpString = (await new HttpClient().GetStringAsync("http://icanhazip.com"))
                .Replace("\\r\\n", "").Replace("\\n", "").Trim();
            if (!IPAddress.TryParse(externalIpString, out var ipAddress)) return null;
            return ipAddress.ToString();
        }
    }
}
