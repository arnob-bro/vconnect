using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VConnect.Database;
using VConnect.Models.Cases;

namespace VConnect.Services
{
    public class StudyService : IStudyService
    {
        private readonly ApplicationDbContext _db;
        public StudyService(ApplicationDbContext db) => _db = db;

        public async Task<IEnumerable<Study>> GetAllAsync()
        {
            return await _db.Studies
                .AsNoTracking()
                .Include(s => s.GalleryImages)
                .Include(s => s.Milestones)
                .OrderByDescending(s => s.Id)
                .ToListAsync();
        }

        public async Task<Study> GetByIdAsync(int id)
        {
            return await _db.Studies
                .Include(s => s.GalleryImages)
                .Include(s => s.Milestones)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<IEnumerable<Study>> GetByCategoryAsync(string categoryName)
        {
            if (string.IsNullOrWhiteSpace(categoryName))
                return Enumerable.Empty<Study>();

            return await _db.Studies
                .AsNoTracking()
                .Where(s => s.Category == categoryName)
                .OrderByDescending(s => s.Id)
                .ToListAsync();
        }

        public async Task<Study> CreateAsync(Study study)
        {
            _db.Studies.Add(study);
            await _db.SaveChangesAsync();
            return study;
        }

        public async Task<bool> UpdateAsync(Study study)
        {
            var exists = await _db.Studies.AnyAsync(s => s.Id == study.Id);
            if (!exists) return false;
            _db.Studies.Update(study);
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var study = await _db.Studies
                .Include(s => s.GalleryImages)
                .Include(s => s.Milestones)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (study == null) return false;

            if (study.GalleryImages?.Any() == true)
                _db.CaseGalleryImages.RemoveRange(study.GalleryImages);
            if (study.Milestones?.Any() == true)
                _db.Milestones.RemoveRange(study.Milestones);

            _db.Studies.Remove(study);
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> AddImageAsync(int studyId, string imageUrl)
        {
            var studyExists = await _db.Studies.AnyAsync(s => s.Id == studyId);
            if (!studyExists) return false;

            _db.CaseGalleryImages.Add(new CaseGalleryImage
            {
                StudyId = studyId,
                ImageUrl = imageUrl
            });
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RemoveImageAsync(int imageId)
        {
            var img = await _db.CaseGalleryImages.FindAsync(imageId);
            if (img == null) return false;

            _db.CaseGalleryImages.Remove(img);
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> AddMilestoneAsync(int studyId, Milestone milestone)
        {
            var studyExists = await _db.Studies.AnyAsync(s => s.Id == studyId);
            if (!studyExists) return false;

            milestone.StudyId = studyId;
            _db.Milestones.Add(milestone);
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RemoveMilestoneAsync(int milestoneId)
        {
            var ms = await _db.Milestones.FindAsync(milestoneId);
            if (ms == null) return false;

            _db.Milestones.Remove(ms);
            await _db.SaveChangesAsync();
            return true;
        }
    }
}
