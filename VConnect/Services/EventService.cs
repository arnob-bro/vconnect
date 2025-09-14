using Microsoft.EntityFrameworkCore;
using VConnect.Database;
using VConnect.Models.Events;
using VConnect.Models.Enums;
using System;
using System.Linq;
using System.Threading.Tasks;


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
                .OrderBy(e => e.StartDateTime)
                .ToListAsync();
        }

        public async Task<IEnumerable<Event>> GetFilteredAsync(
            string? search,
            string? category,
            EventStatus? status,
            string? month,
            int page = 1,
            int pageSize = 12)
        {
            var q = _db.Events
                .Include(e => e.Roles)
                .Include(e => e.Applications)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var s = search.Trim().ToLower();
                q = q.Where(e =>
                    e.Title.ToLower().Contains(s) ||
                    e.Description.ToLower().Contains(s) ||
                    e.Location.ToLower().Contains(s));
            }

            if (!string.IsNullOrWhiteSpace(category))
            {
                // Only filter if you actually have a Category field on Event.
                // If you don’t, remove this block or add a Category string to Event.
                q = q.Where(e => e.Description.Contains(category)); // placeholder; swap to e.Category == category if you add Category
            }

            if (status.HasValue)
            {
                q = q.Where(e => e.Status == status.Value);
            }

            if (!string.IsNullOrWhiteSpace(month))
            {
                // Accept formats like "2025-09" or "September 2025"
                if (DateTime.TryParse(month + "-01", out var firstDay) ||
                    DateTime.TryParse("01 " + month, out firstDay))
                {
                    var start = new DateTime(firstDay.Year, firstDay.Month, 1, 0, 0, 0, DateTimeKind.Utc);
                    var end = start.AddMonths(1);
                    q = q.Where(e => e.StartDateTime >= start && e.StartDateTime < end);
                }
            }

            q = q.OrderBy(e => e.StartDateTime);

            // simple paging
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 12;

            return await q.Skip((page - 1) * pageSize)
                          .Take(pageSize)
                          .ToListAsync();
        }

        public async Task<Event?> GetByIdAsync(int id)
        {
            return await _db.Events
                .Include(e => e.Roles)
                .Include(e => e.Applications)
                    .ThenInclude(a => a.Role)
                .Include(e => e.Applications)
                    .ThenInclude(a => a.User)
                .Include(e => e.Participations)
                    .ThenInclude(p => p.Role)
                .Include(e => e.Participations)
                    .ThenInclude(p => p.User)
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

            // if your DB column is timestamptz, use UTC values
            existing.StartDateTime = DateTime.SpecifyKind(evt.StartDateTime, DateTimeKind.Utc);
            existing.EndDateTime = DateTime.SpecifyKind(evt.EndDateTime, DateTimeKind.Utc);

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

        // Volunteer applies (role optional)
        public async Task<EventApplication?> ApplyAsync(int eventId, int? roleId, int userId)
        {
            // Load roles + applications to pick a role if not provided
            var evt = await _db.Events
                .Include(e => e.Roles)
                    .ThenInclude(r => r.Applications)
                .FirstOrDefaultAsync(e => e.EventId == eventId);

            if (evt == null || evt.Roles == null || evt.Roles.Count == 0) return null;

            Role? role = null;

            if (roleId.HasValue)
            {
                role = evt.Roles.FirstOrDefault(r => r.EventRoleId == roleId.Value);
                if (role == null) return null;
            }
            else
            {
                // Choose the first role with available capacity (count accepted < capacity)
                role = evt.Roles.FirstOrDefault(r =>
                {
                    var accepted = r.Applications?.Count(a => a.Status == ApplicationStatus.Accepted) ?? 0;
                    return accepted < r.Capacity;
                });
                if (role == null) return null;
            }

            // Already applied for this role?
            var already = await _db.EventApplications
                .AnyAsync(a => a.EventId == eventId && a.EventRoleId == role.EventRoleId && a.UserId == userId);
            if (already) return null;

            // Capacity enforcement at apply time (based on accepted, not pending)
            var acceptedCount = role.Applications?.Count(a => a.Status == ApplicationStatus.Accepted) ?? 0;
            if (acceptedCount >= role.Capacity) return null;

            var application = new EventApplication
            {
                EventId = eventId,
                EventRoleId = role.EventRoleId,
                UserId = userId,
                Status = ApplicationStatus.Pending,
                AppliedAt = DateTime.UtcNow
            };

            _db.EventApplications.Add(application);
            await _db.SaveChangesAsync();

            // TODO: enqueue notification
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
                    .ThenInclude(r => r.Applications)
                .FirstOrDefaultAsync(a => a.EventApplicationId == appId);
            if (app == null) return false;

            if (status == ApplicationStatus.Accepted)
            {
                // Enforce role capacity at accept time
                var role = app.Role;
                if (role == null) return false;

                var acceptedCount = role.Applications?.Count(a => a.Status == ApplicationStatus.Accepted) ?? 0;
                if (acceptedCount >= role.Capacity) return false;
            }

            app.Status = status;
            await _db.SaveChangesAsync();

            // TODO: notification for accepted/rejected
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

            await _db.SaveChangesAsync();
        }

    }
}
