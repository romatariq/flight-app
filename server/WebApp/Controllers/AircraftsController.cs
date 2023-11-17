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
    public class AircraftsController : Controller
    {
        private readonly AppDbContext _context;

        public AircraftsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Aircrafts
        public async Task<IActionResult> Index()
        {
            var appDbContext = _context.Aircrafts.Include(a => a.AircraftModel);
            return View(await appDbContext.ToListAsync());
        }

        // GET: Aircrafts/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null || _context.Aircrafts == null)
            {
                return NotFound();
            }

            var aircraft = await _context.Aircrafts
                .Include(a => a.AircraftModel)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (aircraft == null)
            {
                return NotFound();
            }

            return View(aircraft);
        }

        // GET: Aircrafts/Create
        public IActionResult Create()
        {
            ViewData["AircraftModelId"] = new SelectList(_context.AircraftModels, "Id", "ModelCode");
            return View();
        }

        // POST: Aircrafts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IcaoHex,RegistrationNumber,Latitude,Longitude,SpeedKmh,InfoLastUpdatedUtc,AircraftModelId,Id")] Aircraft aircraft)
        {
            if (ModelState.IsValid)
            {
                aircraft.Id = Guid.NewGuid();
                _context.Add(aircraft);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["AircraftModelId"] = new SelectList(_context.AircraftModels, "Id", "ModelCode", aircraft.AircraftModelId);
            return View(aircraft);
        }

        // GET: Aircrafts/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null || _context.Aircrafts == null)
            {
                return NotFound();
            }

            var aircraft = await _context.Aircrafts.FindAsync(id);
            if (aircraft == null)
            {
                return NotFound();
            }
            ViewData["AircraftModelId"] = new SelectList(_context.AircraftModels, "Id", "ModelCode", aircraft.AircraftModelId);
            return View(aircraft);
        }

        // POST: Aircrafts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("IcaoHex,RegistrationNumber,Latitude,Longitude,SpeedKmh,InfoLastUpdatedUtc,AircraftModelId,Id")] Aircraft aircraft)
        {
            if (id != aircraft.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(aircraft);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AircraftExists(aircraft.Id))
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
            ViewData["AircraftModelId"] = new SelectList(_context.AircraftModels, "Id", "ModelCode", aircraft.AircraftModelId);
            return View(aircraft);
        }

        // GET: Aircrafts/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null || _context.Aircrafts == null)
            {
                return NotFound();
            }

            var aircraft = await _context.Aircrafts
                .Include(a => a.AircraftModel)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (aircraft == null)
            {
                return NotFound();
            }

            return View(aircraft);
        }

        // POST: Aircrafts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            if (_context.Aircrafts == null)
            {
                return Problem("Entity set 'AppDbContext.Aircrafts'  is null.");
            }
            var aircraft = await _context.Aircrafts.FindAsync(id);
            if (aircraft != null)
            {
                _context.Aircrafts.Remove(aircraft);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AircraftExists(Guid id)
        {
          return (_context.Aircrafts?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
