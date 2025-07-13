using System.Collections.Generic;
using Vune.UI.Models;

namespace Vune.UI.Services
{
    public interface IThemeService
    {
        void Initialize();
        IEnumerable<Theme> GetAvailableThemes();
        Theme GetCurrentTheme();
        void SetTheme(string themeId);
    }
}