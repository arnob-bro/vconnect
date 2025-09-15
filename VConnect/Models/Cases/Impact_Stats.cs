using System.ComponentModel.DataAnnotations;

namespace VConnect.Models.Cases
{
    public class Impact_Stats
    {
        [Key]
        public int Id { get; set; }

        public int TotalCases { get; set; }
        public int CommunitiesImpacted { get; set; }
        public int VolunteersInvolved { get; set; }
        public int SuccessRate { get; set; }
    }
}
