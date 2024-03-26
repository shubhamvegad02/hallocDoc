using halloDocEntities.DataContext;
using halloDocLogic.Interfaces;
using halloDocLogic.Repository;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSession();
builder.Services.AddScoped<IHome, Home>();
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<IFormRequest, FormRequest>();
builder.Services.AddScoped<IPDashboard, PDashboard>();
builder.Services.AddScoped<IADashboard, ADashboard>();
builder.Services.AddScoped<IAdminProvider, AdminProvider>();
builder.Services.AddScoped<IAdminAccess, AdminAccess>();


/*var provider = builder.Services.BuildServiceProvider();*/
/*var config = provider.GetService<IConfiguration>();*/
builder.Services.AddDbContext<ApplicationDbContext>(item => item.UseNpgsql(builder.Configuration.GetConnectionString("dbcs")));



var app = builder.Build();



// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseSession();
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=first}/{id?}");

app.Run();


/*Scaffold - DbContext "User ID = postgres;Password=Vegad@12;Server=localhost;Port=5432;Database=halloDoc;Integrated Security=true;Pooling=true;" Npgsql.EntityFrameworkCore.PostgreSQL - OutputDir "DataModels" –context "ApplicationDbContext" –contextDir "DataContext" -f -DataAnnotations;
*/