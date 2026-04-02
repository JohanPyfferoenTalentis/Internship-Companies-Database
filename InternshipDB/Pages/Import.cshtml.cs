using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using InternshipDB.Services;

namespace InternshipDB.Pages
{
    public class ImportModel : PageModel
    {
        private readonly CompanyImportService _importService;

        public ImportModel(CompanyImportService importService)
        {
            _importService = importService;
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

            BackupDatabase();

            var result = await _importService.ImportFromTextAsync(RawText);

            ResultMessage = $"Import completed. Created: {result.Created}, Updated: {result.Updated}, Skipped: {result.Skipped}";
            return Page();
        }

        private static void BackupDatabase()
        {
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
        }
    }
}
