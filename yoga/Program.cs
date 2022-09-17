using System.Globalization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using yoga.Data;
using yoga.Models;
using yoga.Models.CustomTokenProviders;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
// builder.Services.AddDbContext<YogaAppDbContext>(options => options.UseSqlServer(
//     builder.Configuration.GetConnectionString("ApplicationDbContextConnection")
// ));

// builder.Services.AddIdentity<AppUser, IdentityRole> (
//     options => {
//         options.Password.RequiredLength = 7; 
//         options.Password.RequireDigit = false; 
//         options.Password.RequireUppercase = false; 
//         options.User.RequireUniqueEmail = true; 
//         options.SignIn.RequireConfirmedEmail = true;
//         options.Tokens.EmailConfirmationTokenProvider = "emailconfirmation";

//     }
// )
// .AddEntityFrameworkStores<YogaAppDbContext>()
// .AddTokenProvider<EmailConfirmationTokenProvider<AppUser>>("emailconfirmation");

builder.Services.Configure<EmailConfirmationTokenProviderOptions>(opt =>
    opt.TokenLifespan = TimeSpan.FromDays(3));


builder.Services.AddRazorPages().AddRazorRuntimeCompilation();
builder.Services.AddLocalization(opt => {opt.ResourcesPath = "Resources";});
builder.Services.AddMvc().AddViewLocalization(Microsoft.AspNetCore.Mvc.Razor.LanguageViewLocationExpanderFormat.Suffix)
.AddDataAnnotationsLocalization();

builder.Services.Configure<RequestLocalizationOptions> (
    opt => 
    {
        var supportedCultures= new List<CultureInfo>
        {
            new CultureInfo("en"),
            new CultureInfo("ar")
        };
        opt.DefaultRequestCulture = new RequestCulture("en");
        opt.SupportedCultures = supportedCultures;
        opt.SupportedUICultures = supportedCultures;
    }
);


builder.Services.AddControllersWithViews();

// var emailConfiguration = new EmailConfiguration();
// builder.Configuration.GetSection("EmailConfiguration").Bind(emailConfiguration);

// builder.Services.AddScoped<IEmailSender, EmailSender>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();



var supportedCults = new [] {"en", "ar"};
var localizationOptions = new RequestLocalizationOptions().SetDefaultCulture(supportedCults[0])
.AddSupportedCultures(supportedCults)
.AddSupportedUICultures(supportedCults);

app.UseRequestLocalization(localizationOptions);

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
