using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using InternshipDB.Helpers;
using InternshipDB.Interfaces;
using InternshipDB.Models;

namespace InternshipDB.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ICompanyRepository _repository;

        public IndexModel(ICompanyRepository repository)
        {
            _repository = repository;
        }

        public IList<Company> Company { get; set; } = default!;
        public List<string> Sectors { get; set; } = new();
        public int TotalCompanies { get; set; }
        public int FilteredCompanies { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? SearchString { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? SelectedSector { get; set; }

        [BindProperty(SupportsGet = true)]
        public string SortOrder { get; set; } = "az";

        public async Task OnGetAsync()
        {
            Sectors = await _repository.GetDistinctSectorsAsync();
            TotalCompanies = await _repository.CountAsync();
            Company = await _repository.SearchAsync(SearchString, SelectedSector, SortOrder);
            FilteredCompanies = Company.Count;
        }

        public string GetInternshipPeriodText(string? internshipPeriod)
            => CompanyTextHelper.GetInternshipPeriodDisplay(internshipPeriod);
    }
}
