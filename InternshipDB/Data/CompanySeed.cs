using ClosedXML.Excel;
using InternshipDB.Models;
using Microsoft.EntityFrameworkCore;

namespace InternshipDB.Data
{
    public static class CompanySeed
    {
        public static void Initialize(WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            db.Database.Migrate();
            EnsureInformationColumn(db);
            EnsureQualityColumn(db);
            EnsureCompanyRegistrationNumberColumn(db);
            EnsureAddressColumn(db);
            EnsureDressCodeColumn(db);

            // Remove empty or invalid company rows
            db.Database.ExecuteSqlRaw(
                @"DELETE FROM Companies 
                  WHERE CompanyName IS NULL 
                     OR TRIM(CompanyName) = '' 
                     OR (CompanyName = 'Unknown' 
                         AND (Sector IS NULL OR TRIM(Sector) = '') 
                         AND (PersonInCharge IS NULL OR TRIM(PersonInCharge) = '') 
                         AND (Email IS NULL OR TRIM(Email) = '') 
                         AND (ContactNumber IS NULL OR TRIM(ContactNumber) = ''))");

            var filePath = Path.Combine(app.Environment.ContentRootPath, "companies", "tblCompanies.xlsx");
            if (File.Exists(filePath))
            {
                // Clear existing companies and re-import from the Excel source
                if (db.Companies.Any())
                {
                    db.Companies.RemoveRange(db.Companies);
                    db.SaveChanges();
                }

                using var wb = new XLWorkbook(filePath);
                var ws = wb.Worksheets.First();

                var rows = ws.RowsUsed().Skip(1); // skip header
                foreach (var row in rows)
                {
                    var companyName = row.Cell(2).GetString()?.Trim();
                    if (string.IsNullOrWhiteSpace(companyName)) continue;

                    var id = (int)row.Cell(1).Value.GetNumber();
                    var regNo = row.Cell(3).GetString()?.Trim();
                    var sector = row.Cell(4).GetString()?.Trim();
                    var person = row.Cell(5).GetString()?.Trim();
                    var address = row.Cell(6).GetString()?.Trim();
                    var phone = row.Cell(7).GetString()?.Trim();
                    var email = row.Cell(8).GetString()?.Trim();
                    var website = row.Cell(9).GetString()?.Trim();
                    var period = row.Cell(10).GetString()?.Trim();
                    var dressCode = row.Cell(11).GetString()?.Trim();
                    var info = row.Cell(12).GetString()?.Trim();

                    // Use raw SQL to insert with the exact ID from the Excel file
                    db.Database.ExecuteSqlRaw(
                        @"INSERT INTO Companies (Id, CompanyName, CompanyRegistrationNumber, Sector, PersonInCharge, Address, ContactNumber, Email, Website, InternshipPeriod, DressCode, Information)
                          VALUES ({0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}, {10}, {11})",
                        id, companyName,
                        (object?)regNo ?? DBNull.Value,
                        (object?)sector ?? DBNull.Value,
                        (object?)person ?? DBNull.Value,
                        (object?)address ?? DBNull.Value,
                        (object?)phone ?? DBNull.Value,
                        (object?)email ?? DBNull.Value,
                        (object?)website ?? DBNull.Value,
                        (object?)period ?? DBNull.Value,
                        (object?)dressCode ?? DBNull.Value,
                        (object?)info ?? DBNull.Value);
                }
            }
        }

        private static void EnsureInformationColumn(AppDbContext db)
        {
            if (ColumnExists(db, "Companies", "Information"))
            {
                return;
            }

            db.Database.ExecuteSqlRaw("ALTER TABLE Companies ADD COLUMN Information TEXT NULL;");
        }

        private static void EnsureQualityColumn(AppDbContext db)
        {
            if (ColumnExists(db, "Companies", "Quality"))
            {
                return;
            }

            db.Database.ExecuteSqlRaw("ALTER TABLE Companies ADD COLUMN Quality TEXT NULL;");
        }

        private static void EnsureCompanyRegistrationNumberColumn(AppDbContext db)
        {
            if (ColumnExists(db, "Companies", "CompanyRegistrationNumber"))
            {
                return;
            }

            db.Database.ExecuteSqlRaw("ALTER TABLE Companies ADD COLUMN CompanyRegistrationNumber TEXT NULL;");
        }

        private static void EnsureAddressColumn(AppDbContext db)
        {
            if (ColumnExists(db, "Companies", "Address"))
            {
                return;
            }

            db.Database.ExecuteSqlRaw("ALTER TABLE Companies ADD COLUMN Address TEXT NULL;");
        }

        private static void EnsureDressCodeColumn(AppDbContext db)
        {
            if (ColumnExists(db, "Companies", "DressCode"))
            {
                return;
            }

            db.Database.ExecuteSqlRaw("ALTER TABLE Companies ADD COLUMN DressCode TEXT NULL;");
        }

        private static bool ColumnExists(AppDbContext db, string tableName, string columnName)
        {
            using var connection = db.Database.GetDbConnection();
            if (connection.State != System.Data.ConnectionState.Open)
            {
                connection.Open();
            }

            using var command = connection.CreateCommand();
            command.CommandText = $"PRAGMA table_info({tableName});";
            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                if (string.Equals(reader[1]?.ToString(), columnName, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
