namespace Vune.UI.Models
{
    public enum ThemeType
    {
        Light,
        Dark,
        HighContrast
    }

    public class Theme
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public ThemeType Type { get; set; }
        public bool IsDefault { get; set; }
    }
}