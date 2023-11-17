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
    public class UserFlightNotificationsController : Controller
    {
        private readonly AppDbContext _context;

        public UserFlightNotificationsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: UserFlightNotifications
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.UserFlightNotifications.Include(u => u.Notification).Include(u => u.UserFlight);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: UserFlightNotifications/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null || _context.UserFlightNotifications == null)
            {
                return NotFound();
            }

            var userFlightNotification = await _context.UserFlightNotifications
                .Include(u => u.Notification)
                .Include(u => u.UserFlight)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (userFlightNotification == null)
            {
                return NotFound();
            }

            return View(userFlightNotification);
        }

        // GET: UserFlightNotifications/Create
        public IActionResult Create()
        {
            ViewData["NotificationId"] = new SelectList(_context.Notifications, "Id", "NotificationType");
            ViewData["UserFlightId"] = new SelectList(_context.UserFlights, "Id", "AppUserId");
            return View();
        }

        // POST: UserFlightNotifications/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,MinutesFromEvent,UserFlightId,NotificationId")] UserFlightNotification userFlightNotification)
        {
            if (ModelState.IsValid)
            {
                userFlightNotification.Id = Guid.NewGuid();
                _context.Add(userFlightNotification);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["NotificationId"] = new SelectList(_context.Notifications, "Id", "NotificationType", userFlightNotification.NotificationId);
            ViewData["UserFlightId"] = new SelectList(_context.UserFlights, "Id", "AppUserId", userFlightNotification.UserFlightId);
            return View(userFlightNotification);
        }

        // GET: UserFlightNotifications/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null || _context.UserFlightNotifications == null)
            {
                return NotFound();
            }

            var userFlightNotification = await _context.UserFlightNotifications.FindAsync(id);
            if (userFlightNotification == null)
            {
                return NotFound();
            }
            ViewData["NotificationId"] = new SelectList(_context.Notifications, "Id", "NotificationType", userFlightNotification.NotificationId);
            ViewData["UserFlightId"] = new SelectList(_context.UserFlights, "Id", "AppUserId", userFlightNotification.UserFlightId);
            return View(userFlightNotification);
        }

        // POST: UserFlightNotifications/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,MinutesFromEvent,UserFlightId,NotificationId")] UserFlightNotification userFlightNotification)
        {
            if (id != userFlightNotification.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(userFlightNotification);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserFlightNotificationExists(userFlightNotification.Id))
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
            ViewData["NotificationId"] = new SelectList(_context.Notifications, "Id", "NotificationType", userFlightNotification.NotificationId);
            ViewData["UserFlightId"] = new SelectList(_context.UserFlights, "Id", "AppUserId", userFlightNotification.UserFlightId);
            return View(userFlightNotification);
        }

        // GET: UserFlightNotifications/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null || _context.UserFlightNotifications == null)
            {
                return NotFound();
            }

            var userFlightNotification = await _context.UserFlightNotifications
                .Include(u => u.Notification)
                .Include(u => u.UserFlight)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (userFlightNotification == null)
            {
                return NotFound();
            }

            return View(userFlightNotification);
        }

        // POST: UserFlightNotifications/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            if (_context.UserFlightNotifications == null)
            {
                return Problem("Entity set 'AppDbContext.UserFlightNotifications'  is null.");
            }
            var userFlightNotification = await _context.UserFlightNotifications.FindAsync(id);
            if (userFlightNotification != null)
            {
                _context.UserFlightNotifications.Remove(userFlightNotification);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserFlightNotificationExists(Guid id)
        {
          return (_context.UserFlightNotifications?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
