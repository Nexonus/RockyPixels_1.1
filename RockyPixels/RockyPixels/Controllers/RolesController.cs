using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RockyPixels.Models;

namespace RockyPixels.Controllers
{
    public class RolesController : Controller
    {
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly ApplicationDbContext dbcontext;
        public RolesController(RoleManager<ApplicationRole> roleManager, ApplicationDbContext db)
        {
            _roleManager = roleManager;
            dbcontext = db;
        }

        public IActionResult Index()
        {
            var roles = _roleManager.Roles.ToList();
            return View(roles);
        }
        public IActionResult Create()
        {
            return View(new ApplicationRole());
        }

        [HttpPost]
        public async Task<IActionResult> Create(ApplicationRole role)
        {
            role.CreateDate = DateTime.Now;
            await _roleManager.CreateAsync(role);
            return RedirectToAction("Index");
        }
    }
}
