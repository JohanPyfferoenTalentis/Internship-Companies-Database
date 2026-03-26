using Microsoft.EntityFrameworkCore;
using InternshipDB.Data;

var builder = WebApplication.CreateBuilder(args);

// 👉 DIT IS HET BELANGRIJKSTE
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=companies.db"));

builder.Services.AddRazorPages();

var app = builder.Build();

// Seed de data bij het opstarten
CompanySeed.Initialize(app);

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();