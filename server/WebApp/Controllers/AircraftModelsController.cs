#pragma warning disable 1591
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using App.DAL.EF;
using App.Domain;
using Microsoft.AspNetCore.Authorization;

namespace WebApp.Controllers
{
    [Authorize(Roles = "admin")]
    public class AircraftModelsController : Controller
    {
        private readonly AppDbContext _context;

        public AircraftModelsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: AircraftModels
        public async Task<IActionResult> Index()
        {
              return _context.AircraftModels != null ? 
                          View(await _context.AircraftModels.ToListAsync()) :
                          Problem("Entity set 'AppDbContext.AircraftModels'  is null.");
        }

        // GET: AircraftModels/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null || _context.AircraftModels == null)
            {
                return NotFound();
            }

            var aircraftModel = await _context.AircraftModels
                .FirstOrDefaultAsync(m => m.Id == id);
            if (aircraftModel == null)
            {
                return NotFound();
            }

            return View(aircraftModel);
        }

        // GET: AircraftModels/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: AircraftModels/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ModelName,ModelCode,Id")] AircraftModel aircraftModel)
        {
            if (ModelState.IsValid)
            {
                aircraftModel.Id = Guid.NewGuid();
                _context.Add(aircraftModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(aircraftModel);
        }

        // GET: AircraftModels/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null || _context.AircraftModels == null)
            {
                return NotFound();
            }

            var aircraftModel = await _context.AircraftModels.FindAsync(id);
            if (aircraftModel == null)
            {
                return NotFound();
            }
            return View(aircraftModel);
        }

        // POST: AircraftModels/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("ModelName,ModelCode,Id")] AircraftModel aircraftModel)
        {
            if (id != aircraftModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(aircraftModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AircraftModelExists(aircraftModel.Id))
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
            return View(aircraftModel);
        }

        // GET: AircraftModels/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null || _context.AircraftModels == null)
            {
                return NotFound();
            }

            var aircraftModel = await _context.AircraftModels
                .FirstOrDefaultAsync(m => m.Id == id);
            if (aircraftModel == null)
            {
                return NotFound();
            }

            return View(aircraftModel);
        }

        // POST: AircraftModels/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            if (_context.AircraftModels == null)
            {
                return Problem("Entity set 'AppDbContext.AircraftModels'  is null.");
            }
            var aircraftModel = await _context.AircraftModels.FindAsync(id);
            if (aircraftModel != null)
            {
                _context.AircraftModels.Remove(aircraftModel);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AircraftModelExists(Guid id)
        {
          return (_context.AircraftModels?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
