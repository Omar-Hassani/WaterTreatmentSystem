using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WaterTreatmentSystem.Models
{
    public class Plant
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "اسم المحطة مطلوب")]
        [Display(Name = "اسم المحطة")]
        [StringLength(100)]
        public string Name { get; set; }

        [Required(ErrorMessage = "الموقع مطلوب")]
        [Display(Name = "الموقع / العنوان")]
        public string Location { get; set; }

        [Required(ErrorMessage = "الطاقة الإنتاجية مطلوبة")]
        [Display(Name = "الطاقة الإنتاجية (متر مكعب/يوم)")]
        [Range(1, 1000000, ErrorMessage = "يجب أن تكون الطاقة الإنتاجية قيمة موجبة")]
        public double Capacity { get; set; }

        [Required(ErrorMessage = "نوع النظام مطلوب")]
        [Display(Name = "نوع النظام المعالج")]
        public string PlantType { get; set; } // مثال: RO (تناضح عكسي) أو UF (ترشيح فائق)

        [Required(ErrorMessage = "تاريخ التركيب مطلوب")]
        [Display(Name = "تاريخ التركيب والتشغيل")]
        [DataType(DataType.Date)]
        public DateTime InstallationDate { get; set; }

        // علاقات قاعدة البيانات (Navigation Properties)
        public virtual ICollection<WaterQualityAnalysis> QualityAnalyses { get; set; } = new List<WaterQualityAnalysis>();
        public virtual ICollection<PlantComponent> Components { get; set; } = new List<PlantComponent>();
        public virtual ICollection<MaintenanceLog> MaintenanceLogs { get; set; } = new List<MaintenanceLog>();
    }
}