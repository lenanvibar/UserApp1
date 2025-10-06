using System.Net;
using System.Net.Sockets;
using System.Reflection.Metadata.Ecma335;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing.Constraints;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using UsersApp.Data;
using UsersApp.Models;



namespace UsersApp.Controllers
{
    public class TicketsController(AppDbContext context, IWebHostEnvironment environment) : Controller
    {
        private readonly AppDbContext _context = context;
        private readonly IWebHostEnvironment _environment = environment;
      

        public IActionResult Create()
        {
            return View();

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Ticket ticket, IFormFile file)
        {
            if (ModelState.IsValid)
            {
                string uploads = Path.Combine(_environment.WebRootPath, "uploads");

                if (!Directory.Exists(uploads))
                {
                    Directory.CreateDirectory(uploads);
                }
                if (file != null && file.Length > 0)
                {
                    var originalFileName = Path.GetFileNameWithoutExtension(file.FileName);
                    var extension = Path.GetExtension(file.FileName);
                    var safeFileName = originalFileName.Replace(" ", "_") + "_" + Guid.NewGuid() + extension;

                    // Create file path safely
                    var filePath = Path.Combine(uploads, safeFileName);

                    // Save file
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }



                    ticket.FilePath = "/uploads/" + safeFileName;



                }

                _context.Tickets.Add(ticket);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index", "Tickets");
            }
            return View(ticket);

        }

        public IActionResult Index()
        {
            var tickets = _context.Tickets.ToList();
            return View(tickets);
        }
    }
}
