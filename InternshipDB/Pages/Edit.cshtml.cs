using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using InternshipDB.Interfaces;
using InternshipDB.Models;

namespace InternshipDB.Pages
{
    public class EditModel : PageModel
    {
        private readonly ICompanyRepository _repository;

        public EditModel(ICompanyRepository repository)
        {
            _repository = repository;
        }

        [BindProperty]
        public Company Company { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
                return NotFound();

            var company = await _repository.GetByIdAsync(id.Value);
            if (company == null)
                return NotFound();

            Company = company;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            TrimCompanyFields(Company);

            try
            {
                await _repository.UpdateAsync(Company);
            }
            catch (DbUpdateConcurrencyException)
            {
                var exists = await _repository.GetByIdAsync(Company.Id);
                if (exists == null)
                    return NotFound();
                throw;
            }

            return RedirectToPage("./Index");
        }

        private static void TrimCompanyFields(Company c)
        {
            c.CompanyName = c.CompanyName?.Trim();
            c.CompanyRegistrationNumber = c.CompanyRegistrationNumber?.Trim();
            c.Sector = c.Sector?.Trim();
            c.PersonInCharge = c.PersonInCharge?.Trim();
            c.Email = c.Email?.Trim();
            c.ContactNumber = c.ContactNumber?.Trim();
            c.Website = c.Website?.Trim();
            c.InternshipPeriod = c.InternshipPeriod?.Trim();
            c.Information = c.Information?.Trim();
            c.Quality = c.Quality?.Trim();
            c.Address = c.Address?.Trim();
            c.DressCode = c.DressCode?.Trim();
        }
    }
}
