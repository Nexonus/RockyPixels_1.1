using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RockyPixels.Models;
using System.Diagnostics;
using Microsoft.Graph;
using Microsoft.Identity.Web;
using System.Data;

namespace RockyPixels.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly GraphServiceClient _graphServiceClient;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, GraphServiceClient graphServiceClient)
        {
            _logger = logger;
            _graphServiceClient = graphServiceClient;;
        }
        [Authorize(Roles = "blog_admin")]
        [AuthorizeForScopes(ScopeKeySection = "MicrosoftGraph:Scopes")]
        public async Task<IActionResult> Index()
        {
            
            var user = await _graphServiceClient.Me.Request().GetAsync();
            
            ViewData["GraphApiResult"] = user.DisplayName;
                        return View();
            return View();
        }


        public IActionResult Privacy()
        {
            return View();
        }

        [AllowAnonymous]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
