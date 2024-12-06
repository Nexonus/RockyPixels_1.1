using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.Elfie.Diagnostics;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Microsoft.Extensions.Hosting;
using Microsoft.Graph;
using Microsoft.Identity.Web;
using RockyPixels.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Microsoft.AspNetCore.Session;

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
        [Authorize(Roles = "blog_admin")]
        public async Task<IActionResult> Index(string sortOrder)
        {
            ViewData["PostIdSort"] = System.String.IsNullOrEmpty(sortOrder) ? "PostId_Desc" : "";
            ViewData["CreationDateSort"] = sortOrder == "CreationDate" ? "CreationDate_Desc" : "CreationDate";
            ViewData["AuthorSort"] = sortOrder == "Author" ? "Author_Desc" : "Author";
            ViewData["TopicSort"] = sortOrder == "Topic" ? "Topic_Desc" : "Topic";
            ViewData["ModificationSort"] = sortOrder == "LastModified" ? "LastModified_Desc" : "LastModified";
            ViewData["CategorySort"] = sortOrder == "Category" ? "Category_Desc" : "Category";

            //_context.Posts.Include(p => p.Category);
            //_context.Posts.Include(p => p.Image);

            //posts.Include(p => p.Category);

            var rockyPixelsBlogContext = _context.Posts
                .Include(p => p.Category)
                .Include(p => p.Image)
                .Include(p => p.PostTags)
                .ThenInclude(pt => pt.Tag);
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
        [Authorize]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var user = await _graphServiceClient.Me.Request().GetAsync();
            ViewBag.CurrentUser = user.DisplayName;

            var post = await _context.Posts
                .Include(p => p.Category)
                .Include(p => p.PostTags)
                .ThenInclude(pt => pt.Tag)
                .Include(p => p.Image)
                .Include(p => p.Comments)
                .FirstOrDefaultAsync(m => m.PostId == id);
            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        // ADD COMMENTS : POST

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddComment(int postId, string content)
        {
            if (ModelState.IsValid)
            {
                var user = await _graphServiceClient.Me.Request().GetAsync();


                // Create a new Comment object
                var comment = new Comment
                {
                    ParentPostId = postId,
                    Author = user.DisplayName,
                    CommentContent = content,
                    CreatedOn = DateTime.Now
                };

                // Add the comment to the database
                _context.Comments.Add(comment);
                await _context.SaveChangesAsync();

                // Redirect to the post details page (where the comments are displayed)
                //return RedirectToAction(nameof(Details), new { id = postId });
                return RedirectToAction("Index", "Home", new { id = postId });
            }
            //return RedirectToAction(nameof(Details), new { id = postId });
            return RedirectToAction("Index", "Home", new { id = postId });
        }

        // DELETE COMMENT : POST

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteComment(int commentId)
        {
            // Retrieve the current user from Microsoft Graph
            var user = await _graphServiceClient.Me.Request().GetAsync();
            if (user == null)
            {
                return RedirectToAction("Error", "Home");
            }
            Console.Write(commentId);
            var comment = await _context.Comments.FirstOrDefaultAsync(c => c.CommentId == commentId);

            if (comment == null)
            {
                return RedirectToAction("Error", "Home");
            }
            if (comment.Author == user.DisplayName || User.IsInRole("blog_admin"))
            {
                // Remove the comment from the database
                _context.Comments.Remove(comment);
                await _context.SaveChangesAsync();
            }
            //return RedirectToAction("Details", "Posts", new { id = comment.ParentPostId });
            return RedirectToAction("Index", "Home", new { id = comment.ParentPostId });
        }

        // GET: Posts/Create
        [Authorize(Roles = "blog_admin")]
        public IActionResult Create()
        {
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryName");
            ViewData["ImageId"] = new SelectList(_context.Images, "ImageId", "ImageData");
            
            //WIP -------------------
            ViewData["TagId"] = new SelectList(_context.Tags, "TagId", "TagName");
            ViewData["PostId"] = new SelectList(_context.PostTags, "PostId", "TagId"); // PostTags

            return View();
        }
        private async Task<byte[]> ReadBytesFromImageForm(Models.Post post, int MB_Limit)
        {
            using (var memoryStream = new MemoryStream())
            {
                if (post.ImageForm != null)
                {
                    await post.ImageForm.CopyToAsync(memoryStream);
                    if (memoryStream.Length < MB_Limit * 1000000)
                    {
                        var ImageData = memoryStream.ToArray();
                        return ImageData;
                    }
                }
            }
            return null;
        }
        [Authorize(Roles = "blog_admin")]
        private async void UpdatePostImage(Models.Post post, Models.Image image, int MB_Limit)
        {
            if (post.ImageForm != null && post.ImageForm.Length > 0)
            {
                try
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await post.ImageForm.CopyToAsync(memoryStream);

                        // Upload the file if less than N MB
                        if (memoryStream.Length < MB_Limit * 1000000)
                        {
                            var file = new Models.Image()
                            {
                                ImageData = memoryStream.ToArray()
                            };
                            post.ImageData = file.ImageData;
                            image.ImageData = file.ImageData;
                            _context.Images.Update(image);
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Log or handle error
                    ModelState.AddModelError("", $"An error occurred while processing the image: {ex.Message}");
                }
            }
        }
        [Authorize(Roles = "blog_admin")]
        private async void UploadImageToPost(Models.Post post, int MB_Limit)
        {
            using (var memoryStream = new MemoryStream())
            {
                if (post.ImageForm != null)
                {
                    await post.ImageForm.CopyToAsync(memoryStream);

                    // Upload the file if less than N MB
                    if (memoryStream.Length < MB_Limit * 1000000)
                    {
                        var file = new Models.Image()
                        {
                            ImageData = memoryStream.ToArray()
                        };
                        post.ImageData = file.ImageData;
                        _context.Images.Add(file);  // Add Image to Images Model
                        await _context.SaveChangesAsync();
                        var item = _context.Images
                               .OrderByDescending(p => p.ImageId)
                               .FirstOrDefault();
                        if (item != null)   // If item image exists
                        {
                            post.Image = item;
                        }

                    }
                    else
                    {
                        ModelState.AddModelError("File", "The file is too large.");
                    }
                }
                else
                {
                    await _context.Database.ExecuteSqlAsync($"INSERT INTO RockyPixels.Images (ImageData) VALUES (NULL)");
                    var newImage = _context.Images
                        .FromSql($"SELECT TOP 1 * FROM RockyPixels.Images ORDER BY ImageId DESC")
                        .FirstOrDefault();
                    if (newImage != null){
                        var newImageId = newImage.ImageId;
                        post.ImageId = newImageId;
                    }

                }
                int milliseconds = 50;
                System.Threading.Thread.Sleep(milliseconds);
            }
        }


        // POST: Posts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeForScopes(ScopeKeySection = "MicrosoftGraph:Scopes")]
        [Authorize(Roles = "blog_admin")]
        public async Task<IActionResult> Create([Bind("PostId,Topic,PostContent,LastModifiedOn,Author,ImageForm,ImageData,CategoryId,ImageId")] Models.Post post, int[] selectedTags)
        {
            if (ModelState.IsValid)
            {
                // Add Tags WIP
                
                if (selectedTags != null)
                {
                    foreach (var tagId in selectedTags)
                    {
                        var tag = _context.Tags.Find(tagId);
                        if (tag != null)
                        {
                            // Add the tag to the post's Tags collection
                            var postTag = new Models.PostTag
                            {
                                PostId = post.PostId,
                                TagId = tag.TagId
                            };
                            post.PostTags.Add(postTag);
                            //post.Tag.Add(tag);
                        }
                    }
                    await _context.SaveChangesAsync();
                }

                UploadImageToPost(post, 2);
                //--------------
                var user = await _graphServiceClient.Me.Request().GetAsync();

                post.Author = user.DisplayName;
                post.CreatedOn = DateTime.Now;
                _context.Add(post);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryName", post.CategoryId);
            ViewData["ImageId"] = new SelectList(_context.Images, "ImageId", "ImageData", post.ImageId);
            // WIP ------------------------
            ViewData["TagId"] = new SelectList(_context.Tags, "TagId", "TagName", selectedTags);
            return View(post);
        }

        // GET: Posts/Edit/5
        [Authorize(Roles = "blog_admin")]
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
            ViewData["ImageId"] = new SelectList(_context.Images, "ImageId", "ImageData", post.ImageId);
            ViewData["TagId"] = new SelectList(_context.Tags, "TagId", "TagName");
            ViewData["PostId"] = new SelectList(_context.PostTags, "PostId", "TagId"); // PostTags
            return View(post);
        }

        // POST: Posts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "blog_admin")]
        public async Task<IActionResult> Edit(int id, [Bind("PostId,Topic,PostContent,CreatedOn,LastModifiedOn,Author,ImageForm,ImageData,CategoryId,ImageId")] Models.Post post, [Bind("ImageId,ImageData")] Models.Image image, int[] selectedTags)
        {
            if (id != post.PostId)
            {
                return NotFound();
            }

            if (post.ImageId is null)
            {
                await _context.Database.ExecuteSqlAsync($"INSERT INTO RockyPixels.Images (ImageData) VALUES (NULL)");
                var updateImage = _context.Images
                    .FromSql($"SELECT TOP 1 * FROM RockyPixels.Images ORDER BY ImageId DESC")
                    .FirstOrDefault();
                int updateImageId = 0;
                var sourcePost = _context.Posts
                    .FromSql($"SELECT * FROM RockyPixels.Posts WHERE PostId LIKE {id}")
                    .FirstOrDefault();
                if (updateImage != null)
                {
                    updateImageId = updateImage.ImageId;    // This is done when the ImageID in the Post has been nullified. We artificially fix it here.
                    await _context.Database.ExecuteSqlAsync($"UPDATE RockyPixels.Posts SET ImageId = {updateImageId} WHERE PostId = {id}");
                }
                var newImageData = ReadBytesFromImageForm(post, 2).Result;

                //byte[] newImage = newImageData ?? new byte[0];

                //object dbValue = (newImageData == null) ? DBNull.Value : newImageData;

                // THIS PART THROWS AN ERROR WHEN USER MAKES A NEW POST WITH NO IMAGE! newData = Null and is passed into Varbinary(MAX)

                try
                {
                    //_context.Update(post);
                    //post.ImageData = newData;
                    //image.ImageData = newData;

                    if (newImageData != null)
                    {
                        await _context.Database.ExecuteSqlRawAsync("UPDATE RockyPixels.Posts SET ImageData = {0} WHERE PostId = {1}", newImageData, id);
                        await _context.Database.ExecuteSqlRawAsync("UPDATE RockyPixels.Images SET ImageData = {0} WHERE ImageId = {1}", newImageData, updateImageId);

                        await _context.SaveChangesAsync();
                    }
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
            else
            {
                if (ModelState.IsValid)
                {
                    try
                    {
                        var toDelete = await _context.PostTags.Where(pt => pt.PostId == id).ToListAsync();
                        _context.PostTags.RemoveRange(toDelete);
                        await _context.SaveChangesAsync();
                        

                        if (selectedTags != null)
                        {
                            foreach (var tagId in selectedTags)
                            {
                                var tag = _context.Tags.Find(tagId);
                                if (tag != null)
                                {
                                    // Add the tag to the post's Tags collection
                                    var postTag = new Models.PostTag
                                    {
                                        PostId = post.PostId,
                                        TagId = tag.TagId,
                                    };
                                    //post.PostTags.Add(postTag);
                                    //post.Tag.Add(tag);
                                    await _context.Database.ExecuteSqlAsync($"INSERT INTO RockyPixels.PostTags (PostId, TagId) VALUES ({id}, {tagId})");
                                }
                            }
                            await _context.SaveChangesAsync();
                        }
                        if (post.ImageForm != null)
                        {
                            UpdatePostImage(post, image, 2);
                        }
                        else
                        {
                            var dbImageData = _context.Images
                                .FromSql($"SELECT * FROM RockyPixels.Images WHERE ImageId LIKE {post.ImageId}")
                                .FirstOrDefault();
                            if (dbImageData != null) {
                                post.ImageData = dbImageData.ImageData;
                            }
                        }
                        Console.Write(post);

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
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryName", post.CategoryId);
            ViewData["ImageId"] = new SelectList(_context.Images, "ImageId", "ImageData", post.ImageId);
            ViewData["TagId"] = new SelectList(_context.Tags, "TagId", "TagName", selectedTags);
            return View(post);
        }

        // GET: Posts/Delete/5
        [Authorize(Roles = "blog_admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Posts
                .Include(p => p.Category)
                .Include(p => p.PostTags)
                .ThenInclude(pt => pt.Tag)
                .Include(p => p.Image)
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
        [Authorize(Roles = "blog_admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {

            var post = await _context.Posts.FindAsync(id);
            var image_id = post.ImageId;
            var child_image = _context.Images.Find(image_id);

            if (post != null)
            {
                if (child_image != null)
                {
                    _context.Images.Remove(child_image);    // This ensures that DELETE CASCADE works both ways. 
                }
                _context.Database.ExecuteSql($"DELETE FROM RockyPixels.PostTags WHERE (PostId = {id})");
                _context.Posts.Remove(post);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        [HttpPost, ActionName("DeleteAll")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "blog_admin")]
        public async Task<IActionResult> DeleteAll(int id, [Bind("PostId,Topic,PostContent,CreatedOn,LastModifiedOn,Author,ImageForm,ImageData,CategoryId,ImageId")] Models.Post post)
        {
            _context.Database.ExecuteSql($"DELETE FROM RockyPixels.PostTags");
            var toDelete = _context.Posts.Select(a => new Models.Post { PostId = a.PostId }).ToList();
            _context.Posts.RemoveRange(toDelete);
            await _context.SaveChangesAsync();

            _context.Database.ExecuteSql($"DBCC CHECKIDENT ('RockyPixels.Posts', RESEED, 0)");
            _context.Database.ExecuteSql($"DELETE FROM RockyPixels.Images");
            _context.Database.ExecuteSql($"DBCC CHECKIDENT ('RockyPixels.Images', RESEED, 0)");

            return RedirectToAction(nameof(Index));
        }

        private bool PostExists(int id)
        {
            return _context.Posts.Any(e => e.PostId == id);
        }
        [Authorize]
        public IActionResult PostIndexPartial()
        {
            return PartialView("_PostIndexPartial", _context);
        }
    }
}
