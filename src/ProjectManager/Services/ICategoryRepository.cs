using ProjectManager.Models;

namespace ProjectManager.Services;

public interface ICategoryRepository
{
    Task<List<Category>> GetAllAsync();
    Task<Category> CreateAsync(Category category);
    Task DeleteAsync(string id);
}
