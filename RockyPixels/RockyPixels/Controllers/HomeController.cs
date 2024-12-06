using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RockyPixels.Models;
using System.Diagnostics;
using Microsoft.Graph;
using Microsoft.Identity.Web;
using System.Data;
using Microsoft.EntityFrameworkCore;

namespace RockyPixels.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly GraphServiceClient _graphServiceClient;
        private readonly ILogger<HomeController> _logger;
        private readonly RockyPixelsBlogContext _context;

        public HomeController(ILogger<HomeController> logger, GraphServiceClient graphServiceClient, RockyPixelsBlogContext context)
        {
            _logger = logger;
            _context = context;
            _graphServiceClient = graphServiceClient;;
        }
        [AuthorizeForScopes(ScopeKeySection = "MicrosoftGraph:Scopes")]
        public async Task<IActionResult> Index()
        {
            var user = await _graphServiceClient.Me.Request().GetAsync();

            ViewData["GraphApiResult"] = user.DisplayName;

            var posts = await _context.Posts
                .Include(p => p.Category)
                .Include(p => p.Image)
                .Include(p => p.PostTags)
                .ThenInclude(pt => pt.Tag)
                .Include(p => p.Comments)
                .ToListAsync();
            return View(posts);
            /*
            var user = await _graphServiceClient.Me.Request().GetAsync();
            
            ViewData["GraphApiResult"] = user.DisplayName;
                        return View();
            return View();*/
        }


        public IActionResult Privacy()
        {
            return View();
        }

        public async Task<IActionResult> SecondTest()
        {
            var posts = await _context.Posts
                .Include(p => p.Category)
                .Include(p => p.Image)
                .Include(p => p.PostTags)
                .ThenInclude(pt => pt.Tag)
                .Include(p => p.Comments)
                .ToListAsync();
            return View(posts);
        }

        [AllowAnonymous]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
