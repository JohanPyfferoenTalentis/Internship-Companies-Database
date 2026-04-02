using InternshipDB.Models;

namespace InternshipDB.Interfaces
{
    public interface ICompanyRepository
    {
        Task<List<Company>> GetAllAsync();
        Task<Company?> GetByIdAsync(int id);
        Task<List<string>> GetDistinctSectorsAsync();
        Task<int> CountAsync();
        Task<List<Company>> SearchAsync(string? searchString, string? sector, string sortOrder);
        Task AddAsync(Company company);
        Task UpdateAsync(Company company);
        Task DeleteAsync(int id);
        Task<Company?> FindByRegistrationNumberAsync(string regNumber);
        Task<Company?> FindByNameAsync(string name);
    }
}
