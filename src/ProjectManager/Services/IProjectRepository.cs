using ProjectManager.Models;

namespace ProjectManager.Services;

public class ProjectFilter
{
    public string? Name { get; set; }
    public string? Category { get; set; }
    public DateTime? StartDateFrom { get; set; }
    public DateTime? StartDateTo { get; set; }
    public ProjectStatus? Status { get; set; }
}

public interface IProjectRepository
{
    Task<List<Project>> GetAllAsync(ProjectFilter? filter = null);
    Task<Project?> GetByIdAsync(string id);
    Task<Project> CreateAsync(Project project);
    Task UpdateAsync(Project project);
    Task DeleteAsync(string id);
    Task<DashboardStats> GetDashboardStatsAsync();
    Task<List<Project>> GetRecentAsync(int count = 10);
}
