using MongoDB.Driver;
using ProjectManager.Models;

namespace ProjectManager.Services;

public class ProjectRepository : IProjectRepository
{
    private readonly IMongoDbService _mongoDb;

    public ProjectRepository(IMongoDbService mongoDb)
    {
        _mongoDb = mongoDb;
    }

    private IMongoCollection<Project> Collection =>
        _mongoDb.GetDatabase().GetCollection<Project>("projects");

    public async Task<List<Project>> GetAllAsync(ProjectFilter? filter = null)
    {
        var builder = Builders<Project>.Filter;
        var filters = new List<FilterDefinition<Project>>();

        if (filter is not null)
        {
            if (!string.IsNullOrWhiteSpace(filter.Name))
                filters.Add(builder.Regex("name", new MongoDB.Bson.BsonRegularExpression(filter.Name, "i")));

            if (!string.IsNullOrWhiteSpace(filter.Category))
                filters.Add(builder.Eq(p => p.Category, filter.Category));

            if (filter.StartDateFrom.HasValue)
                filters.Add(builder.Gte(p => p.StartDate, filter.StartDateFrom.Value));

            if (filter.StartDateTo.HasValue)
                filters.Add(builder.Lte(p => p.StartDate, filter.StartDateTo.Value));

            if (filter.Status.HasValue)
                filters.Add(builder.Eq(p => p.Status, filter.Status.Value));
        }

        var combined = filters.Count > 0 ? builder.And(filters) : builder.Empty;

        return await Collection
            .Find(combined)
            .SortByDescending(p => p.UpdatedAt)
            .ToListAsync();
    }

    public async Task<Project?> GetByIdAsync(string id)
    {
        return await Collection.Find(p => p.Id == id).FirstOrDefaultAsync();
    }

    public async Task<Project> CreateAsync(Project project)
    {
        project.CreatedAt = DateTime.UtcNow;
        project.UpdatedAt = DateTime.UtcNow;
        await Collection.InsertOneAsync(project);
        return project;
    }

    public async Task UpdateAsync(Project project)
    {
        project.UpdatedAt = DateTime.UtcNow;
        await Collection.ReplaceOneAsync(p => p.Id == project.Id, project);
    }

    public async Task DeleteAsync(string id)
    {
        await Collection.DeleteOneAsync(p => p.Id == id);
    }

    public async Task<DashboardStats> GetDashboardStatsAsync()
    {
        var projects = Collection;
        var planned = await projects.CountDocumentsAsync(p => p.Status == ProjectStatus.Planned);
        var inProgress = await projects.CountDocumentsAsync(p => p.Status == ProjectStatus.InProgress);
        var completed = await projects.CountDocumentsAsync(p => p.Status == ProjectStatus.Completed);

        return new DashboardStats
        {
            PlannedCount = (int)planned,
            InProgressCount = (int)inProgress,
            CompletedCount = (int)completed
        };
    }

    public async Task<List<Project>> GetRecentAsync(int count = 10)
    {
        return await Collection
            .Find(Builders<Project>.Filter.Empty)
            .SortByDescending(p => p.UpdatedAt)
            .Limit(count)
            .ToListAsync();
    }
}
