using VConnect.Models;
using VConnect.Models.Events;

namespace VConnect.Areas.Admin.Models
{
    public class AdminDashboardViewModel
    {
        public decimal TotalDonationGot { get; set; }
        //public decimal TotalDonationGave { get; set; }
        public int TotalEvents { get; set; }
        public int TotalVolunteers { get; set; }

        public List<Event> LastEvents { get; set; }
        public List<Donation> LastDonationsGot { get; set; }
        //public List<Donation> LastDonationsGave { get; set; }
        
    }

}
