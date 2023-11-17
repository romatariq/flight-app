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
    public class RecommendationsController : Controller
    {
        private readonly AppDbContext _context;

        public RecommendationsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Recommendations
        public async Task<IActionResult> Index()
        {
            var appDbContext = _context.Recommendations.Include(r => r.Airport).Include(r => r.AppUser).Include(r => r.RecommendationCategory);
            return View(await appDbContext.ToListAsync());
        }

        // GET: Recommendations/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null || _context.Recommendations == null)
            {
                return NotFound();
            }

            var recommendation = await _context.Recommendations
                .Include(r => r.Airport)
                .Include(r => r.AppUser)
                .Include(r => r.RecommendationCategory)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (recommendation == null)
            {
                return NotFound();
            }

            return View(recommendation);
        }

        // GET: Recommendations/Create
        public IActionResult Create()
        {
            ViewData["AirportId"] = new SelectList(_context.Airports, "Id", "Iata");
            ViewData["AppUserId"] = new SelectList(_context.AppUsers, "Id", "Id");
            ViewData["RecommendationCategoryId"] = new SelectList(_context.RecommendationCategories, "Id", "Category");
            return View();
        }

        // POST: Recommendations/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("RecommendationText,Rating,CreatedAtUtc,PostRating,AirportId,RecommendationCategoryId,AppUserId,Id")] Recommendation recommendation)
        {
            if (ModelState.IsValid)
            {
                recommendation.Id = Guid.NewGuid();
                _context.Add(recommendation);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["AirportId"] = new SelectList(_context.Airports, "Id", "Iata", recommendation.AirportId);
            ViewData["AppUserId"] = new SelectList(_context.AppUsers, "Id", "Id", recommendation.AppUserId);
            ViewData["RecommendationCategoryId"] = new SelectList(_context.RecommendationCategories, "Id", "Category", recommendation.RecommendationCategoryId);
            return View(recommendation);
        }

        // GET: Recommendations/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null || _context.Recommendations == null)
            {
                return NotFound();
            }

            var recommendation = await _context.Recommendations.FindAsync(id);
            if (recommendation == null)
            {
                return NotFound();
            }
            ViewData["AirportId"] = new SelectList(_context.Airports, "Id", "Iata", recommendation.AirportId);
            ViewData["AppUserId"] = new SelectList(_context.AppUsers, "Id", "Id", recommendation.AppUserId);
            ViewData["RecommendationCategoryId"] = new SelectList(_context.RecommendationCategories, "Id", "Category", recommendation.RecommendationCategoryId);
            return View(recommendation);
        }

        // POST: Recommendations/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("RecommendationText,Rating,CreatedAtUtc,PostRating,AirportId,RecommendationCategoryId,AppUserId,Id")] Recommendation recommendation)
        {
            if (id != recommendation.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(recommendation);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RecommendationExists(recommendation.Id))
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
            ViewData["AirportId"] = new SelectList(_context.Airports, "Id", "Iata", recommendation.AirportId);
            ViewData["AppUserId"] = new SelectList(_context.AppUsers, "Id", "Id", recommendation.AppUserId);
            ViewData["RecommendationCategoryId"] = new SelectList(_context.RecommendationCategories, "Id", "Category", recommendation.RecommendationCategoryId);
            return View(recommendation);
        }

        // GET: Recommendations/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null || _context.Recommendations == null)
            {
                return NotFound();
            }

            var recommendation = await _context.Recommendations
                .Include(r => r.Airport)
                .Include(r => r.AppUser)
                .Include(r => r.RecommendationCategory)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (recommendation == null)
            {
                return NotFound();
            }

            return View(recommendation);
        }

        // POST: Recommendations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            if (_context.Recommendations == null)
            {
                return Problem("Entity set 'AppDbContext.Recommendations'  is null.");
            }
            var recommendation = await _context.Recommendations.FindAsync(id);
            if (recommendation != null)
            {
                _context.Recommendations.Remove(recommendation);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RecommendationExists(Guid id)
        {
          return (_context.Recommendations?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
