using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Graph;
using RockyPixels.Models;

namespace RockyPixels.Controllers
{
    public class ImagesController : Controller
    {
        private readonly RockyPixelsBlogContext _context;

        public ImagesController(RockyPixelsBlogContext context)
        {
            _context = context;
        }

        /// UPLOAD IMAGE TEST
        /*
        public async Task<IActionResult> UploadImage([Bind("ImageId,ImageForm")] Image buffered_file)
        {
            using (var memoryStream = new MemoryStream())
            {
                if (buffered_file.ImageForm != null)
                {
                    await buffered_file.ImageForm.CopyToAsync(memoryStream);

                    // Upload the file if less than 2 MB
                    if (memoryStream.Length < 2097152)
                    {
                        var file = new Image()
                        {
                            Data = memoryStream.ToArray()
                        };

                        _context.Images.Add(file);
                        int milliseconds = 50;
                        System.Threading.Thread.Sleep(milliseconds);

                        await _context.SaveChangesAsync();
                    }
                    else
                    {
                        ModelState.AddModelError("File", "The file is too large.");
                    }
                }
            }

            return View();
        }
        */

        // GET: Images
        public async Task<IActionResult> Index()
        {
            return View(await _context.Images.ToListAsync());
        }

        // GET: Images/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var image = await _context.Images
                .FirstOrDefaultAsync(m => m.ImageId == id);
            if (image == null)
            {
                return NotFound();
            }

            return View(image);
        }

        // GET: Images/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Images/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ImageId,Data,Metadata,ImageForm")] Models.Image image)
        {
            if (ModelState.IsValid)
            {
                /*
                 * 
                 * Upload image here
                 * 
                */
                //------------------------

                using (var memoryStream = new MemoryStream())
                {
                    if (image.ImageForm != null)
                    {
                        await image.ImageForm.CopyToAsync(memoryStream);

                        // Upload the file if less than 2 MB
                        if (memoryStream.Length < 2097152)
                        {
                            var file = new Models.Image()
                            {
                                Data = memoryStream.ToArray()
                            };

                            file.Metadata = image.Metadata;
                            _context.Add(file);
                            //_context.Images.Add(file);
                            int milliseconds = 50;
                            System.Threading.Thread.Sleep(milliseconds);

                            await _context.SaveChangesAsync();

                            int id = _context.Images.Max(x => x.ImageId);
                            TempData["parentImageId"] = id;
                        }
                        else
                        {
                            ModelState.AddModelError("File", "The file is too large.");
                        }
                    }
                }
                //------------------------
                //_context.Add(image);
                //await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(image);
        }

        // GET: Images/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var image = await _context.Images.FindAsync(id);
            if (image == null)
            {
                return NotFound();
            }
            return View(image);
        }

        // POST: Images/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ImageId,Data,Metadata")] Models.Image image)
        {
            if (id != image.ImageId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(image);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ImageExists(image.ImageId))
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
            return View(image);
        }

        // GET: Images/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var image = await _context.Images
                .FirstOrDefaultAsync(m => m.ImageId == id);
            if (image == null)
            {
                return NotFound();
            }

            return View(image);
        }

        // POST: Images/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var image = await _context.Images.FindAsync(id);
            if (image != null)
            {
                _context.Images.Remove(image);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ImageExists(int id)
        {
            return _context.Images.Any(e => e.ImageId == id);
        }
    }
}
