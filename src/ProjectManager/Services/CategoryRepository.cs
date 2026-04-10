using MongoDB.Driver;
using ProjectManager.Models;

namespace ProjectManager.Services;

public class CategoryRepository : ICategoryRepository
{
    private readonly IMongoDbService _mongoDb;

    public CategoryRepository(IMongoDbService mongoDb)
    {
        _mongoDb = mongoDb;
    }

    private IMongoCollection<Category> Collection =>
        _mongoDb.GetDatabase().GetCollection<Category>("categories");

    public async Task<List<Category>> GetAllAsync()
    {
        return await Collection
            .Find(Builders<Category>.Filter.Empty)
            .SortBy(c => c.Name)
            .ToListAsync();
    }

    public async Task<Category> CreateAsync(Category category)
    {
        category.CreatedAt = DateTime.UtcNow;
        await Collection.InsertOneAsync(category);
        return category;
    }

    public async Task DeleteAsync(string id)
    {
        await Collection.DeleteOneAsync(c => c.Id == id);
    }
}
