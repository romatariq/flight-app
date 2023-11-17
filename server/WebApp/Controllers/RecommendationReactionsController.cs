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
    public class RecommendationReactionsController : Controller
    {
        private readonly AppDbContext _context;

        public RecommendationReactionsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: RecommendationReactions
        public async Task<IActionResult> Index()
        {
            var appDbContext = _context.RecommendationReactions.Include(r => r.AppUser).Include(r => r.Recommendation);
            return View(await appDbContext.ToListAsync());
        }

        // GET: RecommendationReactions/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null || _context.RecommendationReactions == null)
            {
                return NotFound();
            }

            var recommendationReaction = await _context.RecommendationReactions
                .Include(r => r.AppUser)
                .Include(r => r.Recommendation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (recommendationReaction == null)
            {
                return NotFound();
            }

            return View(recommendationReaction);
        }

        // GET: RecommendationReactions/Create
        public IActionResult Create()
        {
            ViewData["AppUserId"] = new SelectList(_context.AppUsers, "Id", "Id");
            ViewData["RecommendationId"] = new SelectList(_context.Recommendations, "Id", "RecommendationText");
            return View();
        }

        // POST: RecommendationReactions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IsPositiveReaction,CreatedAtUtc,RecommendationId,AppUserId,Id")] RecommendationReaction recommendationReaction)
        {
            if (ModelState.IsValid)
            {
                recommendationReaction.Id = Guid.NewGuid();
                _context.Add(recommendationReaction);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["AppUserId"] = new SelectList(_context.AppUsers, "Id", "Id", recommendationReaction.AppUserId);
            ViewData["RecommendationId"] = new SelectList(_context.Recommendations, "Id", "RecommendationText", recommendationReaction.RecommendationId);
            return View(recommendationReaction);
        }

        // GET: RecommendationReactions/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null || _context.RecommendationReactions == null)
            {
                return NotFound();
            }

            var recommendationReaction = await _context.RecommendationReactions.FindAsync(id);
            if (recommendationReaction == null)
            {
                return NotFound();
            }
            ViewData["AppUserId"] = new SelectList(_context.AppUsers, "Id", "Id", recommendationReaction.AppUserId);
            ViewData["RecommendationId"] = new SelectList(_context.Recommendations, "Id", "RecommendationText", recommendationReaction.RecommendationId);
            return View(recommendationReaction);
        }

        // POST: RecommendationReactions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("IsPositiveReaction,CreatedAtUtc,RecommendationId,AppUserId,Id")] RecommendationReaction recommendationReaction)
        {
            if (id != recommendationReaction.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(recommendationReaction);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RecommendationReactionExists(recommendationReaction.Id))
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
            ViewData["AppUserId"] = new SelectList(_context.AppUsers, "Id", "Id", recommendationReaction.AppUserId);
            ViewData["RecommendationId"] = new SelectList(_context.Recommendations, "Id", "RecommendationText", recommendationReaction.RecommendationId);
            return View(recommendationReaction);
        }

        // GET: RecommendationReactions/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null || _context.RecommendationReactions == null)
            {
                return NotFound();
            }

            var recommendationReaction = await _context.RecommendationReactions
                .Include(r => r.AppUser)
                .Include(r => r.Recommendation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (recommendationReaction == null)
            {
                return NotFound();
            }

            return View(recommendationReaction);
        }

        // POST: RecommendationReactions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            if (_context.RecommendationReactions == null)
            {
                return Problem("Entity set 'AppDbContext.RecommendationReactions'  is null.");
            }
            var recommendationReaction = await _context.RecommendationReactions.FindAsync(id);
            if (recommendationReaction != null)
            {
                _context.RecommendationReactions.Remove(recommendationReaction);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RecommendationReactionExists(Guid id)
        {
          return (_context.RecommendationReactions?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
