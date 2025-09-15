namespace VConnect.Models.SOS
{
    // Type of emergency selected in the form
    public enum SosEmergencyType
    {
        Fire = 1,
        Accident = 2,
        Flood = 3,
        Medical = 4,
        Other = 99
    }

    // Status of the SOS post in the feed
    public enum SosPostStatus
    {
        Urgent = 1,      // default on create
        Active = 2,      // if work is ongoing
        Completed = 3    // owner can close when resolved
    }
}
