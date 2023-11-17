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
    public class UserFlightsController : Controller
    {
        private readonly AppDbContext _context;

        public UserFlightsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: UserFlights
        public async Task<IActionResult> Index()
        {
            var appDbContext = _context.UserFlights.Include(u => u.AppUser).Include(u => u.Flight);
            return View(await appDbContext.ToListAsync());
        }

        // GET: UserFlights/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null || _context.UserFlights == null)
            {
                return NotFound();
            }

            var userFlight = await _context.UserFlights
                .Include(u => u.AppUser)
                .Include(u => u.Flight)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (userFlight == null)
            {
                return NotFound();
            }

            return View(userFlight);
        }

        // GET: UserFlights/Create
        public IActionResult Create()
        {
            ViewData["AppUserId"] = new SelectList(_context.AppUsers, "Id", "Id");
            ViewData["FlightId"] = new SelectList(_context.Flights, "Id", "FlightIata");
            return View();
        }

        // POST: UserFlights/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("NotifyGateChanges,FlightId,AppUserId,Id")] UserFlight userFlight)
        {
            if (ModelState.IsValid)
            {
                userFlight.Id = Guid.NewGuid();
                _context.Add(userFlight);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["AppUserId"] = new SelectList(_context.AppUsers, "Id", "Id", userFlight.AppUserId);
            ViewData["FlightId"] = new SelectList(_context.Flights, "Id", "FlightIata", userFlight.FlightId);
            return View(userFlight);
        }

        // GET: UserFlights/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null || _context.UserFlights == null)
            {
                return NotFound();
            }

            var userFlight = await _context.UserFlights.FindAsync(id);
            if (userFlight == null)
            {
                return NotFound();
            }
            ViewData["AppUserId"] = new SelectList(_context.AppUsers, "Id", "Id", userFlight.AppUserId);
            ViewData["FlightId"] = new SelectList(_context.Flights, "Id", "FlightIata", userFlight.FlightId);
            return View(userFlight);
        }

        // POST: UserFlights/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("NotifyGateChanges,FlightId,AppUserId,Id")] UserFlight userFlight)
        {
            if (id != userFlight.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(userFlight);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserFlightExists(userFlight.Id))
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
            ViewData["AppUserId"] = new SelectList(_context.AppUsers, "Id", "Id", userFlight.AppUserId);
            ViewData["FlightId"] = new SelectList(_context.Flights, "Id", "FlightIata", userFlight.FlightId);
            return View(userFlight);
        }

        // GET: UserFlights/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null || _context.UserFlights == null)
            {
                return NotFound();
            }

            var userFlight = await _context.UserFlights
                .Include(u => u.AppUser)
                .Include(u => u.Flight)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (userFlight == null)
            {
                return NotFound();
            }

            return View(userFlight);
        }

        // POST: UserFlights/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            if (_context.UserFlights == null)
            {
                return Problem("Entity set 'AppDbContext.UserFlights'  is null.");
            }
            var userFlight = await _context.UserFlights.FindAsync(id);
            if (userFlight != null)
            {
                _context.UserFlights.Remove(userFlight);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserFlightExists(Guid id)
        {
          return (_context.UserFlights?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
