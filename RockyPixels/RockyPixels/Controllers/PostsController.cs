using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Graph;
using Microsoft.Identity.Web;
using RockyPixels.Models;

namespace RockyPixels.Controllers
{
    public class PostsController : Controller
    {
        private readonly RockyPixelsBlogContext _context;
        private readonly ApplicationDbContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly GraphServiceClient _graphServiceClient;
        private readonly string _connectionString;
        public PostsController(RockyPixelsBlogContext context, IConfiguration configuration, ApplicationDbContext dbContext, IHttpContextAccessor httpContextAccessor, GraphServiceClient graphServiceClient)
        {
            _context = context;
            _dbContext = dbContext;
            _httpContextAccessor = httpContextAccessor;
            _connectionString = configuration.GetConnectionString("DefaultConnection");
            _graphServiceClient = graphServiceClient; ;
        }

        // GET: Posts
        public async Task<IActionResult> Index()
        {
            var rockyPixelsBlogContext = _context.Posts.Include(p => p.User);
            return View(await rockyPixelsBlogContext.ToListAsync());
            //var rockyPixelsBlogContext = _context.Posts.Include(p => p.User);
            //return View(await rockyPixelsBlogContext.ToListAsync());
        }

        // GET: Posts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Posts
                .Include(p => p.User)
                .FirstOrDefaultAsync(m => m.PostId == id);
            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        // GET: Posts/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Posts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeForScopes(ScopeKeySection = "MicrosoftGraph:Scopes")]
        public async Task<IActionResult> Create([Bind("Id,Topic,PostContent,CreatedOn,LastModifiedOn,UserId")] Models.Post post) // + IFormFile file
        {

            
            if (ModelState.IsValid)
            {
                //---------------------------------------------------------

                /// THIS IS USED TO USE IDENTITY ID'S. COMMENT TO SWITCH TO AZURE.:
                /*
                var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
                var userName = User.Identity.Name;
                
                post.UserId = userId;
                */

                /*
                if (User.Identity?.IsAuthenticated == true)
                {
                    var user = await _graphServiceClient.Me.Request().GetAsync();
                    ViewData["UserId"] = user.Id;
                    //post.UserId = user.Id;
                }
                */

                /// Nothing is happening here intentionally for now. Break time.


                //post.User.UserName = User.FindFirst(ClaimTypes.NameIdentifier).Value;
                post.CreatedOn = DateTime.Now;
                _context.Add(post);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            //ViewData["UserId"] = new SelectList(_context.AspNetUsers, "UserId", "UserId", post.UserId);
            return View(post);
        }

        //---------------------------------------------------------
        /*
        private byte[] ConvertFileToByteArray(IFormFile file)
        {
            using (var memoryStream = new MemoryStream())
            {
                file.CopyTo(memoryStream);
                return memoryStream.ToArray();
            }
        }
        */


        //---------------------------------------------------------

        // GET: Posts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Posts.FindAsync(id);
            if (post == null)
            {
                return NotFound();
            }
            //ViewData["UserId"] = new SelectList(_context.Users, "UserId", "UserId", post.UserId);
            return View(post);
        }

        // POST: Posts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PostId,Topic,PostContent,CreatedOn,LastModifiedOn,UserId")] Models.Post post)
        {
            if (id != post.PostId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {   
                try
                {
                    post.LastModifiedOn = DateTime.Now;
                    _context.Update(post);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PostExists(post.PostId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            //ViewData["UserId"] = new SelectList(_context.Users, "UserId", "UserId", post.UserId);
            return View(post);
        }

        // GET: Posts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Posts
                //.Include(p => p.User)
                .FirstOrDefaultAsync(m => m.PostId == id);
            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        // POST: Posts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var post = await _context.Posts.FindAsync(id);
            if (post != null)
            {
                _context.Posts.Remove(post);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PostExists(int id)
        {
            return _context.Posts.Any(e => e.PostId == id);
        }
    }
}
