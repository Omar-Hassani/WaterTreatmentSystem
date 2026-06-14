using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WaterTreatmentSystem.Data;
using WaterTreatmentSystem.Models;

namespace WaterTreatmentSystem.Controllers
{
    public class WaterQualityController : Controller
    {
        private readonly ApplicationDbContext _context;

        public WaterQualityController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ➕ 1. شاشة إضافة تحليل مخبري جديد (GET)
        public async Task<IActionResult> Create(int plantId)
        {
            var plant = await _context.Plants.FindAsync(plantId);
            if (plant == null) return NotFound();

            var model = new WaterQualityAnalysis
            {
                PlantId = plantId,
                AnalysisDate = DateTime.Now
            };

            ViewBag.PlantName = plant.Name;
            return View(model);
        }

        // 💾 2. استقبال بيانات التحليل الجديد (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(WaterQualityAnalysis analysis)
        {
            ModelState.Remove("Plant");

            if (ModelState.IsValid)
            {
                _context.WaterQualityAnalyses.Add(analysis);
                await _context.SaveChangesAsync();
                return RedirectToAction("Details", "Plants", new { id = analysis.PlantId });
            }

            var plant = await _context.Plants.FindAsync(analysis.PlantId);
            ViewBag.PlantName = plant?.Name;
            return View(analysis);
        }

        // 📝 3. شاشة تعديل تحليل مخبري سابق (GET)
        public async Task<IActionResult> Edit(int id)
        {
            var analysis = await _context.WaterQualityAnalyses
                .Include(a => a.Plant)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (analysis == null) return NotFound();

            ViewBag.PlantName = analysis.Plant?.Name;
            return View(analysis);
        }

        // 💾 4. حفظ تعديلات التحليل المخبري (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, WaterQualityAnalysis analysis)
        {
            if (id != analysis.Id) return NotFound();

            ModelState.Remove("Plant");

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(analysis);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.WaterQualityAnalyses.Any(e => e.Id == analysis.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction("Details", "Plants", new { id = analysis.PlantId });
            }

            var plant = await _context.Plants.FindAsync(analysis.PlantId);
            ViewBag.PlantName = plant?.Name;
            return View(analysis);
        }

        // ❌ 5. إجراء الحذف المباشر (POST) لتبسيط العملية بدون شاشة تأكيد منفصلة
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var analysis = await _context.WaterQualityAnalyses.FindAsync(id);
            if (analysis == null) return NotFound();

            int plantId = analysis.PlantId; // الاحتفاظ بالمعرف للعودة لنفس المحطة

            _context.WaterQualityAnalyses.Remove(analysis);
            await _context.SaveChangesAsync();

            return RedirectToAction("Details", "Plants", new { id = plantId });
        }

       
    }
}