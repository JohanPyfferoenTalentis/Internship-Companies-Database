using InternshipDB.Models;
using ClosedXML.Excel;
using InternshipDB.Helpers;
using Microsoft.EntityFrameworkCore;
namespace InternshipDB.Data
{
    public static class CompanySeed
    {
        public static void Initialize(WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            db.Database.EnsureCreated();
            EnsureInformationColumn(db);
            EnsureQualityColumn(db);
            EnsureCompanyRegistrationNumberColumn(db);
            EnsureAddressColumn(db);
            EnsureDressCodeColumn(db);

            db.Database.ExecuteSqlRaw("UPDATE Companies SET CompanyName = 'Unknown' WHERE CompanyName IS NULL");

            // Clear existing companies as requested
            if (db.Companies.Any())
            {
                db.Companies.RemoveRange(db.Companies);
                db.SaveChanges();
            }

            var filePath = Path.Combine(app.Environment.ContentRootPath, "companies", "tblCompanies.xlsx");
            if (File.Exists(filePath))
            {
                using var wb = new XLWorkbook(filePath);
                var ws = wb.Worksheets.First();

                var rows = ws.RowsUsed().Skip(1); // skip header
                foreach (var row in rows)
                {
                    var companyName = row.Cell(2).GetString()?.Trim();
                    if (string.IsNullOrWhiteSpace(companyName)) continue;

                    var company = new Company 
                    {
                        Id = (int?)row.Cell(1).Value.GetNumber() ?? 0,
                        CompanyName = companyName,
                        CompanyRegistrationNumber = row.Cell(3).GetString(),
                        Sector = row.Cell(4).GetString(),
                        PersonInCharge = row.Cell(5).GetString(),
                        Address = row.Cell(6).GetString(),
                        ContactNumber = row.Cell(7).GetString(),
                        Email = row.Cell(8).GetString(),
                        Website = row.Cell(9).GetString(),
                        InternshipPeriod = row.Cell(10).GetString(),
                        DressCode = row.Cell(11).GetString(),
                        Information = row.Cell(12).GetString(),
                    };

                    PrepareCompany(company);
                    if(string.IsNullOrWhiteSpace(company.InternshipPeriod) || company.InternshipPeriod == "-"){
                        var rawSource = !string.IsNullOrWhiteSpace(company.InternshipPeriod) ? company.InternshipPeriod : company.Information;
                        company.InternshipPeriod = CompanyTextHelper.GetInternshipPeriodDisplay(rawSource);
                        if (company.InternshipPeriod == "-") {
                            company.InternshipPeriod = row.Cell(10).GetString();
                        }
                    }

                    db.Companies.Add(company);
                }

                db.SaveChanges();
            }

            EnsureRequiredCompanies(db);
        }

        private static void EnsureRequiredCompanies(AppDbContext db)
        {
            var company = db.Companies.FirstOrDefault(c => c.CompanyName == "3Plex Group");

            if (company is null)
            {
                company = new Company
                {
                    CompanyName = "3Plex Group",
                    Sector = "Aviation",
                    PersonInCharge = "Paul Fenech",
                    Email = "hr@3plexgroup.com",
                    ContactNumber = "+356 99036205",
                    InternshipPeriod = "-",
                    Information = "C78062"
                };

                db.Companies.Add(company);
            }
            else
            {
                company.Sector = "Aviation";
                company.PersonInCharge = "Paul Fenech";
                company.Email = "hr@3plexgroup.com";
                company.ContactNumber = "+356 99036205";

                if (string.IsNullOrWhiteSpace(company.Information))
                {
                    company.Information = "C78062";
                }
            }

            db.SaveChanges();
        }

        private static void PrepareCompany(Company company)
        {
            var rawSource = !string.IsNullOrWhiteSpace(company.InternshipPeriod)
                ? company.InternshipPeriod
                : company.Information;

            company.InternshipPeriod = CompanyTextHelper.GetInternshipPeriodDisplay(rawSource);
            company.Information = CompanyTextHelper.GetAdditionalInformation(rawSource);
        }

