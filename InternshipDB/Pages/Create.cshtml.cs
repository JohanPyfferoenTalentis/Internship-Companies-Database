using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using InternshipDB.Interfaces;
using InternshipDB.Models;

namespace InternshipDB.Pages
{
    public class CreateModel : PageModel
    {
        private readonly ICompanyRepository _repository;

        public CreateModel(ICompanyRepository repository)
        {
            _repository = repository;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public Company Company { get; set; } = default!;

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            TrimCompanyFields(Company);

            await _repository.AddAsync(Company);

            return RedirectToPage("./Index", new { SortOrder = "az" });
        }

        private static void TrimCompanyFields(Company c)
        {
            c.CompanyName = c.CompanyName?.Trim();
            c.CompanyRegistrationNumber = c.CompanyRegistrationNumber?.Trim();
            c.Sector = c.Sector?.Trim();
            c.Address = c.Address?.Trim();
            c.DressCode = c.DressCode?.Trim();
            c.PersonInCharge = c.PersonInCharge?.Trim();
            c.Email = c.Email?.Trim();
            c.ContactNumber = c.ContactNumber?.Trim();
            c.Website = c.Website?.Trim();
            c.InternshipPeriod = c.InternshipPeriod?.Trim();
            c.Information = c.Information?.Trim();
            c.Quality = c.Quality?.Trim();
        }
    }
}
