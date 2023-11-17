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
    public class FlightsController : Controller
    {
        private readonly AppDbContext _context;

        public FlightsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Flights
        public async Task<IActionResult> Index()
        {
            var appDbContext = _context.Flights.Include(f => f.Aircraft).Include(f => f.Airline).Include(f => f.ArrivalAirport).Include(f => f.DepartureAirport).Include(f => f.FlightStatus);
            return View(await appDbContext.ToListAsync());
        }

        // GET: Flights/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null || _context.Flights == null)
            {
                return NotFound();
            }

            var flight = await _context.Flights
                .Include(f => f.Aircraft)
                .Include(f => f.Airline)
                .Include(f => f.ArrivalAirport)
                .Include(f => f.DepartureAirport)
                .Include(f => f.FlightStatus)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (flight == null)
            {
                return NotFound();
            }

            return View(flight);
        }

        // GET: Flights/Create
        public IActionResult Create()
        {
            ViewData["AircraftId"] = new SelectList(_context.Aircrafts, "Id", "IcaoHex");
            ViewData["AirlineId"] = new SelectList(_context.Airlines, "Id", "Iata");
            ViewData["ArrivalAirportId"] = new SelectList(_context.Airports, "Id", "Iata");
            ViewData["DepartureAirportId"] = new SelectList(_context.Airports, "Id", "Iata");
            ViewData["FlightStatusId"] = new SelectList(_context.FlightStatuses, "Id", "Name");
            return View();
        }

        // POST: Flights/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("FlightIata,ScheduledDepartureUtc,ScheduledArrivalUtc,ExpectedDepartureUtc,ExpectedArrivalUtc,ScheduledDepartureLocal,ScheduledArrivalLocal,ExpectedDepartureLocal,ExpectedArrivalLocal,DepartureTerminal,ArrivalTerminal,DepartureGate,ArrivalGate,FlightInfoLastCheckedUtc,DepartureAirportId,ArrivalAirportId,FlightStatusId,AirlineId,AircraftId,Id")] Flight flight)
        {
            if (ModelState.IsValid)
            {
                flight.Id = Guid.NewGuid();
                _context.Add(flight);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["AircraftId"] = new SelectList(_context.Aircrafts, "Id", "IcaoHex", flight.AircraftId);
            ViewData["AirlineId"] = new SelectList(_context.Airlines, "Id", "Iata", flight.AirlineId);
            ViewData["ArrivalAirportId"] = new SelectList(_context.Airports, "Id", "Iata", flight.ArrivalAirportId);
            ViewData["DepartureAirportId"] = new SelectList(_context.Airports, "Id", "Iata", flight.DepartureAirportId);
            ViewData["FlightStatusId"] = new SelectList(_context.FlightStatuses, "Id", "Name", flight.FlightStatusId);
            return View(flight);
        }

        // GET: Flights/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null || _context.Flights == null)
            {
                return NotFound();
            }

            var flight = await _context.Flights.FindAsync(id);
            if (flight == null)
            {
                return NotFound();
            }
            ViewData["AircraftId"] = new SelectList(_context.Aircrafts, "Id", "IcaoHex", flight.AircraftId);
            ViewData["AirlineId"] = new SelectList(_context.Airlines, "Id", "Iata", flight.AirlineId);
            ViewData["ArrivalAirportId"] = new SelectList(_context.Airports, "Id", "Iata", flight.ArrivalAirportId);
            ViewData["DepartureAirportId"] = new SelectList(_context.Airports, "Id", "Iata", flight.DepartureAirportId);
            ViewData["FlightStatusId"] = new SelectList(_context.FlightStatuses, "Id", "Name", flight.FlightStatusId);
            return View(flight);
        }

        // POST: Flights/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("FlightIata,ScheduledDepartureUtc,ScheduledArrivalUtc,ExpectedDepartureUtc,ExpectedArrivalUtc,ScheduledDepartureLocal,ScheduledArrivalLocal,ExpectedDepartureLocal,ExpectedArrivalLocal,DepartureTerminal,ArrivalTerminal,DepartureGate,ArrivalGate,FlightInfoLastCheckedUtc,DepartureAirportId,ArrivalAirportId,FlightStatusId,AirlineId,AircraftId,Id")] Flight flight)
        {
            if (id != flight.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(flight);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FlightExists(flight.Id))
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
            ViewData["AircraftId"] = new SelectList(_context.Aircrafts, "Id", "IcaoHex", flight.AircraftId);
            ViewData["AirlineId"] = new SelectList(_context.Airlines, "Id", "Iata", flight.AirlineId);
            ViewData["ArrivalAirportId"] = new SelectList(_context.Airports, "Id", "Iata", flight.ArrivalAirportId);
            ViewData["DepartureAirportId"] = new SelectList(_context.Airports, "Id", "Iata", flight.DepartureAirportId);
            ViewData["FlightStatusId"] = new SelectList(_context.FlightStatuses, "Id", "Name", flight.FlightStatusId);
            return View(flight);
        }

        // GET: Flights/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null || _context.Flights == null)
            {
                return NotFound();
            }

            var flight = await _context.Flights
                .Include(f => f.Aircraft)
                .Include(f => f.Airline)
                .Include(f => f.ArrivalAirport)
                .Include(f => f.DepartureAirport)
                .Include(f => f.FlightStatus)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (flight == null)
            {
                return NotFound();
            }

            return View(flight);
        }

        // POST: Flights/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            if (_context.Flights == null)
            {
                return Problem("Entity set 'AppDbContext.Flights'  is null.");
            }
            var flight = await _context.Flights.FindAsync(id);
            if (flight != null)
            {
                _context.Flights.Remove(flight);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FlightExists(Guid id)
        {
          return (_context.Flights?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
