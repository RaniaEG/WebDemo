using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebDemo.Data;
using WebDemo.Models;

namespace WebDemo.Controllers
{
    public class MessagesController : Controller
    {
        private readonly AppDbContext _db;

        public MessagesController(AppDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var messages = await _db.Messages
                .OrderByDescending(m => m.Id)
                .ToListAsync();

            return View(messages);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new Message());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Message input)
        {
            if (!ModelState.IsValid)
            {
                return View(input);
            }

            _db.Messages.Add(input);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
