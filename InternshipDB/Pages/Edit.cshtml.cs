using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using InternshipDB.Data;
using InternshipDB.Models;

namespace InternshipDB.Pages
{
    public class EditModel : PageModel
    {
        private readonly InternshipDB.Data.AppDbContext _context;

        public EditModel(InternshipDB.Data.AppDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Company Company { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var company =  await _context.Companies.FirstOrDefaultAsync(m => m.Id == id);
            if (company == null)
            {
                return NotFound();
            }
            Company = company;
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Normalize trimmed fields to avoid accidental leading/trailing spaces
            Company.CompanyName = Company.CompanyName?.Trim();
            Company.CompanyRegistrationNumber = Company.CompanyRegistrationNumber?.Trim();
            Company.Sector = Company.Sector?.Trim();
            Company.PersonInCharge = Company.PersonInCharge?.Trim();
            Company.Email = Company.Email?.Trim();
            Company.ContactNumber = Company.ContactNumber?.Trim();
            Company.Website = Company.Website?.Trim();
            Company.InternshipPeriod = Company.InternshipPeriod?.Trim();
            Company.Information = Company.Information?.Trim();
            Company.Quality = Company.Quality?.Trim();
            Company.Address = Company.Address?.Trim();
            Company.DressCode = Company.DressCode?.Trim();

            _context.Attach(Company).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CompanyExists(Company.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool CompanyExists(int id)
        {
            return _context.Companies.Any(e => e.Id == id);
        }
    }
}
