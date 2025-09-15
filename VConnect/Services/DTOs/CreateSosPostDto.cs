namespace VConnect.Services
{
    public class CreateSosPostDto
    {
        public string Name { get; set; }         // optional for logged-in users
        public string Contact { get; set; }      // phone/email
        public string Location { get; set; }     // free-text
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public string EmergencyType { get; set; } // "fire" | "accident" | "flood" | "medical" | "other"
        public string Description { get; set; }
    }
}
