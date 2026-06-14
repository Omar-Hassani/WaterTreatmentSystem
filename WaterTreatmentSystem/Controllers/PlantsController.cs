using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using WaterTreatmentSystem.Data;
using WaterTreatmentSystem.Models;

namespace WaterTreatmentSystem.Controllers
{
    public class PlantsController : Controller
    {
        private readonly ApplicationDbContext _context;

        // حقن سياق قاعدة البيانات مباشرة في الكنترولر
        public PlantsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // 1. عرض قائمة جميع المحطات (Index)
        public async Task<IActionResult> Index()
        {
            // جلب المحطات مع تضمين المكونات والتحاليل لحساب الإحصائيات في الواجهة
            var plants = await _context.Plants
                .Include(p => p.Components)
                .Include(p => p.QualityAnalyses)
                .ToListAsync();

            return View(plants);
        }

        // 🎮 داخل ملف Controllers/PlantsController.cs

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // 🚀 الحركة الفنية الهامة: شحن كافة الجداول التابعة للمحطة في استعلام واحد متكامل
            var plant = await _context.Plants
                .Include(p => p.QualityAnalyses)   // شحن تحاليل جودة المياه
                .Include(p => p.Components)        // شحن الفلاتر والمستهلكات
                .Include(p => p.MaintenanceLogs)   // شحن سجلات الصيانة
                .FirstOrDefaultAsync(m => m.Id == id);

            if (plant == null)
            {
                return NotFound();
            }

            return View(plant);
        }

        // 3. صفحة إضافة محطة جديدة - GET
        public IActionResult Create()
        {
            return View();
        }

        // 4. استقبال بيانات المحطة الجديدة وحفظها - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Location,Capacity,PlantType,InstallationDate")] Plant plant)
        {
            if (ModelState.IsValid)
            {
                _context.Add(plant);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(plant);
        }

        // 5. صفحة تعديل محطة - GET
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var plant = await _context.Plants.FindAsync(id);
            if (plant == null) return NotFound();

            return View(plant);
        }

