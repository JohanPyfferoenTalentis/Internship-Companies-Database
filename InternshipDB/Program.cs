using Microsoft.EntityFrameworkCore;
using InternshipDB.Data;

var builder = WebApplication.CreateBuilder(args);

// 👉 DIT IS HET BELANGRIJKSTE
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=companies.db"));

builder.Services.AddRazorPages();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.ExecuteSqlRaw("UPDATE Companies SET CompanyName = 'Unknown' WHERE CompanyName IS NULL");
}

// Seed de data bij het opstarten
CompanySeed.Initialize(app);

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();