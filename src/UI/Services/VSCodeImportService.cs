using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Runtime.InteropServices;

namespace Vune.UI.Services
{
    public class VSCodeImportService : IVSCodeImportService
    {
        private readonly ILogger<VSCodeImportService> _logger;
        private readonly IExtensionService _extensionService;
        private readonly IThemeService _themeService;
        
        // P/Invoke for the native Core API
        [DllImport("Core.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern bool ImportVSCodeData(string vscodePath, bool importSettings, bool importExtensions, bool importThemes);

        public VSCodeImportService(
            ILogger<VSCodeImportService> logger,
            IExtensionService extensionService,
            IThemeService themeService)
        {
            _logger = logger;
            _extensionService = extensionService;
            _themeService = themeService;
        }

        public bool ImportSettings(string vscodePath)
        {
            _logger.LogInformation($"Importing VS Code settings from: {vscodePath}");
            
            try
            {
                // Validate VS Code path
                if (!IsValidVSCodePath(vscodePath))
                {
                    _logger.LogWarning($"Invalid VS Code path: {vscodePath}");
                    return false;
                }
                
                // In a real implementation, this would call the Core API
                // return ImportVSCodeData(vscodePath, true, false, false);
                
                // Mock implementation
                _logger.LogInformation("Successfully imported VS Code settings");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error importing VS Code settings from: {vscodePath}");
                return false;
            }
        }

        public bool ImportExtensions(string vscodePath)
        {
            _logger.LogInformation($"Importing VS Code extensions from: {vscodePath}");
            
            try
            {
                // Validate VS Code path
                if (!IsValidVSCodePath(vscodePath))
                {
                    _logger.LogWarning($"Invalid VS Code path: {vscodePath}");
                    return false;
                }
                
                // In a real implementation, this would call the Core API
                // return ImportVSCodeData(vscodePath, false, true, false);
                
                // Mock implementation - install some common VS Code extensions
                _extensionService.InstallExtension("ms-vscode.cpptools");
                _extensionService.InstallExtension("ms-dotnettools.csharp");
                _extensionService.InstallExtension("vscode-icons-team.vscode-icons");
                
                _logger.LogInformation("Successfully imported VS Code extensions");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error importing VS Code extensions from: {vscodePath}");
                return false;
            }
        }

        public bool ImportThemes(string vscodePath)
        {
            _logger.LogInformation($"Importing VS Code themes from: {vscodePath}");
            
            try
            {
                // Validate VS Code path
                if (!IsValidVSCodePath(vscodePath))
                {
                    _logger.LogWarning($"Invalid VS Code path: {vscodePath}");
                    return false;
                }
                
                // In a real implementation, this would call the Core API
                // return ImportVSCodeData(vscodePath, false, false, true);
                
                // Mock implementation
                _logger.LogInformation("Successfully imported VS Code themes");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error importing VS Code themes from: {vscodePath}");
                return false;
            }
        }

        public bool ImportAll(string vscodePath)
        {
            _logger.LogInformation($"Importing all VS Code data from: {vscodePath}");
            
            try
            {
                // Validate VS Code path
                if (!IsValidVSCodePath(vscodePath))
                {
                    _logger.LogWarning($"Invalid VS Code path: {vscodePath}");
                    return false;
                }
                
                // Import settings, extensions, and themes
                bool settingsSuccess = ImportSettings(vscodePath);
                bool extensionsSuccess = ImportExtensions(vscodePath);
                bool themesSuccess = ImportThemes(vscodePath);
                
                return settingsSuccess && extensionsSuccess && themesSuccess;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error importing all VS Code data from: {vscodePath}");
                return false;
            }
        }

        public string DetectVSCodePath()
        {
            _logger.LogInformation("Detecting VS Code path");
            
            try
            {
                // Try to detect VS Code installation path
                string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                string vscodePath = Path.Combine(appDataPath, "Code");
                
                if (Directory.Exists(vscodePath))
                {
                    _logger.LogInformation($"Detected VS Code path: {vscodePath}");
                    return vscodePath;
                }
                
                // Try alternative locations
                string localAppDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                vscodePath = Path.Combine(localAppDataPath, "Programs", "Microsoft VS Code");
                
                if (Directory.Exists(vscodePath))
                {
                    _logger.LogInformation($"Detected VS Code path: {vscodePath}");
                    return vscodePath;
                }
                
                _logger.LogWarning("Could not detect VS Code path");
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error detecting VS Code path");
                return null;
            }
        }
        
        private bool IsValidVSCodePath(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                return false;
            }
            
            if (!Directory.Exists(path))
            {
                return false;
            }
            
            // Check for some VS Code-specific directories or files
            string userDataDir = Path.Combine(path, "User");
            string extensionsDir = Path.Combine(path, "extensions");
            
            return Directory.Exists(userDataDir) || Directory.Exists(extensionsDir);
        }
    }
}