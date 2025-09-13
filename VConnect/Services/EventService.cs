using Microsoft.EntityFrameworkCore;
using VConnect.Database;
using VConnect.Models.Events;
using VConnect.Models.Enums;

namespace VConnect.Services
{
    public class EventService : IEventService
    {
        private readonly ApplicationDbContext _db;

        public EventService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IEnumerable<Event>> GetAllAsync()
        {
            return await _db.Events
                .Include(e => e.Roles)
                .Include(e => e.Applications)
                .ToListAsync();
        }

        public async Task<Event?> GetByIdAsync(int id)
        {
            return await _db.Events
                .Include(e => e.Roles)
                .Include(e => e.Applications)
                .ThenInclude(a => a.Role)
                .FirstOrDefaultAsync(e => e.EventId == id);
        }

        public async Task<Event> CreateAsync(Event evt)
        {
            evt.CreatedAt = DateTime.UtcNow;
            evt.UpdatedAt = DateTime.UtcNow;
            _db.Events.Add(evt);
            await _db.SaveChangesAsync();
            return evt;
        }

        public async Task<Event?> UpdateAsync(Event evt)
        {
            var existing = await _db.Events.FindAsync(evt.EventId);
            if (existing == null) return null;

            existing.Title = evt.Title;
            existing.Description = evt.Description;
            existing.Location = evt.Location;
            existing.StartDateTime = evt.StartDateTime;
            existing.EndDateTime = evt.EndDateTime;
            existing.Capacity = evt.Capacity;
            existing.Compensation = evt.Compensation;
            existing.Status = evt.Status;
            existing.UpdatedAt = DateTime.UtcNow;

            await _db.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var evt = await _db.Events.FindAsync(id);
            if (evt == null) return false;

            _db.Events.Remove(evt);
            await _db.SaveChangesAsync();
            return true;
        }

        // Volunteer applies
        public async Task<EventApplication?> ApplyAsync(int eventId, int roleId, int userId)
        {
            var role = await _db.EventRoles
                .Include(r => r.Applications)
                .FirstOrDefaultAsync(r => r.EventRoleId == roleId && r.EventId == eventId);
            if (role == null) return null;

            // Already applied?
            var already = await _db.EventApplications
                .AnyAsync(a => a.EventId == eventId && a.EventRoleId == roleId && a.UserId == userId);
            if (already) return null;

            // Capacity enforcement (accepted only)
            var acceptedCount = role.Applications?.Count(a => a.Status == ApplicationStatus.Accepted) ?? 0;
            if (acceptedCount >= role.Capacity) return null;

            var application = new EventApplication
            {
                EventId = eventId,
                EventRoleId = roleId,
                UserId = userId,
                Status = ApplicationStatus.Pending,
                AppliedAt = DateTime.UtcNow
            };

            _db.EventApplications.Add(application);
            await _db.SaveChangesAsync();

            // TODO: enqueue EventNotification
            return application;
        }

        public async Task<bool> CancelApplicationAsync(int eventApplicationId, int userId)
        {
            var app = await _db.EventApplications
                .FirstOrDefaultAsync(a => a.EventApplicationId == eventApplicationId && a.UserId == userId);
            if (app == null) return false;

            _db.EventApplications.Remove(app);
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<EventApplication>> GetApplicationsForEventAsync(int eventId)
        {
            return await _db.EventApplications
                .Include(a => a.User)
                .Include(a => a.Role)
                .Where(a => a.EventId == eventId)
                .OrderByDescending(a => a.AppliedAt)
                .ToListAsync();
        }

        public async Task<bool> UpdateApplicationStatusAsync(int appId, ApplicationStatus status)
        {
            var app = await _db.EventApplications
                .Include(a => a.Role)
                .FirstOrDefaultAsync(a => a.EventApplicationId == appId);
            if (app == null) return false;

            if (status == ApplicationStatus.Accepted)
            {
                // Enforce role capacity at accept time
                var role = await _db.EventRoles
                    .Include(r => r.Applications)
                    .FirstOrDefaultAsync(r => r.EventRoleId == app.EventRoleId);
                if (role == null) return false;

                var acceptedCount = role.Applications?.Count(a => a.Status == ApplicationStatus.Accepted) ?? 0;
                if (acceptedCount >= role.Capacity) return false;
            }

            app.Status = status;
            await _db.SaveChangesAsync();

            // TODO: EventNotification for accepted/rejected
            return true;
        }

        public async Task RecordParticipationAsync(int eventId, int userId, int roleId, int hours)
        {
            var participation = new Participation
            {
                EventId = eventId,
                UserId = userId,
                EventRoleId = roleId,
                HoursContributed = hours,
                CompletedAt = DateTime.UtcNow
            };
            _db.Participations.Add(participation);

            // Update profile counters if present
            var profile = await _db.ProfileDetails.FirstOrDefaultAsync(p => p.UserId == userId);
            if (profile != null)
            {
                profile.TotalVolunteerHours += hours;
                profile.EventsParticipated += 1;
            }

            // Mark event completed if all roles reached end time and admin wants to close separately
            await _db.SaveChangesAsync();
        }
    }
}
