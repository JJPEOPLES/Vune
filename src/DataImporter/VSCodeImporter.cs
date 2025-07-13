using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Vune.ExtensionManager;

namespace Vune.DataImporter
{
    public class VSCodeImporter
    {
        private readonly ILogger<VSCodeImporter> _logger;
        private readonly ExtensionManager _extensionManager;

        public VSCodeImporter(ILogger<VSCodeImporter> logger, ExtensionManager extensionManager)
        {
            _logger = logger;
            _extensionManager = extensionManager;
        }

        public async Task<bool> ImportSettingsAsync(string vscodePath)
        {
            _logger.LogInformation($"Importing VS Code settings from: {vscodePath}");
            
            try
            {
                string userDataDir = Path.Combine(vscodePath, "User");
                string settingsPath = Path.Combine(userDataDir, "settings.json");
                
                if (!File.Exists(settingsPath))
                {
                    _logger.LogWarning($"VS Code settings file not found: {settingsPath}");
                    return false;
                }
                
                // Read VS Code settings
                string settingsJson = await File.ReadAllTextAsync(settingsPath);
                var settings = JsonSerializer.Deserialize<JsonElement>(settingsJson);
                
                // Convert VS Code settings to Vune settings
                var vuneSettings = ConvertVSCodeSettings(settings);
                
                // Save Vune settings
                string vuneUserDataDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Vune");
                Directory.CreateDirectory(vuneUserDataDir);
                
                string vuneSettingsPath = Path.Combine(vuneUserDataDir, "settings.json");
                await File.WriteAllTextAsync(vuneSettingsPath, JsonSerializer.Serialize(vuneSettings, new JsonSerializerOptions { WriteIndented = true }));
                
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error importing VS Code settings from: {vscodePath}");
                return false;
            }
        }

        public async Task<bool> ImportExtensionsAsync(string vscodePath)
        {
            _logger.LogInformation($"Importing VS Code extensions from: {vscodePath}");
            
            try
            {
                string extensionsDir = Path.Combine(vscodePath, "extensions");
                
                if (!Directory.Exists(extensionsDir))
                {
                    _logger.LogWarning($"VS Code extensions directory not found: {extensionsDir}");
                    return false;
                }
                
                // Get all extension directories
                var extensionDirs = Directory.GetDirectories(extensionsDir);
                int successCount = 0;
                
                foreach (var extensionDir in extensionDirs)
                {
                    string packageJsonPath = Path.Combine(extensionDir, "package.json");
                    
                    if (File.Exists(packageJsonPath))
                    {
                        try
                        {
                            string packageJsonContent = await File.ReadAllTextAsync(packageJsonPath);
                            var packageJson = JsonSerializer.Deserialize<JsonElement>(packageJsonContent);
                            
                            string extensionId = packageJson.GetProperty("name").GetString();
                            string version = packageJson.GetProperty("version").GetString();
                            
                            // Install extension in Vune
                            if (await _extensionManager.InstallExtensionAsync(extensionId, version))
                            {
                                successCount++;
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, $"Error importing extension from: {extensionDir}");
                        }
                    }
                }
                
                _logger.LogInformation($"Successfully imported {successCount} extensions out of {extensionDirs.Length}");
                return successCount > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error importing VS Code extensions from: {vscodePath}");
                return false;
            }
        }

