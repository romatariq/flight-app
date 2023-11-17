#pragma warning disable 1591
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using App.DAL.EF;
using App.Domain;
using Microsoft.AspNetCore.Authorization;

namespace WebApp.Controllers
{
    [Authorize(Roles = "admin")]
    public class FlightStatusesController : Controller
    {
        private readonly AppDbContext _context;

        public FlightStatusesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: FlightStatuses
        public async Task<IActionResult> Index()
        {
              return _context.FlightStatuses != null ? 
                          View(await _context.FlightStatuses.ToListAsync()) :
                          Problem("Entity set 'AppDbContext.FlightStatuses'  is null.");
        }

        // GET: FlightStatuses/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null || _context.FlightStatuses == null)
            {
                return NotFound();
            }

            var flightStatus = await _context.FlightStatuses
                .FirstOrDefaultAsync(m => m.Id == id);
            if (flightStatus == null)
            {
                return NotFound();
            }

            return View(flightStatus);
        }

        // GET: FlightStatuses/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: FlightStatuses/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name")] FlightStatus flightStatus)
        {
            if (ModelState.IsValid)
            {
                flightStatus.Id = Guid.NewGuid();
                _context.Add(flightStatus);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(flightStatus);
        }

        // GET: FlightStatuses/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null || _context.FlightStatuses == null)
            {
                return NotFound();
            }

            var flightStatus = await _context.FlightStatuses.FindAsync(id);
            if (flightStatus == null)
            {
                return NotFound();
            }
            return View(flightStatus);
        }

        // POST: FlightStatuses/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,Name")] FlightStatus flightStatus)
        {
            if (id != flightStatus.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(flightStatus);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FlightStatusExists(flightStatus.Id))
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
            return View(flightStatus);
        }

        // GET: FlightStatuses/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null || _context.FlightStatuses == null)
            {
                return NotFound();
            }

            var flightStatus = await _context.FlightStatuses
                .FirstOrDefaultAsync(m => m.Id == id);
            if (flightStatus == null)
            {
                return NotFound();
            }

            return View(flightStatus);
        }

        // POST: FlightStatuses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            if (_context.FlightStatuses == null)
            {
                return Problem("Entity set 'AppDbContext.FlightStatuses'  is null.");
            }
            var flightStatus = await _context.FlightStatuses.FindAsync(id);
            if (flightStatus != null)
            {
                _context.FlightStatuses.Remove(flightStatus);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FlightStatusExists(Guid id)
        {
          return (_context.FlightStatuses?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
