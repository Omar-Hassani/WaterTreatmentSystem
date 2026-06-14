using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WaterTreatmentSystem.Models
{
    public class PlantComponent
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "المحطة")]
        public int PlantId { get; set; }

        [ForeignKey("PlantId")]
        public virtual Plant Plant { get; set; }

        [Required(ErrorMessage = "اسم الجزء أو المستهلك مطلوب")]
        [Display(Name = "اسم الفلتر/الجزء")]
        [StringLength(100)]
        public string ComponentName { get; set; } // مثال: RO Membrane, Carbon Filter, Cartridge Filter

        [Required(ErrorMessage = "العمر الافتراضي مطلوب")]
        [Display(Name = "العمر الافتراضي (بالأيام)")]
        public int LifetimeDays { get; set; } // كم يوم يعيش هذا الفلتر قبل تبديله؟

        [Required(ErrorMessage = "تاريخ آخر تبديل مطلوب")]
        [Display(Name = "تاريخ آخر تبديل")]
        [DataType(DataType.Date)]
        public DateTime LastReplacementDate { get; set; }

        // 🧠 خاصية ذكية: حساب تاريخ التبديل القادم تلقائياً بجمع تاريخ آخر تبديل + عمره الافتراضي
        [NotMapped]
        [Display(Name = "تاريخ التبديل القادم")]
        public DateTime NextReplacementDate => LastReplacementDate.AddDays(LifetimeDays);

        // 🧠 خاصية ذكية: حساب الأيام المتبقية أو كم يوم مضى على انتهاء الصلاحية
        [NotMapped]
        public int DaysRemaining => (NextReplacementDate - DateTime.Today).Days;

        // 🧠 خاصية ذكية: هل الفلتر تالف ويحتاج تغيير فوري؟
        [NotMapped]
        [Display(Name = "حالة الجزء")]
        public bool NeedsUrgentReplacement => DateTime.Today >= NextReplacementDate;
    }
}