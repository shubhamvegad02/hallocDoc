using hallocDoc.DataContext;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<ApplicationDbContext>();


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

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=first}/{id?}");

app.Run();


/*Scaffold - DbContext "User ID = postgres;Password=Tatva@123;Server=localhost;Port=5432;Database=EFCoreTestDB1;Integrated Security=true;Pooling=true;" Npgsql.EntityFrameworkCore.PostgreSQL - OutputDir "DataModels" –context "ApplicationDbContext" –contextDir "DataContext"*/