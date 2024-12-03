using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Graph;
using Microsoft.Identity.Web;
using RockyPixels.Models;

namespace RockyPixels.Controllers
{
    public class PostsController : Controller
    {
        private readonly GraphServiceClient _graphServiceClient;
        private readonly RockyPixelsBlogContext _context;

        public PostsController(RockyPixelsBlogContext context, GraphServiceClient graphServiceClient)
        {
            _context = context;
            _graphServiceClient = graphServiceClient; ;
        }

        // SORTED POSTS
        // GET: Posts

        public async Task<IActionResult> Index(string sortOrder)
        {
            ViewData["PostIdSort"] = String.IsNullOrEmpty(sortOrder) ? "PostId_Desc" : "";
            ViewData["CreationDateSort"] = sortOrder == "CreationDate" ? "CreationDate_Desc" : "CreationDate";
            ViewData["AuthorSort"] = sortOrder == "Author" ? "Author_Desc" : "Author";
            ViewData["TopicSort"] = sortOrder == "Topic" ? "Topic_Desc" : "Topic";
            ViewData["ModificationSort"] = sortOrder == "LastModified" ? "LastModified_Desc" : "LastModified";
            ViewData["CategorySort"] = sortOrder == "Category" ? "Category_Desc" : "Category";
            
            _context.Posts.Include(p => p.Category);

            //posts.Include(p => p.Category);

            var rockyPixelsBlogContext = _context.Posts.Include(p => p.Category);
            var posts = from s in rockyPixelsBlogContext
                        select s;

            switch (sortOrder)
            {
                case "PostId_Desc":
                    posts = posts.OrderByDescending(s => s.PostId);
                    break;
                case "CreationDate":
                    posts = posts.OrderBy(s => s.CreatedOn);
                    break;
                case "CreationDate_Desc":
                    posts = posts.OrderByDescending(s => s.CreatedOn);
                    break;
                case "Author":
                    posts = posts.OrderBy(s => s.Author);
                    break;
                case "Author_Desc":
                    posts = posts.OrderByDescending(s => s.Author);
                    break;
                case "Topic":
                    posts = posts.OrderBy(s => s.Topic);
                    break;
                case "Topic_Desc":
                    posts = posts.OrderByDescending(s => s.Topic);
                    break;
                case "LastModified":
                    posts = posts.OrderBy(s => s.LastModifiedOn);
                    break;
                case "LastModified_Desc":
                    posts = posts.OrderByDescending(s => s.LastModifiedOn);
                    break;
                case "Category":
                    posts = posts.OrderBy(s => s.Category);
                    break;
                case "Category_Desc":
                    posts = posts.OrderByDescending(s => s.Category);
                    break;
                default:
                    posts = posts.OrderBy(s => s.PostId);
                    break;
            }
            //return View(await posts.AsNoTracking().ToListAsync());
            return View(await posts.ToListAsync());
        }

        /*
        public async Task<IActionResult> Index()
        {
            var rockyPixelsBlogContext = _context.Posts.Include(p => p.Category);
            return View(await rockyPixelsBlogContext.ToListAsync());
        }
        */

        // GET: Posts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Posts
                .Include(p => p.Category)
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
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryName");
            return View();
        }

        // POST: Posts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeForScopes(ScopeKeySection = "MicrosoftGraph:Scopes")]
        public async Task<IActionResult> Create([Bind("PostId,Topic,PostContent,LastModifiedOn,Author,ImageForm,ImageData,CategoryId")] Models.Post post)
        {
           

            if (ModelState.IsValid)
            {
                /*
                 * UPLOAD IMAGE HERE!!!
                 */
                //--------------


                using (var memoryStream = new MemoryStream())
                {
                    if (post.ImageForm != null)
                    {
                        await post.ImageForm.CopyToAsync(memoryStream);

                        // Upload the file if less than 2 MB
                        if (memoryStream.Length < 2097152)
                        {
                            var file = new Models.Image()
                            {
                                Data = memoryStream.ToArray()
                            };

                            post.ImageData = file.Data;
                            //_context.Add(file);
                            //_context.Images.Add(file);
                            //await _context.SaveChangesAsync();
                        }
                        else
                        {
                            ModelState.AddModelError("File", "The file is too large.");
                        }
                    }
                }


                //--------------
                var user = await _graphServiceClient.Me.Request().GetAsync();

                post.Author = user.DisplayName;
                post.CreatedOn = DateTime.Now;
                _context.Add(post);
                await _context.SaveChangesAsync();

                int milliseconds = 50;
                System.Threading.Thread.Sleep(milliseconds);

                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryName", post.CategoryId);
            return View(post);
        }

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
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryName", post.CategoryId);
            return View(post);
        }

        // POST: Posts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PostId,Topic,PostContent,CreatedOn,LastModifiedOn,Author,ImageForm,ImageData,CategoryId")] Models.Post post)
        {
            if (id != post.PostId)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                try
                {
                    Console.Write(post.CreatedOn);
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
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryName", post.CategoryId);
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
                .Include(p => p.Category)
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
        [HttpPost, ActionName("DeleteAll")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteAll()
        {
            var toDelete = _context.Posts.Select(a => new Models.Post { PostId = a.PostId }).ToList();
            _context.Posts.RemoveRange(toDelete);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PostExists(int id)
        {
            return _context.Posts.Any(e => e.PostId == id);
        }
    }
}