        // 6. استقبال البيانات المعدلة وحفظها - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Location,Capacity,PlantType,InstallationDate")] Plant plant)
        {
            if (id != plant.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(plant);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PlantExists(plant.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(plant);
        }

        // 7. صفحة تأكيد الحذف - GET
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var plant = await _context.Plants
                .FirstOrDefaultAsync(m => m.Id == id);

            if (plant == null) return NotFound();

            return View(plant);
        }

        // 8. تنفيذ الحذف الفعلي - POST
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var plant = await _context.Plants.FindAsync(id);
            if (plant != null)
            {
                _context.Plants.Remove(plant);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool PlantExists(int id)
        {
            return _context.Plants.Any(e => e.Id == id);
        }

        // ➕ 1. شاشة ربط فلتر أو غشاء جديد بالمحطة (GET)
        public async Task<IActionResult> CreateComponent(int plantId)
        {
            var plant = await _context.Plants.FindAsync(plantId);
            if (plant == null)
            {
                return NotFound();
            }

            // تجهيز موديل فارغ وربطه بمعرف المحطة تلقائياً
            var model = new PlantComponent
            {
                PlantId = plantId,
                LastReplacementDate = DateTime.Today // وضع تاريخ اليوم كافتراضي لآخر تبديل
            };

            ViewBag.PlantName = plant.Name;
            return View(model);
        }

        // 💾 2. استقبال بيانات الفلتر وحفظها (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateComponent(PlantComponent component)
        {
            // إزالة كائن المحطة الملاحي من التحقق لضمان نجاح الـ Validation
            ModelState.Remove("Plant");

            if (ModelState.IsValid)
            {
                _context.PlantComponents.Add(component);
                await _context.SaveChangesAsync();

                // العودة مباشرة إلى لوحة تحكم المحطة (تبويب الفلاتر)
                return RedirectToAction("Details", new { id = component.PlantId });
            }

            var plant = await _context.Plants.FindAsync(component.PlantId);
            ViewBag.PlantName = plant?.Name;
            return View(component);
        }

        // 📝 3. شاشة تعديل بيانات فلتر/مكون سابق (GET)
        public async Task<IActionResult> EditComponent(int id)
        {
            var component = await _context.PlantComponents
                .Include(c => c.Plant)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (component == null) return NotFound();

            ViewBag.PlantName = component.Plant?.Name;
            return View(component);
        }

        // 💾 4. حفظ تعديلات الفلتر/المكون (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditComponent(int id, PlantComponent component)
        {
            if (id != component.Id) return NotFound();

            ModelState.Remove("Plant");

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(component);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.PlantComponents.Any(e => e.Id == component.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction("Details", new { id = component.PlantId });
            }

            var plant = await _context.Plants.FindAsync(component.PlantId);
            ViewBag.PlantName = plant?.Name;
            return View(component);
        }

        // ❌ 5. إجراء حذف المكون/الفلتر مباشرة (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteComponent(int id)
        {
            var component = await _context.PlantComponents.FindAsync(id);
            if (component == null) return NotFound();

            int plantId = component.PlantId; // الاحتفاظ بالمعرف للعودة لنفس المحطة

            _context.PlantComponents.Remove(component);
            await _context.SaveChangesAsync();

            return RedirectToAction("Details", new { id = plantId });
        }

        // ➕ 1. شاشة تدوين أمر صيانة جديد (GET)
        public async Task<IActionResult> CreateMaintenance(int plantId)
        {
            var plant = await _context.Plants.FindAsync(plantId);
            if (plant == null)
            {
                return NotFound();
            }

            var model = new MaintenanceLog
            {
                PlantId = plantId,
                MaintenanceDate = DateTime.Today // وضع تاريخ اليوم كافتراضي
            };

            ViewBag.PlantName = plant.Name;
            return View(model);
        }

        // 💾 2. استقبال بيانات الصيانة وحفظها (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateMaintenance(MaintenanceLog log)
        {
            // إزالة كائن المحطة الملاحي من التحقق لضمان نجاح الـ Validation
            ModelState.Remove("Plant");

            if (ModelState.IsValid)
            {
                _context.MaintenanceLogs.Add(log);
                await _context.SaveChangesAsync();

                // العودة مباشرة إلى لوحة تحكم المحطة (تبويب الصيانة)
                return RedirectToAction("Details", new { id = log.PlantId });
            }

            var plant = await _context.Plants.FindAsync(log.PlantId);
            ViewBag.PlantName = plant?.Name;
            return View(log);
        }

        // 📝 3. شاشة تعديل أمر صيانة سابق (GET)
        public async Task<IActionResult> EditMaintenance(int id)
        {
            var log = await _context.MaintenanceLogs
                .Include(l => l.Plant)
                .FirstOrDefaultAsync(l => l.Id == id);

            if (log == null) return NotFound();

            ViewBag.PlantName = log.Plant?.Name;
            return View(log);
        }

        // 💾 4. حفظ تعديلات أمر الصيانة (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditMaintenance(int id, MaintenanceLog log)
        {
            if (id != log.Id) return NotFound();

            ModelState.Remove("Plant");

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(log);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.MaintenanceLogs.Any(e => e.Id == log.Id)) return NotFound();
                    else throw;
                }
                // العودة إلى لوحة تحكم المحطة
                return RedirectToAction("Details", "Plants", new { id = log.PlantId });
            }

            var plant = await _context.Plants.FindAsync(log.PlantId);
            ViewBag.PlantName = plant?.Name;
            return View(log);
        }

        // ❌ 5. إجراء حذف أمر الصيانة مباشرة (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteMaintenance(int id)
        {
            var log = await _context.MaintenanceLogs.FindAsync(id);
            if (log == null) return NotFound();

            int plantId = log.PlantId; // الاحتفاظ بالمعرف للعودة لنفس المحطة بعد الحذف

            _context.MaintenanceLogs.Remove(log);
            await _context.SaveChangesAsync();

            return RedirectToAction("Details", "Plants", new { id = plantId });
        }
    }
}