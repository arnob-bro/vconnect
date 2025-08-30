using System.Collections.Generic;
namespace VConnect.Models
{
    public class NavItem
    {
        public string Text { get; set; }
        public string Url { get; set; }
        public bool IsActive { get; set; }
        public List<NavItem> Children { get; set; } = new List<NavItem>();
    }

    public class NavViewModel
    {
        public List<NavItem> Items { get; set; } = new List<NavItem>();
        public string LogoUrl { get; set; } = "/images/j.png";
        public string ApplyNowUrl { get; set; }// null
    }
}