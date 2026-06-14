using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WaterTreatmentSystem.Models
{
    public class WaterQualityAnalysis
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "المحطة")]
        public int PlantId { get; set; }

        [ForeignKey("PlantId")]
        public virtual Plant? Plant { get; set; }

        [Required(ErrorMessage = "تاريخ التحليل مطلوب")]
        [Display(Name = "تاريخ وساعة الفحص")]
        public DateTime AnalysisDate { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "نسبة أملاح المياه الخام مطلوبة")]
        [Display(Name = "أملاح المياه الداخلة (TDS Inlet - ppm)")]
        public double TdsInlet { get; set; }

        [Required(ErrorMessage = "نسبة أملاح المياه المنتجة مطلوبة")]
        [Display(Name = "أملاح المياه المنتجة (TDS Outlet - ppm)")]
        public double TdsOutlet { get; set; }

        [Required(ErrorMessage = "درجة الحموضة مطلوبة")]
        [Display(Name = "درجة الحموضة (pH)")]
        [Range(0, 14, ErrorMessage = "قيمة pH يجب أن تكون بين 0 و 14")]
        public double Ph { get; set; }

        [Required(ErrorMessage = "درجة العكارة مطلوبة")]
        [Display(Name = "العكارة (Turbidity - NTU)")]
        public double Turbidity { get; set; }

        // 🧠 خاصية ذكية برمجياً: حساب كفاءة إزالة الأملاح (Salt Rejection Rate %) تلقائياً
        [NotMapped]
        [Display(Name = "نسبة حجز الأملاح")]
        public double SaltRejectionRate
        {
            get
            {
                if (TdsInlet <= 0) return 0;
                // المعادلة الهندسية: ((الملح الداخل - الملح الخارج) / الملح الداخل) * 100
                var rejection = ((TdsInlet - TdsOutlet) / TdsInlet) * 100;
                return Math.Round(rejection, 2);
            }
        }

        // 🧠 خاصية ذكية برمجياً: التحقق التلقائي من مطابقة العينة للمواصفات القياسية (ISO 9001 / معايير مياه الشرب)
        [NotMapped]
        [Display(Name = "حالة المطابقة للمواصفات")]
        public bool IsCompliant
        {
            get
            {
                // المعايير القياسية: الأملاح المنتجة أقل من 500، الـ pH بين 6.5 و 8.5، العكارة أقل من 1 NTU
                return TdsOutlet <= 500 && Ph >= 6.5 && Ph <= 8.5 && Turbidity <= 1.0;
            }
        }
    }
}