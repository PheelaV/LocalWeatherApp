namespace LocalWeatherApp.Models
{
    public enum MenuItemType
    {
        Browse,
        Weather,
        About
    }
    public class HomeMenuItem
    {
        public MenuItemType Id { get; set; }

        public string Title { get; set; }
    }
}
