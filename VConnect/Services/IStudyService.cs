using System.Collections.Generic;
using System.Threading.Tasks;
using VConnect.Models.Cases;

namespace VConnect.Services
{
    public interface IStudyService
    {
        // Read
        Task<IEnumerable<Study>> GetAllAsync();
        Task<Study> GetByIdAsync(int id);
        Task<IEnumerable<Study>> GetByCategoryAsync(string categoryName);

        // Write
        Task<Study> CreateAsync(Study study);
        Task<bool> UpdateAsync(Study study);
        Task<bool> DeleteAsync(int id);

        // Gallery
        Task<bool> AddImageAsync(int studyId, string imageUrl);
        Task<bool> RemoveImageAsync(int imageId);

        // Milestones
        Task<bool> AddMilestoneAsync(int studyId, Milestone milestone);
        Task<bool> RemoveMilestoneAsync(int milestoneId);
    }
}
