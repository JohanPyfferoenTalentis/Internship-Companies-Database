using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using InternshipDB.Data;
using InternshipDB.Models;

namespace InternshipDB.Pages
{
    public class ImportModel : PageModel
    {
        private readonly AppDbContext _context;

        public ImportModel(AppDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public string? RawText { get; set; }

        public string? ResultMessage { get; set; }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (string.IsNullOrWhiteSpace(RawText))
            {
                ResultMessage = "No data provided.";
                return Page();
            }

            // Backup DB file if exists
            try
            {
                var contentRoot = Directory.GetCurrentDirectory();
                var dbPath = Path.Combine(contentRoot, "companies.db");
                if (System.IO.File.Exists(dbPath))
                {
                    var backup = Path.Combine(contentRoot, $"companies.db.bak.{DateTime.UtcNow:yyyyMMddHHmmss}");
                    System.IO.File.Copy(dbPath, backup);
                }
            }
            catch
            {
                // ignore backup errors
            }

            var lines = RawText.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(l => l.Trim()).Where(l => !string.IsNullOrWhiteSpace(l)).ToList();

            // Detect header
            int headerIndex = -1;
            for (int i = 0; i < Math.Min(5, lines.Count); i++)
            {
                var l = lines[i];
                if (Regex.IsMatch(l, @"\bID\b", RegexOptions.IgnoreCase) && Regex.IsMatch(l, @"\bCompany", RegexOptions.IgnoreCase))
                {
                    headerIndex = i;
                    break;
                }
            }

            List<string> headers;
            int dataStart = 0;
            if (headerIndex >= 0)
            {
                headers = SplitColumns(lines[headerIndex]);
                dataStart = headerIndex + 1;
            }
            else
            {
                // assume first line is data and known ordering
                headers = new List<string> { "ID", "CompanyName", "CompanyRegistrationNumber", "Sector", "PersonInCharge", "Address", "ContactNumber", "EmailAddress", "Website", "InternshipPeriod", "Dress-code", "Information" };
                dataStart = 0;
            }

            int created = 0, updated = 0, skipped = 0;

            for (int i = dataStart; i < lines.Count; i++)
            {
                var cols = SplitColumns(lines[i]);
                if (cols.Count == 0) continue;

                var dict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                for (int c = 0; c < headers.Count && c < cols.Count; c++)
                {
                    var key = NormalizeHeader(headers[c]);
                    dict[key] = cols[c].Trim();
                }

                // try parse ID
                int id = 0;
                if (dict.TryGetValue("id", out var idv) && int.TryParse(Regex.Match(idv, @"\d+").Value, out var parsedId))
                {
                    id = parsedId;
                }

                Company? company = null;
                if (id > 0)
                {
                    company = _context.Companies.FirstOrDefault(c => c.Id == id);
                }

                // try match on CompanyRegistrationNumber or CompanyName if not found
                if (company == null)
                {
                    if (dict.TryGetValue("companyregistrationnumber", out var reg) && !string.IsNullOrWhiteSpace(reg))
                    {
                        company = _context.Companies.FirstOrDefault(c => c.CompanyRegistrationNumber == reg.Trim());
                    }
                }

                if (company == null)
                {
                    if (dict.TryGetValue("companyname", out var name) && !string.IsNullOrWhiteSpace(name))
                    {
                        company = _context.Companies.FirstOrDefault(c => c.CompanyName == name.Trim());
                    }
                }

                var isNew = false;
                if (company == null)
                {
                    company = new Company();
                    isNew = true;
                }

                // Helper to set field if incoming value not empty
                void SetIf(string key, Action<string> setter)
                {
                    if (dict.TryGetValue(key, out var v) && !string.IsNullOrWhiteSpace(v))
                    {
                        setter(v.Trim());
                    }
                }

                SetIf("companyname", v => company.CompanyName = v);
                SetIf("companyregistrationnumber", v => company.CompanyRegistrationNumber = v);
                SetIf("sector", v => company.Sector = v);
                SetIf("personincharge", v => company.PersonInCharge = v);
                SetIf("address", v => company.Address = v);
                SetIf("contactnumber", v => company.ContactNumber = v);
                SetIf("emailaddress", v => company.Email = v);
                SetIf("website", v => company.Website = v);
                SetIf("internshipperiod", v => company.InternshipPeriod = v);
                SetIf("dress-code", v => company.DressCode = v);
                SetIf("dresscode", v => company.DressCode = v);
                SetIf("information", v => company.Information = v);
                SetIf("quality", v => company.Quality = v);

                if (isNew)
                {
                    // if ID supplied, try to set it
                    if (id > 0) company.Id = id;
                    _context.Companies.Add(company);
                    created++;
                }
                else
                {
                    updated++;
                }
            }

            await _context.SaveChangesAsync();

            ResultMessage = $"Import completed. Created: {created}, Updated: {updated}, Skipped: {skipped}";
            return Page();
        }

        private static List<string> SplitColumns(string line)
        {
            // Try tab first
            if (line.Contains('\t'))
            {
                return line.Split('\t').Select(s => s.Trim()).Where(s => s != "").ToList();
            }

            // If comma present, do a simple comma split and trim quotes
            if (line.Contains(","))
            {
                return line.Split(',')
                    .Select(s => s.Trim().Trim('"')).ToList();
            }

            // fallback: split on 2+ spaces
            var parts = Regex.Split(line, "\\s{2,}").Select(s => s.Trim()).Where(s => s != "").ToList();
            return parts.Count > 0 ? parts : new List<string> { line.Trim() };
        }

        private static string NormalizeHeader(string header)
        {
            return header?.Trim().Replace(" ", "").Replace("-", "").Replace("_", "").ToLower() ?? string.Empty;
        }
    }
}
