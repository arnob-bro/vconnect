using VConnect.Models.Events;
using System.Collections.Generic;

namespace VConnect.Areas.Admin.Models
{
    public class AdminEventViewModel
    {
        public int TotalEvents { get; set; }
        public List<Event> AllEvents { get; set; } = new List<Event>();
    }
}
