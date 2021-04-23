using System.Collections.Generic;

namespace LimitedPower.UI
{
    public static class PagesList
    {
        public static List<Page> Pages = new()
        {
            new("Tier List", "/", "badge"),
            new("Best commons", "topcommonspage", "layers"),
            new("Combat Tricks", "combattrickspage", "bolt"),
        };
    }

    public class Page
    {
        public string Name { get; set; }
        public string Href { get; set; }

        public string Icon { get; set; }

        public Page(string name, string href, string icon)
        {
            Name = name;
            Href = href;
            Icon = icon;
        }
    }
}
