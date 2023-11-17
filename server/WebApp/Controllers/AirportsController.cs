#pragma warning disable 1591
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using App.DAL.EF;
using App.Domain;
using Microsoft.AspNetCore.Authorization;

namespace WebApp.Controllers
{
    [Authorize(Roles = "admin")]
    public class AirportsController : Controller
    {
        private readonly AppDbContext _context;

        public AirportsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Airports
        public async Task<IActionResult> Index()
        {
            var appDbContext = _context.Airports.Include(a => a.Country).Where(a => a.DisplayAirport);
            return View(await appDbContext.ToListAsync());
        }

        // GET: Airports/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null || _context.Airports == null)
            {
                return NotFound();
            }

            var airport = await _context.Airports
                .Include(a => a.Country)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (airport == null)
            {
                return NotFound();
            }

            return View(airport);
        }

        // GET: Airports/Create
        public IActionResult Create()
        {
            ViewData["CountryId"] = new SelectList(_context.Countries, "Id", "Iso2");
            return View();
        }

        // POST: Airports/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Iata,Name,DeparturesLastCheckedUtc,ArrivalsLastCheckedUtc,Longitude,Latitude,DisplayGate,DisplayTerminal,DisplayAirport,CountryId,Id")] Airport airport)
        {
            if (ModelState.IsValid)
            {
                airport.Id = Guid.NewGuid();
                _context.Add(airport);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CountryId"] = new SelectList(_context.Countries, "Id", "Iso2", airport.CountryId);
            return View(airport);
        }

        // GET: Airports/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null || _context.Airports == null)
            {
                return NotFound();
            }

            var airport = await _context.Airports.FindAsync(id);
            if (airport == null)
            {
                return NotFound();
            }
            ViewData["CountryId"] = new SelectList(_context.Countries, "Id", "Iso2", airport.CountryId);
            return View(airport);
        }

        // POST: Airports/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Iata,Name,DeparturesLastCheckedUtc,ArrivalsLastCheckedUtc,Longitude,Latitude,DisplayGate,DisplayTerminal,DisplayAirport,CountryId,Id")] Airport airport)
        {
            if (id != airport.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(airport);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AirportExists(airport.Id))
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
            ViewData["CountryId"] = new SelectList(_context.Countries, "Id", "Iso2", airport.CountryId);
            return View(airport);
        }

        // GET: Airports/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null || _context.Airports == null)
            {
                return NotFound();
            }

            var airport = await _context.Airports
                .Include(a => a.Country)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (airport == null)
            {
                return NotFound();
            }

            return View(airport);
        }

        // POST: Airports/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            if (_context.Airports == null)
            {
                return Problem("Entity set 'AppDbContext.Airports'  is null.");
            }
            var airport = await _context.Airports.FindAsync(id);
            if (airport != null)
            {
                _context.Airports.Remove(airport);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AirportExists(Guid id)
        {
          return (_context.Airports?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
