using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using InternshipDB.Data;
using InternshipDB.Helpers;
using InternshipDB.Models;

namespace InternshipDB.Pages
{
    public class IndexModel : PageModel
    {
        private readonly AppDbContext _context;

        public IndexModel(AppDbContext context)
        {
            _context = context;
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
            var companies = _context.Companies.AsQueryable();

            Sectors = await _context.Companies
                .Where(c => !string.IsNullOrWhiteSpace(c.Sector))
                .Select(c => c.Sector!)
                .Distinct()
                // Order case-insensitively for consistent alphabetical ordering
                .OrderBy(s => s.ToLower())
                .ToListAsync();

            TotalCompanies = await _context.Companies.CountAsync();

            if (!string.IsNullOrWhiteSpace(SearchString))
            {
                var search = SearchString.Trim().ToLower();
                companies = companies.Where(c =>
                    (c.CompanyName != null && c.CompanyName.ToLower().Contains(search)) ||
                    (c.Sector != null && c.Sector.ToLower().Contains(search)) ||
                    (c.PersonInCharge != null && c.PersonInCharge.ToLower().Contains(search)));
            }

            if (!string.IsNullOrWhiteSpace(SelectedSector))
            {
                companies = companies.Where(c => c.Sector == SelectedSector);
            }

            companies = SortOrder == "za"
                ? companies.OrderByDescending(c => (c.CompanyName ?? string.Empty).ToLower())
                : companies.OrderBy(c => (c.CompanyName ?? string.Empty).ToLower());

            Company = await companies.ToListAsync();
            FilteredCompanies = Company.Count;
        }

        public string GetInternshipPeriodText(string? internshipPeriod)
            => CompanyTextHelper.GetInternshipPeriodDisplay(internshipPeriod);
    }
}
