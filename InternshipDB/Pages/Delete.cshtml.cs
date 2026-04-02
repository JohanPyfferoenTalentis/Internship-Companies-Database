using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using InternshipDB.Interfaces;
using InternshipDB.Models;

namespace InternshipDB.Pages
{
    public class DeleteModel : PageModel
    {
        private readonly ICompanyRepository _repository;

        public DeleteModel(ICompanyRepository repository)
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

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
                return NotFound();

            await _repository.DeleteAsync(id.Value);

            return RedirectToPage("./Index");
        }
    }
}
