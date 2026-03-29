using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using InternshipDB.Data;
using InternshipDB.Models;

namespace InternshipDB.Pages
{
    public class CreateModel : PageModel
    {
        private readonly InternshipDB.Data.AppDbContext _context;

        public CreateModel(InternshipDB.Data.AppDbContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public Company Company { get; set; } = default!;

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            // Normalize/trim input so sorting and distinct operations behave consistently
            Company.CompanyName = Company.CompanyName?.Trim();
            Company.CompanyRegistrationNumber = Company.CompanyRegistrationNumber?.Trim();
            Company.Sector = Company.Sector?.Trim();
            Company.Address = Company.Address?.Trim();
            Company.DressCode = Company.DressCode?.Trim();
            Company.PersonInCharge = Company.PersonInCharge?.Trim();
            Company.Email = Company.Email?.Trim();
            Company.ContactNumber = Company.ContactNumber?.Trim();
            Company.Website = Company.Website?.Trim();
            Company.InternshipPeriod = Company.InternshipPeriod?.Trim();
            Company.Information = Company.Information?.Trim();
            Company.Quality = Company.Quality?.Trim();

            _context.Companies.Add(Company);
            await _context.SaveChangesAsync();

            // Ensure we return to the index with alphabetical sort so the newly added
            // company appears in the correct position.
            return RedirectToPage("./Index", new { SortOrder = "az" });
        }
    }
}
