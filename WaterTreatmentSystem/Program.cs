using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using WaterTreatmentSystem.Data;

var builder = WebApplication.CreateBuilder(args);

// 1. جلب نص الاتصال وتسجيل الـ DbContext ليعمل مع SQL Server
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// 2. تسجيل الـ Identity المطور (مع إضافة دعم الأدوار وتسهيل التحقق للتطوير)
builder.Services.AddDefaultIdentity<IdentityUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = false; // جعلناها false لتسجيل الدخول مباشرة دون تفعيل الإيميل محلياً
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
})
.AddRoles<IdentityRole>() // 🔥 قمنا بإضافة هذا السطر لتفعيل الصلاحيات (مدير / مهندس / فني)
.AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddControllersWithViews();

var app = builder.Build();

//var supportedCultures = new[] {
//    new CultureInfo("ar-SY"), // الليرة السورية ل.س
//    new CultureInfo("en-US")  // الدولار الأمريكي $
//};

//// 2. إعداد خيارات اختيار اللغة والعملة
//app.UseRequestLocalization(new RequestLocalizationOptions
//{
//    DefaultRequestCulture = new RequestCulture("ar-SY"), // الثقافة الافتراضية عند أول دخول للموقع
//    SupportedCultures = supportedCultures,
//    SupportedUICultures = supportedCultures
//});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// 🔐 خطوة الأمان الحرجة: يجب التحقق من الهوية أولاً ثم الصلاحيات
app.UseAuthentication(); // 🔥 قمنا بإضافة هذا السطر لكي يدرك السيرفر الحساب المسجل
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Plants}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();