        public async Task<bool> ImportThemesAsync(string vscodePath)
        {
            _logger.LogInformation($"Importing VS Code themes from: {vscodePath}");
            
            try
            {
                // VS Code themes are part of extensions, so we need to find theme extensions
                string extensionsDir = Path.Combine(vscodePath, "extensions");
                
                if (!Directory.Exists(extensionsDir))
                {
                    _logger.LogWarning($"VS Code extensions directory not found: {extensionsDir}");
                    return false;
                }
                
                // Get all extension directories
                var extensionDirs = Directory.GetDirectories(extensionsDir);
                int successCount = 0;
                
                foreach (var extensionDir in extensionDirs)
                {
                    string packageJsonPath = Path.Combine(extensionDir, "package.json");
                    
                    if (File.Exists(packageJsonPath))
                    {
                        try
                        {
                            string packageJsonContent = await File.ReadAllTextAsync(packageJsonPath);
                            var packageJson = JsonSerializer.Deserialize<JsonElement>(packageJsonContent);
                            
                            // Check if this extension contributes themes
                            if (packageJson.TryGetProperty("contributes", out var contributes) &&
                                contributes.TryGetProperty("themes", out var themes) &&
                                themes.ValueKind == JsonValueKind.Array &&
                                themes.GetArrayLength() > 0)
                            {
                                string extensionId = packageJson.GetProperty("name").GetString();
                                string version = packageJson.GetProperty("version").GetString();
                                
                                // Install theme extension in Vune
                                if (await _extensionManager.InstallExtensionAsync(extensionId, version))
                                {
                                    successCount++;
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, $"Error importing theme from: {extensionDir}");
                        }
                    }
                }
                
                _logger.LogInformation($"Successfully imported {successCount} theme extensions");
                return successCount > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error importing VS Code themes from: {vscodePath}");
                return false;
            }
        }

        public async Task<bool> ImportAllAsync(string vscodePath)
        {
            _logger.LogInformation($"Importing all VS Code data from: {vscodePath}");
            
            bool settingsSuccess = await ImportSettingsAsync(vscodePath);
            bool extensionsSuccess = await ImportExtensionsAsync(vscodePath);
            bool themesSuccess = await ImportThemesAsync(vscodePath);
            
            return settingsSuccess || extensionsSuccess || themesSuccess;
        }

        private Dictionary<string, object> ConvertVSCodeSettings(JsonElement vsCodeSettings)
        {
            var vuneSettings = new Dictionary<string, object>();
            
            // Map VS Code settings to Vune settings
            // This is a simplified implementation - in a real app, we would have a more comprehensive mapping
            foreach (var property in vsCodeSettings.EnumerateObject())
            {
                string key = property.Name;
                
                // Map known VS Code settings to Vune settings
                if (key.StartsWith("editor."))
                {
                    // Keep editor settings as is
                    vuneSettings[key] = GetJsonElementValue(property.Value);
                }
                else if (key.StartsWith("workbench."))
                {
                    // Map workbench settings
                    string vuneKey = key.Replace("workbench.", "ui.");
                    vuneSettings[vuneKey] = GetJsonElementValue(property.Value);
                }
                else if (key.StartsWith("files."))
                {
                    // Keep files settings as is
                    vuneSettings[key] = GetJsonElementValue(property.Value);
                }
                else if (key.StartsWith("terminal."))
                {
                    // Keep terminal settings as is
                    vuneSettings[key] = GetJsonElementValue(property.Value);
                }
                else
                {
                    // For other settings, keep them but prefix with "vscode."
                    vuneSettings[$"vscode.{key}"] = GetJsonElementValue(property.Value);
                }
            }
            
            return vuneSettings;
        }

        private object GetJsonElementValue(JsonElement element)
        {
            switch (element.ValueKind)
            {
                case JsonValueKind.String:
                    return element.GetString();
                case JsonValueKind.Number:
                    if (element.TryGetInt32(out int intValue))
                    {
                        return intValue;
                    }
                    return element.GetDouble();
                case JsonValueKind.True:
                    return true;
                case JsonValueKind.False:
                    return false;
                case JsonValueKind.Null:
                    return null;
                case JsonValueKind.Object:
                    var obj = new Dictionary<string, object>();
                    foreach (var property in element.EnumerateObject())
                    {
                        obj[property.Name] = GetJsonElementValue(property.Value);
                    }
                    return obj;
                case JsonValueKind.Array:
                    var array = new List<object>();
                    foreach (var item in element.EnumerateArray())
                    {
                        array.Add(GetJsonElementValue(item));
                    }
                    return array;
                default:
                    return null;
            }
        }
    }
}