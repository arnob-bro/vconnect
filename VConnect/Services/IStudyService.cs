using System.Collections.Generic;
using System.Threading.Tasks;
using VConnect.Models.Cases;

namespace VConnect.Services
{
    public interface IStudyService
    {
        Task<IEnumerable<Study>> GetAllAsync();
        Task<Study> GetByIdAsync(int id);
        Task<IEnumerable<Study>> GetByCategoryAsync(string categoryName);
        Task<Study> CreateAsync(Study study);
        Task<bool> UpdateAsync(Study study);
        Task<bool> DeleteAsync(int id);
        Task<bool> AddImageAsync(int studyId, string imageUrl);
        Task<bool> RemoveImageAsync(int imageId);
        Task<bool> AddMilestoneAsync(int studyId, Milestone milestone);
        Task<bool> RemoveMilestoneAsync(int milestoneId);
    }
}
