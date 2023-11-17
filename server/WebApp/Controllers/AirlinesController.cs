#pragma warning disable 1591
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using App.DAL.EF;
using App.Domain;
using Microsoft.AspNetCore.Authorization;

namespace WebApp.Controllers
{
    [Authorize(Roles = "admin")]
    public class AirlinesController : Controller
    {
        private readonly AppDbContext _context;

        public AirlinesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Airlines
        public async Task<IActionResult> Index()
        {
              return _context.Airlines != null ? 
                          View(await _context.Airlines.ToListAsync()) :
                          Problem("Entity set 'AppDbContext.Airlines'  is null.");
        }

        // GET: Airlines/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null || _context.Airlines == null)
            {
                return NotFound();
            }

            var airline = await _context.Airlines
                .FirstOrDefaultAsync(m => m.Id == id);
            if (airline == null)
            {
                return NotFound();
            }

            return View(airline);
        }

        // GET: Airlines/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Airlines/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Iata,Icao,Logo,Id")] Airline airline)
        {
            if (ModelState.IsValid)
            {
                airline.Id = Guid.NewGuid();
                _context.Add(airline);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(airline);
        }

        // GET: Airlines/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null || _context.Airlines == null)
            {
                return NotFound();
            }

            var airline = await _context.Airlines.FindAsync(id);
            if (airline == null)
            {
                return NotFound();
            }
            return View(airline);
        }

        // POST: Airlines/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Name,Iata,Icao,Logo,Id")] Airline airline)
        {
            if (id != airline.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(airline);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AirlineExists(airline.Id))
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
            return View(airline);
        }

        // GET: Airlines/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null || _context.Airlines == null)
            {
                return NotFound();
            }

            var airline = await _context.Airlines
                .FirstOrDefaultAsync(m => m.Id == id);
            if (airline == null)
            {
                return NotFound();
            }

            return View(airline);
        }

        // POST: Airlines/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            if (_context.Airlines == null)
            {
                return Problem("Entity set 'AppDbContext.Airlines'  is null.");
            }
            var airline = await _context.Airlines.FindAsync(id);
            if (airline != null)
            {
                _context.Airlines.Remove(airline);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AirlineExists(Guid id)
        {
          return (_context.Airlines?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
