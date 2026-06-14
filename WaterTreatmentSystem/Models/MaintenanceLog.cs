using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WaterTreatmentSystem.Models
{
    public class MaintenanceLog
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "المحطة")]
        public int PlantId { get; set; }

        [ForeignKey("PlantId")]
        public virtual Plant Plant { get; set; }

        [Required(ErrorMessage = "تاريخ الصيانة مطلوب")]
        [Display(Name = "تاريخ الصيانة")]
        [DataType(DataType.Date)]
        public DateTime MaintenanceDate { get; set; } = DateTime.Today;

        [Required(ErrorMessage = "وصف الصيانة مطلوب")]
        [Display(Name = "تفاصيل العمل والخدمة")]
        public string Description { get; set; } // مثال: غسيل كيميائي CIP للأغشية أو تنظيف خزان الموازنة

        [Required(ErrorMessage = "التكلفة مطلوبة")]
        [Display(Name = "التكلفة الإجمالية")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Cost { get; set; }

        [Required(ErrorMessage = "اسم الفني مطلوب")]
        [Display(Name = "المهندس/الفني المسؤول")]
        public string TechnicianName { get; set; }
    }
}