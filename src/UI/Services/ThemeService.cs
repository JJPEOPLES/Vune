using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Vune.UI.Models;

namespace Vune.UI.Services
{
    public class ThemeService : IThemeService
    {
        private readonly ILogger<ThemeService> _logger;
        private List<Theme> _availableThemes = new List<Theme>();
        private Theme _currentTheme;

        public ThemeService(ILogger<ThemeService> logger)
        {
            _logger = logger;
        }

        public void Initialize()
        {
            _logger.LogInformation("Initializing ThemeService");
            
            // Add default themes
            _availableThemes.Add(new Theme
            {
                Id = "vune-light",
                Name = "Vune Light",
                Type = ThemeType.Light,
                IsDefault = true
            });
            
            _availableThemes.Add(new Theme
            {
                Id = "vune-dark",
                Name = "Vune Dark",
                Type = ThemeType.Dark,
                IsDefault = false
            });
            
            _availableThemes.Add(new Theme
            {
                Id = "vune-high-contrast",
                Name = "Vune High Contrast",
                Type = ThemeType.HighContrast,
                IsDefault = false
            });
            
            // Set default theme
            _currentTheme = _availableThemes.First(t => t.IsDefault);
            ApplyTheme(_currentTheme);
        }

        public IEnumerable<Theme> GetAvailableThemes()
        {
            return _availableThemes;
        }

        public Theme GetCurrentTheme()
        {
            return _currentTheme;
        }

        public void SetTheme(string themeId)
        {
            var theme = _availableThemes.FirstOrDefault(t => t.Id == themeId);
            if (theme == null)
            {
                _logger.LogWarning($"Theme {themeId} not found");
                return;
            }
            
            _currentTheme = theme;
            ApplyTheme(theme);
        }
        
        private void ApplyTheme(Theme theme)
        {
            _logger.LogInformation($"Applying theme: {theme.Name}");
            
            // In a real implementation, this would load and apply theme resources
            var app = Application.Current;
            var resources = app.Resources;
            
            // Clear existing theme resources
            var keysToRemove = resources.MergedDictionaries
                .Where(d => d.Source?.ToString().Contains("/Themes/") ?? false)
                .ToList();
                
            foreach (var dict in keysToRemove)
            {
                resources.MergedDictionaries.Remove(dict);
            }
            
            // Add new theme resources
            var themePath = $"/Themes/{theme.Id}.xaml";
            // In a real app, we would load the actual theme file
            // resources.MergedDictionaries.Add(new ResourceDictionary { Source = new Uri(themePath, UriKind.Relative) });
            
            // For now, just set some basic colors based on theme type
            switch (theme.Type)
            {
                case ThemeType.Dark:
                    resources["BackgroundColor"] = System.Windows.Media.Colors.Black;
                    resources["ForegroundColor"] = System.Windows.Media.Colors.White;
                    break;
                case ThemeType.Light:
                    resources["BackgroundColor"] = System.Windows.Media.Colors.White;
                    resources["ForegroundColor"] = System.Windows.Media.Colors.Black;
                    break;
                case ThemeType.HighContrast:
                    resources["BackgroundColor"] = System.Windows.Media.Colors.Black;
                    resources["ForegroundColor"] = System.Windows.Media.Colors.Yellow;
                    break;
            }
        }
    }
}