        private static void NormalizeExistingCompanies(AppDbContext db)
        {
            var companies = db.Companies.ToList();
            var hasChanges = false;

            foreach (var company in companies)
            {
                var rawSource = string.Join(". ", new[] { company.InternshipPeriod, company.Information }
                    .Where(value => !string.IsNullOrWhiteSpace(value)));

                if (string.IsNullOrWhiteSpace(rawSource))
                {
                    continue;
                }

                var normalizedPeriod = CompanyTextHelper.GetInternshipPeriodDisplay(rawSource);
                var information = CompanyTextHelper.GetAdditionalInformation(rawSource);

                if (normalizedPeriod != company.InternshipPeriod || information != company.Information)
                {
                    company.InternshipPeriod = normalizedPeriod;
                    company.Information = information;
                    hasChanges = true;
                }

                // Try to extract structured fields from the information text into specific columns
                if (ExtractStructuredFields(company))
                {
                    hasChanges = true;
                }
            }

            if (hasChanges)
            {
                db.SaveChanges();
            }
        }

        // Attempt to parse company.Information and fill other fields when empty.
        private static bool ExtractStructuredFields(Company company)
        {
            var changed = false;
            if (string.IsNullOrWhiteSpace(company.Information)) return false;

            var info = company.Information;

            // Registration number patterns: C 12345, C12345, Reg: C12345, Reg: C 12345
            var regMatch = System.Text.RegularExpressions.Regex.Match(info, @"(C\s?\d{1,6}|Reg(?:istration)?[:]?\s*C\s?\d{1,6})", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            if (regMatch.Success)
            {
                var reg = regMatch.Value;
                // clean prefix
                reg = System.Text.RegularExpressions.Regex.Replace(reg, @"Reg(?:istration)?[:]?", "", System.Text.RegularExpressions.RegexOptions.IgnoreCase).Trim();
                if (string.IsNullOrWhiteSpace(company.CompanyRegistrationNumber) || company.CompanyRegistrationNumber.Trim() != reg.Trim())
                {
                    company.CompanyRegistrationNumber = reg.Trim();
                    changed = true;
                }
            }

            // Phone number - look for +356 or sequences of digits with separators
            var phoneMatch = System.Text.RegularExpressions.Regex.Match(info, @"(\+?\d[0-9 ()\-/\.]{6,20})");
            if (phoneMatch.Success)
            {
                var phone = phoneMatch.Value.Trim();
                if (string.IsNullOrWhiteSpace(company.ContactNumber) || company.ContactNumber.Trim() != phone)
                {
                    company.ContactNumber = phone;
                    changed = true;
                }
            }

            // Address: try to capture a line with street keywords or commas (heuristic)
            if (string.IsNullOrWhiteSpace(company.Address))
            {
                // heuristic: capture longer sequences (likely address) of printable chars excluding newlines
                var addrMatch = System.Text.RegularExpressions.Regex.Match(info, @"([^\r\n]{20,200})");
                if (addrMatch.Success)
                {
                    var addrCandidate = addrMatch.Value.Trim();
                    // ignore if candidate is just a phone or short
                    if (addrCandidate.Length > 20 && !System.Text.RegularExpressions.Regex.IsMatch(addrCandidate, "^\\+?\\d+$"))
                    {
                        company.Address = addrCandidate;
                        changed = true;
                    }
                }
            }

            // Dress code: look for words like 'uniform', 'casual', 'smart'
            if (string.IsNullOrWhiteSpace(company.DressCode))
            {
                var dressMatch = System.Text.RegularExpressions.Regex.Match(info, "(uniforms?|casual|smart casual|smart|comfortable|black|safety shoes)", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                if (dressMatch.Success)
                {
                    company.DressCode = dressMatch.Value.Trim();
                    changed = true;
                }
            }

            return changed;
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
