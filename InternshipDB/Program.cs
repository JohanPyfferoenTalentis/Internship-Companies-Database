using Microsoft.EntityFrameworkCore;
using InternshipDB.Data;
using InternshipDB.Interfaces;
using InternshipDB.Repositories;
using InternshipDB.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=companies.db"));

builder.Services.AddScoped<ICompanyRepository, CompanyRepository>();
builder.Services.AddScoped<CompanyImportService>();

builder.Services.AddRazorPages();

var app = builder.Build();

// Seed de data bij het opstarten
CompanySeed.Initialize(app);

// Only redirect to HTTPS in production; skip for local network access
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();