using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using InternshipDB.Interfaces;
using InternshipDB.Models;

namespace InternshipDB.Pages
{
    public class DetailsModel : PageModel
    {
        private readonly ICompanyRepository _repository;

        public DetailsModel(ICompanyRepository repository)
        {
            _repository = repository;
        }

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
    }
}
