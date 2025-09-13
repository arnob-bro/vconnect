namespace VConnect.Models.Events
{
    public class Role
    {
        public int EventRoleId { get; set; }  // PK
        public int EventId { get; set; }      // FK → Event

        public string RoleName { get; set; } = string.Empty;
        public int Capacity { get; set; }    // how many volunteers needed

        // Navigation
        public Event Event { get; set; } = null!;
        public ICollection<EventApplication> Applications { get; set; } = new List<EventApplication>();
    }
}
