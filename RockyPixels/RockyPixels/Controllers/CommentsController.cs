using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RockyPixels.Models;

namespace RockyPixels.Controllers
{
    public class CommentsController : Controller
    {
        private readonly RockyPixelsBlogContext _context;

        public CommentsController(RockyPixelsBlogContext context)
        {
            _context = context;
        }

        // GET: Comments
        [Authorize(Roles = "blog_admin")]
        public async Task<IActionResult> Index()
        {
            var rockyPixelsBlogContext = _context.Comments.Include(c => c.ParentPost);
            return View(await rockyPixelsBlogContext.ToListAsync());
        }

        // GET: Comments/Details/5
        [Authorize]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var comment = await _context.Comments
                .Include(c => c.ParentPost)
                .FirstOrDefaultAsync(m => m.CommentId == id);
            if (comment == null)
            {
                return NotFound();
            }

            return View(comment);
        }

        // GET: Comments/Create
        [Authorize(Roles = "blog_admin,blog_user")]
        public IActionResult Create()
        {
            ViewData["ParentPostId"] = new SelectList(_context.Posts, "PostId", "PostId");
            return View();
        }

        // POST: Comments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "blog_admin,blog_user")]
        public async Task<IActionResult> Create([Bind("CommentId,CommentContent,Author,CreatedOn,ParentPostId")] Comment comment)
        {
            if (ModelState.IsValid)
            {
                _context.Add(comment);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ParentPostId"] = new SelectList(_context.Posts, "PostId", "PostId", comment.ParentPostId);
            return View(comment);
        }

        // GET: Comments/Edit/5
        [Authorize(Roles = "blog_admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var comment = await _context.Comments.FindAsync(id);
            if (comment == null)
            {
                return NotFound();
            }
            ViewData["ParentPostId"] = new SelectList(_context.Posts, "PostId", "PostId", comment.ParentPostId);
            return View(comment);
        }

        // POST: Comments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "blog_admin")]
        public async Task<IActionResult> Edit(int id, [Bind("CommentId,CommentContent,Author,CreatedOn,ParentPostId")] Comment comment)
        {
            if (id != comment.CommentId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(comment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CommentExists(comment.CommentId))
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
            ViewData["ParentPostId"] = new SelectList(_context.Posts, "PostId", "PostId", comment.ParentPostId);
            return View(comment);
        }

        // GET: Comments/Delete/5
        [Authorize(Roles = "blog_admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var comment = await _context.Comments
                .Include(c => c.ParentPost)
                .FirstOrDefaultAsync(m => m.CommentId == id);
            if (comment == null)
            {
                return NotFound();
            }

            return View(comment);
        }

        // POST: Comments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "blog_admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var comment = await _context.Comments.FindAsync(id);
            if (comment != null)
            {
                _context.Comments.Remove(comment);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CommentExists(int id)
        {
            return _context.Comments.Any(e => e.CommentId == id);
        }
    }
}
