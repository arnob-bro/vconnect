using VConnect.Models.Events;
using VConnect.Models.Enums;

namespace VConnect.Services
{
    public interface IEventService
    {
        Task<IEnumerable<Event>> GetAllAsync();
        Task<Event?> GetByIdAsync(int id);
        Task<Event> CreateAsync(Event evt);
        Task<Event?> UpdateAsync(Event evt);
        Task<bool> DeleteAsync(int id);

        Task<EventApplication?> ApplyAsync(int eventId, int roleId, int userId);
        Task<bool> CancelApplicationAsync(int eventApplicationId, int userId);
        Task<IEnumerable<EventApplication>> GetApplicationsForEventAsync(int eventId);
        Task<bool> UpdateApplicationStatusAsync(int appId, ApplicationStatus status);

        Task RecordParticipationAsync(int eventId, int userId, int roleId, int hours);
    }
}
