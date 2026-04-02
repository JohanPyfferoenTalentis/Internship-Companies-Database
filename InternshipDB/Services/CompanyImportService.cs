using System.Text.RegularExpressions;
using InternshipDB.Interfaces;
using InternshipDB.Models;

namespace InternshipDB.Services
{
    public class CompanyImportResult
    {
        public int Created { get; set; }
        public int Updated { get; set; }
        public int Skipped { get; set; }
    }

    public class CompanyImportService
    {
        private readonly ICompanyRepository _repository;

        public CompanyImportService(ICompanyRepository repository)
        {
            _repository = repository;
        }

        public async Task<CompanyImportResult> ImportFromTextAsync(string rawText)
        {
            var result = new CompanyImportResult();

            var lines = rawText.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(l => l.Trim()).Where(l => !string.IsNullOrWhiteSpace(l)).ToList();

            int headerIndex = DetectHeaderIndex(lines);

            List<string> headers;
            int dataStart;
            if (headerIndex >= 0)
            {
                headers = SplitColumns(lines[headerIndex]);
                dataStart = headerIndex + 1;
            }
            else
            {
                headers = new List<string>
                {
                    "ID", "CompanyName", "CompanyRegistrationNumber", "Sector",
                    "PersonInCharge", "Address", "ContactNumber", "EmailAddress",
                    "Website", "InternshipPeriod", "Dress-code", "Information"
                };
                dataStart = 0;
            }

            for (int i = dataStart; i < lines.Count; i++)
            {
                var cols = SplitColumns(lines[i]);
                if (cols.Count == 0) continue;

                var dict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                for (int c = 0; c < headers.Count && c < cols.Count; c++)
                {
                    dict[NormalizeHeader(headers[c])] = cols[c].Trim();
                }

                int id = 0;
                if (dict.TryGetValue("id", out var idv) && int.TryParse(Regex.Match(idv, @"\d+").Value, out var parsedId))
                {
                    id = parsedId;
                }

                Company? company = null;
                if (id > 0)
                    company = await _repository.GetByIdAsync(id);

                if (company == null && dict.TryGetValue("companyregistrationnumber", out var reg) && !string.IsNullOrWhiteSpace(reg))
                    company = await _repository.FindByRegistrationNumberAsync(reg);

                if (company == null && dict.TryGetValue("companyname", out var name) && !string.IsNullOrWhiteSpace(name))
                    company = await _repository.FindByNameAsync(name);

                var isNew = company == null;
                company ??= new Company();

                SetIf(dict, "companyname", v => company.CompanyName = v);
                SetIf(dict, "companyregistrationnumber", v => company.CompanyRegistrationNumber = v);
                SetIf(dict, "sector", v => company.Sector = v);
                SetIf(dict, "personincharge", v => company.PersonInCharge = v);
                SetIf(dict, "address", v => company.Address = v);
                SetIf(dict, "contactnumber", v => company.ContactNumber = v);
                SetIf(dict, "emailaddress", v => company.Email = v);
                SetIf(dict, "website", v => company.Website = v);
                SetIf(dict, "internshipperiod", v => company.InternshipPeriod = v);
                SetIf(dict, "dresscode", v => company.DressCode = v);
                SetIf(dict, "information", v => company.Information = v);
                SetIf(dict, "quality", v => company.Quality = v);

                if (isNew)
                {
                    if (id > 0) company.Id = id;
                    await _repository.AddAsync(company);
                    result.Created++;
                }
                else
                {
                    await _repository.UpdateAsync(company);
                    result.Updated++;
                }
            }

            return result;
        }

        private static int DetectHeaderIndex(List<string> lines)
        {
            for (int i = 0; i < Math.Min(5, lines.Count); i++)
            {
                if (Regex.IsMatch(lines[i], @"\bID\b", RegexOptions.IgnoreCase) &&
                    Regex.IsMatch(lines[i], @"\bCompany", RegexOptions.IgnoreCase))
                {
                    return i;
                }
            }
            return -1;
        }

        private static void SetIf(Dictionary<string, string> dict, string key, Action<string> setter)
        {
            if (dict.TryGetValue(key, out var v) && !string.IsNullOrWhiteSpace(v))
                setter(v.Trim());
        }

        private static List<string> SplitColumns(string line)
        {
            if (line.Contains('\t'))
                return line.Split('\t').Select(s => s.Trim()).Where(s => s != "").ToList();

            if (line.Contains(','))
                return line.Split(',').Select(s => s.Trim().Trim('"')).ToList();

            var parts = Regex.Split(line, @"\s{2,}").Select(s => s.Trim()).Where(s => s != "").ToList();
            return parts.Count > 0 ? parts : new List<string> { line.Trim() };
        }

        private static string NormalizeHeader(string header)
        {
            return header.Trim().Replace(" ", "").Replace("-", "").Replace("_", "").ToLower();
        }
    }
}
