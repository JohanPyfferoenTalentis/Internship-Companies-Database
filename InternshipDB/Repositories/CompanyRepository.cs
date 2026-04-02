using Microsoft.EntityFrameworkCore;
using InternshipDB.Data;
using InternshipDB.Interfaces;
using InternshipDB.Models;

namespace InternshipDB.Repositories
{
    public class CompanyRepository : ICompanyRepository
    {
        private readonly AppDbContext _context;

        public CompanyRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Company>> GetAllAsync()
        {
            return await _context.Companies.ToListAsync();
        }

        public async Task<Company?> GetByIdAsync(int id)
        {
            return await _context.Companies.FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<List<string>> GetDistinctSectorsAsync()
        {
            return await _context.Companies
                .Where(c => !string.IsNullOrWhiteSpace(c.Sector))
                .Select(c => c.Sector!)
                .Distinct()
                .OrderBy(s => s.ToLower())
                .ToListAsync();
        }

        public async Task<int> CountAsync()
        {
            return await _context.Companies.CountAsync();
        }

        public async Task<List<Company>> SearchAsync(string? searchString, string? sector, string sortOrder)
        {
            var query = _context.Companies.AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchString))
            {
                var search = searchString.Trim().ToLower();
                query = query.Where(c =>
                    (c.CompanyName != null && c.CompanyName.ToLower().Contains(search)) ||
                    (c.Sector != null && c.Sector.ToLower().Contains(search)) ||
                    (c.PersonInCharge != null && c.PersonInCharge.ToLower().Contains(search)));
            }

            if (!string.IsNullOrWhiteSpace(sector))
            {
                query = query.Where(c => c.Sector == sector);
            }

            query = sortOrder == "za"
                ? query.OrderByDescending(c => (c.CompanyName ?? string.Empty).ToLower())
                : query.OrderBy(c => (c.CompanyName ?? string.Empty).ToLower());

            return await query.ToListAsync();
        }

        public async Task AddAsync(Company company)
        {
            _context.Companies.Add(company);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Company company)
        {
            _context.Attach(company).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var company = await _context.Companies.FindAsync(id);
            if (company != null)
            {
                _context.Companies.Remove(company);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<Company?> FindByRegistrationNumberAsync(string regNumber)
        {
            return await _context.Companies
                .FirstOrDefaultAsync(c => c.CompanyRegistrationNumber == regNumber.Trim());
        }

        public async Task<Company?> FindByNameAsync(string name)
        {
            return await _context.Companies
                .FirstOrDefaultAsync(c => c.CompanyName == name.Trim());
        }
    }
}
