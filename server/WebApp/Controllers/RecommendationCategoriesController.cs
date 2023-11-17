#pragma warning disable 1591
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using App.DAL.EF;
using App.Domain;
using Microsoft.AspNetCore.Authorization;

namespace WebApp.Controllers
{
    [Authorize(Roles = "admin")]
    public class RecommendationCategoriesController : Controller
    {
        private readonly AppDbContext _context;

        public RecommendationCategoriesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: RecommendationCategories
        public async Task<IActionResult> Index()
        {
              return _context.RecommendationCategories != null ? 
                          View(await _context.RecommendationCategories.ToListAsync()) :
                          Problem("Entity set 'AppDbContext.RecommendationCategories'  is null.");
        }

        // GET: RecommendationCategories/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null || _context.RecommendationCategories == null)
            {
                return NotFound();
            }

            var recommendationCategory = await _context.RecommendationCategories
                .FirstOrDefaultAsync(m => m.Id == id);
            if (recommendationCategory == null)
            {
                return NotFound();
            }

            return View(recommendationCategory);
        }

        // GET: RecommendationCategories/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: RecommendationCategories/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Category")] RecommendationCategory recommendationCategory)
        {
            if (ModelState.IsValid)
            {
                recommendationCategory.Id = Guid.NewGuid();
                _context.Add(recommendationCategory);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(recommendationCategory);
        }

        // GET: RecommendationCategories/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null || _context.RecommendationCategories == null)
            {
                return NotFound();
            }

            var recommendationCategory = await _context.RecommendationCategories.FindAsync(id);
            if (recommendationCategory == null)
            {
                return NotFound();
            }
            return View(recommendationCategory);
        }

        // POST: RecommendationCategories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,Category")] RecommendationCategory recommendationCategory)
        {
            if (id != recommendationCategory.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(recommendationCategory);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RecommendationCategoryExists(recommendationCategory.Id))
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
            return View(recommendationCategory);
        }

        // GET: RecommendationCategories/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null || _context.RecommendationCategories == null)
            {
                return NotFound();
            }

            var recommendationCategory = await _context.RecommendationCategories
                .FirstOrDefaultAsync(m => m.Id == id);
            if (recommendationCategory == null)
            {
                return NotFound();
            }

            return View(recommendationCategory);
        }

        // POST: RecommendationCategories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            if (_context.RecommendationCategories == null)
            {
                return Problem("Entity set 'AppDbContext.RecommendationCategories'  is null.");
            }
            var recommendationCategory = await _context.RecommendationCategories.FindAsync(id);
            if (recommendationCategory != null)
            {
                _context.RecommendationCategories.Remove(recommendationCategory);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RecommendationCategoryExists(Guid id)
        {
          return (_context.RecommendationCategories?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
